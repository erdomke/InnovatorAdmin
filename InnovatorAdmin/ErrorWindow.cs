using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class ErrorWindow : Form
  {
    public ErrorWindow()
    {
      InitializeComponent();

      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.TopMost = false;
    }

    public static void HandleEvent(RecoverableErrorEventArgs args)
    {
      using (var dialog = new ErrorWindow())
      {
        dialog.txtMessage.Text = args.Message ?? args.Exception.Message;
        dialog.txtErrorDetails.Text = FormatXml(args.Exception.ErrorNode);
        dialog.txtQuery.Text = FormatXml(args.Exception.QueryNode);
        dialog.TopMost = true;
        switch (dialog.ShowDialog())
        {
          case DialogResult.Ignore:
            args.RecoveryOption = RecoveryOption.Skip;
            break;
          case DialogResult.Retry:
            args.RecoveryOption = RecoveryOption.Retry;
            var doc = new XmlDocument();
            doc.LoadXml(dialog.txtQuery.Text);
            args.NewQuery = doc.DocumentElement;
            break;
          default:
            args.RecoveryOption = RecoveryOption.Abort;
            break;
        }
      }
    }

    private static string FormatXml(XmlNode node)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      using (var output = new StringWriter())
      {
        using (var writer = XmlTextWriter.Create(output, settings))
        {
          node.WriteTo(writer);
        }
        return output.ToString();
      }
    }

    private void btnShowDetails_Click(object sender, EventArgs e)
    {
      try
      {
        tbcMain.Visible = !tbcMain.Visible;
        if (tbcMain.Visible)
        {
          this.Height = 400;
        }
        else
        {
          this.Height = 155;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
      try
      {
        txtQuery.Enabled = true;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
