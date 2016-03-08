using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms.Integration;
using ICSharpCode.AvalonEdit.Document;
using System.Threading.Tasks;

namespace InnovatorAdmin.Controls
{
  public partial class Compare : UserControl, IWizardStep
  {
    private IWizard _wizard;

    public InstallScript BaseInstall { get; set; }

    public Compare()
    {
      InitializeComponent();
      gridDiffs.AutoGenerateColumns = false;
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      wizard.NextLabel = "Start Over";
      wizard.NextEnabled = true;
      var diffs = new FullBindingList<InstallItemDiff>();
      diffs.AddRange(InstallItemDiff.GetDiffs(BaseInstall, _wizard.InstallScript));
      gridDiffs.DataSource = diffs;
    }

    public void GoNext()
    {
      _wizard.GoToStep(new Welcome());
    }

    private void gridDiffs_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        if (e.ColumnIndex == colCompare.DisplayIndex && e.RowIndex >= 0)
        {
          var diff = (InstallItemDiff)gridDiffs.Rows[e.RowIndex].DataBoundItem;
          if (diff.DiffType == DiffType.Different)
          {
            Settings.Current.PerformDiff("Left"
              , s => ToAml(s, diff.LeftScript)
              , "Right"
              , s => ToAml(s, diff.RightScript));
          }
          else
          {
            using (var dialog = new EditorWindow())
            {
              dialog.AllowRun = false;
              dialog.Script = Utils.FormatXml(diff.LeftScript ?? diff.RightScript);
              dialog.SetConnection(_wizard.Connection, _wizard.ConnectionInfo.First().ConnectionName);
              dialog.ShowDialog(this);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    public async Task ToAml(Stream stream, XmlElement elem)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";
      settings.CheckCharacters = true;

      using (var xmlWriter = XmlWriter.Create(stream, settings))
      {
        elem.WriteTo(xmlWriter);
      }
    }

    private string WriteXml(XmlElement elem)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      using (var writer = new StringWriter())
      {
        using (var xml = XmlTextWriter.Create(writer, settings))
        {
          elem.WriteTo(xml);
        }
        return writer.ToString();
      }

    }
  }
}
