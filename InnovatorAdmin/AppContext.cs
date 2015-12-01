using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aras.Tools.InnovatorAdmin
{
  class AppContext : ApplicationContext
  {
    Task<ReleaseEntry> _updates = null;
    Task<UpdateManager> _mgr = null;

    public AppContext(Form mainForm) : base(mainForm)
    {
      this.MainForm.Shown += MainForm_Shown;
    }

    void MainForm_Shown(object sender, EventArgs e)
    {
      this.MainForm.Shown -= MainForm_Shown;

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

#if !DEBUG
      _mgr = UpdateManager.GitHubUpdateManager("https://github.com/erdomke/InnovatorAdmin");
      _mgr.ContinueWith(t => _updates = t.Result.UpdateApp());
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
        #if !DEBUG
        if (_updates != null) _updates.Wait();
        #endif
        SnippetManager.Instance.Close();
        base.OnMainFormClosed(sender, e);
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        if (_updates != null) _updates.Dispose();
        if (_mgr != null) _mgr.Dispose();
      }
    }
  }
}
