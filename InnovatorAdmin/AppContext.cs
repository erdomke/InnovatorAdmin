using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using Squirrel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  class AppContext : ApplicationContext
  {
    Task<ReleaseEntry> _updates = null;
    Task<UpdateManager> _mgr = null;

    public AppContext(Form mainForm) : base(mainForm)
    {
      GenerateBatFile();
      this.MainForm.Shown += MainForm_Shown;
    }

    void MainForm_Shown(object sender, EventArgs e)
    {
      this.MainForm.Shown -= MainForm_Shown;
      UpdateJumpList();
#if !DEBUG
      _mgr = UpdateManager.GitHubUpdateManager("https://github.com/erdomke/InnovatorAdmin");
      _mgr.ContinueWith(t => {
        var listener = this.MainForm as IUpdateListener;
        _updates = t.Result.UpdateApp(listener == null ? (Action<int>)null : listener.UpdateCheckProgress);
        _updates.ContinueWith(r =>
        {
          if (listener != null)
            listener.UpdateCheckComplete(r.Result == default(ReleaseEntry) ? default(Version) : r.Result.Version.Version);
        });
      });
#endif
    }

    protected override void OnMainFormClosed(object sender, EventArgs e)
    {
      if (Application.OpenForms.Count > 0)
      {
        this.MainForm = Application.OpenForms[0];
      }
      else
      {
        try
        {
          SnippetManager.Instance.Close();
          GenerateBatFile();
          base.OnMainFormClosed(sender, e);
          #if !DEBUG
            if (_updates != null) _updates.Wait();
          #endif
        }
        catch (Exception) {}  // Eat the error for now
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        try
        {
          if (_updates != null) _updates.Dispose();
          if (_mgr != null) _mgr.Dispose();
        }
        catch (Exception) { }
      }
    }

    public static void UpdateJumpList()
    {
      // Create a Jump List for AML Studio connections
      try
      {
        var list = JumpList.CreateJumpList();
        var currPath = Program.AssemblyPath;
        JumpListLink link;
        var links = new List<IJumpListItem>();

        foreach (var conn in ConnectionManager.Current.Library.Connections)
        {
          link = new JumpListLink(currPath, conn.ConnectionName);
          link.Arguments = "\"/amlstudio:" + conn.ConnectionName + "\"";
          link.IconReference = new IconReference(currPath, 1);
          links.Add(link);
        }

        var amlStudioCat = new JumpListCustomCategory("AML Studio");
        amlStudioCat.AddJumpListItems(links.ToArray());
        list.AddCustomCategories(amlStudioCat);
        list.Refresh();
      }
      catch (Exception) { }
    }

    private void GenerateBatFile()
    {
      try
      {
        var parentDir = Path.GetDirectoryName(Program.AssemblyPath);
        var fileName = Path.GetFileName(Program.AssemblyPath);
        var search = Path.GetFileName(parentDir);
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        if (search.EndsWith(version))
          search = search.Substring(0, search.Length - version.Length) + "*";

        var dir = Directory.GetParent(parentDir);
        var latestDir = dir.GetDirectories(search).OrderByDescending(n => n.Name).First();
        File.WriteAllText(Path.Combine(dir.FullName, "run.bat"),
          string.Format("start \"\" \"{0}\\{1}\" %*", latestDir.FullName, fileName));
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }
  }
}
