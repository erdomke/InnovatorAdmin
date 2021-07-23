using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  class AppContext : ApplicationContext
  {
    public AppContext(Form mainForm) : base(mainForm)
    {
      GenerateBatFile();
      this.MainForm.Shown += MainForm_Shown;
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Utils.HandleError(e.ExceptionObject as Exception);
    }

    void MainForm_Shown(object sender, EventArgs e)
    {
      this.MainForm.Shown -= MainForm_Shown;
      UpdateJumpList();
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
          GenerateBatFile();
          base.OnMainFormClosed(sender, e);
        }
        catch (Exception) {}  // Eat the error for now
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
