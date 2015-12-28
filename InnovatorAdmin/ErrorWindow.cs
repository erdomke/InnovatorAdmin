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

namespace InnovatorAdmin
{
  public partial class ErrorWindow : Form
  {
    public ErrorWindow()
    {
      InitializeComponent();

      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;
      this.txtErrorDetails.Helper = new Editor.AmlSimpleEditorHelper();
      this.txtQuery.Helper = new Editor.AmlSimpleEditorHelper();
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
        dialog.txtErrorDetails.Text = Utils.IndentXml(args.Exception.AsAmlString());
        dialog.txtQuery.Text = Utils.IndentXml(args.Exception.Query);
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
