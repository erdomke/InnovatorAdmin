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
      wizard.NextEnabled = false;
    }

    public void GoNext()
    {
      // Do nothing
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
          || MessageBox.Show("The merge file appears not to have changed.  Do you still want to mark it resolved anyway?", 
          "Merge Resolution", MessageBoxButtons.YesNo) == DialogResult.Yes)
          item.ResolutionStatus = MergeStatus.ResolvedConflict;
      }
      ProcessManualUpdate();
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
          //if (item.InLocal == FileStatus.DoesntExist)
          //{
          //  File.Delete(_mergeOp.MergePath(item.Path));
          //}
          //else
          //{
          //  _mergeOp.GetLocal(item.Path).CopyTo(File.OpenWrite(_mergeOp.MergePath(item.Path)));
          //}
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
          //if (item.InRemote == FileStatus.DoesntExist)
          //{
          //  File.Delete(_mergeOp.MergePath(item.Path));
          //}
          //else
          //{
          //  _mergeOp.GetRemote(item.Path).CopyTo(File.OpenWrite(_mergeOp.MergePath(item.Path)));
          //}
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
  }
}
