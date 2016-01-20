using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace InnovatorAdmin.Controls
{
  public partial class MergeInterface : UserControl, IWizardStep
  {
    private IMergeOperation _mergeOp;
    private FullBindingList<FileCompare> _mergeData;
    private IWizard _wizard;

    public MergeInterface()
    {
      InitializeComponent();

      chkStatus.CheckOnClick = true;
      grid.AutoGenerateColumns = false;
      foreach (var val in Enum.GetValues(typeof(MergeStatus)))
      {
        chkStatus.Items.Add(val, true);
      }
    }

    public MergeInterface Initialize(IMergeOperation op)
    {
      _mergeOp = op;
      _mergeData = new FullBindingList<FileCompare>(_mergeOp.GetChanges());
      var existing = GetExisting();
      MergeStatus status;
      foreach (var item in _mergeData)
      {
        if (existing.TryGetValue(item.Path, out status))
          item.ResolutionStatus = status;
      }
      lblFilter.Text = string.Format("Filter: ({0} row(s))", _mergeData.Count);
      _mergeData.ListChanged += _mergeData_ListChanged;
      _mergeData.SortChanging += _mergeData_SortChanging;
      grid.DataSource = _mergeData;
      return this;
    }

    void _mergeData_SortChanging(object sender, SortChangingEventArgs e)
    {
      if (e.Sort != null && !e.Sort.OfType<ListSortDescription>().Any(d => d.PropertyDescriptor.Name == "Path"))
      {
        e.Sort = new ListSortDescriptionCollection(e.Sort.OfType<ListSortDescription>().Concat(_mergeData.SortDescriptors("Path")).ToArray());
        e.Handled = true;
      }
    }

    void _mergeData_ListChanged(object sender, ListChangedEventArgs e)
    {
      lblFilter.Text = string.Format("Filter: ({0} row(s))", _mergeData.Count);
    }

    private void UpdateFilter(IList<MergeStatus> checkedItems = null)
    {
      if (_mergeData == null) return;

      checkedItems = checkedItems ?? chkStatus.CheckedItems.OfType<MergeStatus>().ToArray();

      if (string.IsNullOrEmpty(txtFilter.Text)
        && checkedItems.Count == chkStatus.Items.Count)
      {
        _mergeData.RemoveFilter();
      }
      else
      {
        _mergeData.ApplyFilter(c =>
            (string.IsNullOrEmpty(txtFilter.Text)
          || c.Path.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0)
          && checkedItems.Contains(c.ResolutionStatus));
      }
    }

    public void Configure(IWizard wizard)
    {
      wizard.NextEnabled = true;
      wizard.NextLabel = "Process Remaining";
      _wizard = wizard;
    }

    public void GoNext()
    {
      var processor = new ProcessFiles();
      var prog = new ProgressStep<ProcessFiles>(processor);
      prog.MethodInvoke = e =>
      {
        e.Execute(_mergeData.Unfiltered, _mergeOp);
      };
      prog.GoNextAction = () => _wizard.GoToStep(this);
      _wizard.GoToStep(prog);
    }

    private void chkStatus_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      var checkedItems = chkStatus.CheckedIndices.OfType<int>()
        .Where(i => i != e.Index)
        .Select(i => (MergeStatus)chkStatus.Items[i]).ToList();
      if (e.NewValue == CheckState.Checked)
      {
        checkedItems.Add((MergeStatus)chkStatus.Items[e.Index]);
      }
      UpdateFilter(checkedItems);
    }

    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
      UpdateFilter();
    }

    private void ShowContextMenu(Point pt)
    {
      conActions.Show(pt);
    }

    private void grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      try
      {
        if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
        {
          if (!grid[e.ColumnIndex, e.RowIndex].Selected)
          {
            grid.ClearSelection();
            grid.Rows[e.RowIndex].Selected = true;
            grid.CurrentCell = grid[e.ColumnIndex, e.RowIndex];
          }

          ShowContextMenu(Cursor.Position);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void grid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      try
      {
        if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
        {
          if (!grid[e.ColumnIndex, e.RowIndex].Selected)
          {
            grid.ClearSelection();
            grid.Rows[e.RowIndex].Selected = true;
            grid.CurrentCell = grid[e.ColumnIndex, e.RowIndex];
          }

          MergeFiles();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnActions_Click(object sender, EventArgs e)
    {
      ShowContextMenu(btnActions.Parent.PointToScreen(new Point(btnActions.Left, btnActions.Bottom)));
    }

    private void mniMarkUnresolved_Click(object sender, EventArgs e)
    {
      try
      {
        var items = grid.SelectedRows.OfType<DataGridViewRow>().Select(r => (FileCompare)r.DataBoundItem).ToList();
        foreach (var item in items)
        {
          item.ResolutionStatus = MergeStatus.UnresolvedConflict;
        }
        ProcessManualUpdate();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniMerge_Click(object sender, EventArgs e)
    {
      try
      {
        MergeFiles();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void MergeFiles()
    {
      var items = grid.SelectedRows.OfType<DataGridViewRow>().Select(r => (FileCompare)r.DataBoundItem).ToList();
      foreach (var item in items)
      {
        var paths = _mergeOp.GetPaths(item.Path);
        var args = "\"" + paths.Base + "\""
          + " \"" + paths.Local + "\""
          + " \"" + paths.Remote + "\""
          + " \"" + paths.Merged + "\"";
        var checksum = Md5HashFile(paths.Merged);
        var proc = Process.Start(@"c:\Program Files\Perforce\p4merge.exe", args);
        proc.WaitForExit();
        var newChecksum = Md5HashFile(paths.Merged);
        if (newChecksum != checksum
          || Dialog.MessageDialog.Show("The merge file appears not to have changed.  Do you still want to mark it resolved anyway?",
          "Merge Resolution", "Mark &Resolved", "&Ignore") == DialogResult.OK)
          item.ResolutionStatus = MergeStatus.ResolvedConflict;

        CleanupTempFiles(paths.Base, paths.Local, paths.Merged, paths.Remote);
      }
      ProcessManualUpdate();
    }

    private void CleanupTempFiles(params string[] paths)
    {
      var tempDir = Path.GetTempPath().TrimEnd('\\');

      foreach (var path in paths)
      {
        try
        {
          if (Path.GetDirectoryName(path).StartsWith(tempDir, StringComparison.OrdinalIgnoreCase)
            && File.Exists(path))
            File.Delete(path);
        }
        catch (IOException) { }
      }
    }

    private string Md5HashFile(string path)
    {
      if (!File.Exists(path)) return string.Empty;

      using (var md5 = System.Security.Cryptography.MD5.Create())
      {
        using (var stream = File.OpenRead(path))
        {
          return Convert.ToBase64String(md5.ComputeHash(stream));
        }
      }
    }

    private void mniTakeLocal_Click(object sender, EventArgs e)
    {
      try
      {
        var items = grid.SelectedRows.OfType<DataGridViewRow>().Select(r => (FileCompare)r.DataBoundItem).ToList();
        foreach (var item in items)
        {
          item.ResolutionStatus = MergeStatus.TakeLocal;
        }
        ProcessManualUpdate();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniTakeRemote_Click(object sender, EventArgs e)
    {
      try
      {
        var items = grid.SelectedRows.OfType<DataGridViewRow>().Select(r => (FileCompare)r.DataBoundItem).ToList();
        foreach (var item in items)
        {
          item.ResolutionStatus = MergeStatus.TakeRemote;
        }
        ProcessManualUpdate();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void ProcessManualUpdate()
    {
      UpdateFilter();
      var lines = (from i in _mergeData.Unfiltered
                  where i.IsModified
                  select i.Path + "|" + ((int)i.ResolutionStatus).ToString()).ToArray();
      File.WriteAllText(GetPersistedFile(), lines.GroupConcat(Environment.NewLine, i => i));
    }
    private void ProcessTakes()
    {

    }
    private Dictionary<string, MergeStatus> GetExisting()
    {
      var path = GetPersistedFile();
      if (!File.Exists(path)) return new Dictionary<string,MergeStatus>();
      return (from l in File.ReadAllLines(GetPersistedFile())
              where !string.IsNullOrEmpty(l)
              let parts = l.Split('|')
              select new { Path = parts[0], Status = (MergeStatus)int.Parse(parts[1]) })
             .ToDictionary(i => i.Path, i => i.Status);
    }

    internal static string GetPersistedFile()
    {
      string path = @"{0}\{1}\MergeStatus.txt";
      return string.Format(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Innovator Admin");
    }

    private void grid_KeyDown(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode == Keys.Enter)
        {
          MergeFiles();
          e.Handled = true;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private class ProcessFiles : IProgressReporter
    {
      public event EventHandler<ActionCompleteEventArgs> ActionComplete;
      public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

      private static int ExecCmd(string fileName, string args, string workingDir, StringBuilder output = null)
      {
        var startInfo = new ProcessStartInfo();
        startInfo.FileName = fileName;
        startInfo.Arguments = args;
        startInfo.WorkingDirectory = workingDir;
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        var proc = Process.Start(startInfo);
        char chr;
        while (!proc.StandardOutput.EndOfStream)
        {
          chr = Convert.ToChar(proc.StandardOutput.Read());
          Console.Write(chr);
          if (output != null) output.Append(chr);
        }
        proc.WaitForExit();
        return proc.ExitCode;
      }

      private enum WorkingDirStatus
      {
        Removed,
        Added,
        Modified
      }

      private Dictionary<string, WorkingDirStatus> GetStatus(string statusText)
      {
        string line;
        WorkingDirStatus status;
        var result = new Dictionary<string, WorkingDirStatus>(StringComparer.OrdinalIgnoreCase);

        using (var reader = new StringReader(statusText))
        {
          while (reader.Peek() > 0)
          {
            line = reader.ReadLine();
            if (!string.IsNullOrWhiteSpace(line) && line.Length > 2)
            {
              status = WorkingDirStatus.Modified;
              switch (line[0])
              {
                case 'M':
                  status = WorkingDirStatus.Modified;
                  break;
                case 'A':
                  status = WorkingDirStatus.Added;
                  break;
                case '!':
                  status = WorkingDirStatus.Removed;
                  break;
              }

              result.Add(line.Substring(2).Replace('\\', '/'), status);
            }
          }
        }

        return result;
      }

      public void Execute(IEnumerable<FileCompare> files, IMergeOperation mergeOp)
      {
        try
        {
          var output = new StringBuilder();
          ExecCmd("hg", "status", mergeOp.MergePath(""), output);
          var statuses = GetStatus(output.ToString());
          WorkingDirStatus status;

          var toProcess = files.Where(f =>
                f.ResolutionStatus == MergeStatus.TakeLocal
              || f.ResolutionStatus == MergeStatus.TakeRemote)
            .ToArray();

          FileCompare file;
          for (var i = 0; i < toProcess.Length; i++ )
          {
            file = toProcess[i];
            var path = mergeOp.MergePath(file.Path);
            if (file.ResolutionStatus == MergeStatus.TakeLocal)
            {
              if (file.InLocal == FileStatus.DoesntExist)
              {
                if (File.Exists(path))
                  File.Delete(path);
              }
              else
              {
                if (statuses.ContainsKey(file.Path))
                {
                  Directory.CreateDirectory(Path.GetDirectoryName(path));
                  using (var write = new FileStream(path, FileMode.Create, FileAccess.Write))
                  {
                    mergeOp.GetLocal(file.Path).CopyTo(write);
                  }
                }
              }
            }
            else
            {
              if (File.Exists(path))
                File.Delete(path);

              if (file.InRemote != FileStatus.DoesntExist)
              {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var write = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                  mergeOp.GetRemote(file.Path).CopyTo(write);
                }
              }
            }
            OnProgressChanged("Processing files...", ((i + 1) * 100) / toProcess.Length);
          }

          OnActionComplete();
        }
        catch (Exception ex)
        {
          OnActionComplete(ex);
        }
      }

      protected virtual void OnActionComplete(Exception ex = null)
      {
        if (ActionComplete != null)
          ActionComplete.Invoke(this, new ActionCompleteEventArgs()
          {
            Exception = ex
          });
      }
      protected virtual void OnProgressChanged(string message, int progress)
      {
        if (ProgressChanged != null)
          ProgressChanged.Invoke(this, new ProgressChangedEventArgs(message, progress));
      }
    }
  }
}
