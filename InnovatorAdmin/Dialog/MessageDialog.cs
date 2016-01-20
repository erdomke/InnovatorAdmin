using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Dialog
{
  public partial class MessageDialog : FormBase
  {
    public string Caption
    {
      get { return this.Text; }
      set { this.Text = string.IsNullOrEmpty(value) ? Assembly.GetEntryAssembly().GetName().Name : value; }
    }
    public Color CaptionColor
    {
      get { return lblTitle.ForeColor; }
      set
      {
        lblTitle.ForeColor = value;
        this.ActiveTextColor = value;
        this.HoverTextColor = value;
      }
    }
    public string Message
    {
      get { return txtMessage.Text; }
      set { txtMessage.Text = value; }
    }
    public string Details
    {
      get { return txtDetails.Text; }
      set
      {
        if (tblMain.Controls.Contains(btnDetails) == string.IsNullOrEmpty(value))
        {
          if (string.IsNullOrEmpty(value))
          {
            tblMain.Controls.Remove(btnDetails);
            tblMain.RowStyles[4].SizeType = SizeType.Absolute;
            tblMain.RowStyles[4].Height = 0;
          }
          else
          {
            tblMain.Controls.Add(btnDetails, 1, 4);
            tblMain.RowStyles[4].SizeType = SizeType.AutoSize;
          }
        }
        txtDetails.Text = value;
      }
    }
    public string OkText
    {
      get { return btnOK.Text; }
      set { btnOK.Text = string.IsNullOrEmpty(value) ? "&OK" : value; }
    }
    public string NoText
    {
      get { return btnNo.Text; }
      set
      {
        if (tblMain.Controls.Contains(btnNo) == string.IsNullOrEmpty(value))
        {
          if (string.IsNullOrEmpty(value))
          {
            tblMain.Controls.Remove(btnNo);
            tblMain.ColumnStyles[2].SizeType = SizeType.Absolute;
            tblMain.ColumnStyles[2].Width = 0;
          }
          else
          {
            tblMain.Controls.Add(btnNo, 2, 6);
            tblMain.ColumnStyles[2].SizeType = SizeType.AutoSize;
          }
        }
        btnNo.Text = value ?? string.Empty;
      }
    }
    public string CancelText
    {
      get { return btnCancel.Text; }
      set
      {
        if (tblMain.Controls.Contains(btnCancel) == string.IsNullOrEmpty(value))
        {
          var newVisible = !string.IsNullOrEmpty(value);
          this.CancelButton = newVisible ? btnOK : btnCancel;
          if (newVisible)
          {
            tblMain.Controls.Add(btnCancel, 3, 6);
            tblMain.ColumnStyles[3].SizeType = SizeType.AutoSize;
          }
          else
          {
            tblMain.Controls.Remove(btnCancel);
            tblMain.ColumnStyles[3].SizeType = SizeType.Absolute;
            tblMain.ColumnStyles[3].Width = 0;
          }
        }
        btnCancel.Text = value ?? string.Empty;
      }
    }

    public MessageDialog()
    {
      InitializeComponent();

      this.TitleLabel = lblTitle;
      this.TopLeftCornerPanel = pnlTopLeft;
      this.TopBorderPanel = pnlTop;
      this.TopRightCornerPanel = pnlTopRight;
      this.LeftBorderPanel = pnlLeft;
      this.RightBorderPanel = pnlRight;
      this.BottomRightCornerPanel = pnlBottomRight;
      this.BottomBorderPanel = pnlBottom;
      this.BottomLeftCornerPanel = pnlBottomLeft;
      this.InitializeTheme();

      this.KeyPreview = true;
      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;

      this.Details = null;
      this.NoText = null;
      this.CancelText = null;
      ToggleDetailVisibility();
    }

    private void btnDetails_Click(object sender, EventArgs e)
    {
      ToggleDetailVisibility();
    }
    private void ToggleDetailVisibility()
    {
      if (tblMain.Controls.Contains(txtDetails))
      {
        this.Height -= 120;
        tblMain.Controls.Remove(txtDetails);
        tblMain.RowStyles[5].SizeType = SizeType.Absolute;
        tblMain.RowStyles[5].Height = 0;
      }
      else
      {
        tblMain.Controls.Add(txtDetails, 1, 5);
        tblMain.RowStyles[5].SizeType = SizeType.Percent;
        tblMain.RowStyles[5].Height = 66.67f;
        this.Height += 120;
      }
    }

    public static DialogResult Show(string message)
    {
      return Show(null, message, null, null, null, null);
    }
    public static DialogResult Show(string message, string caption)
    {
      return Show(null, message, caption, null, null, null);
    }
    public static DialogResult Show(string message, string caption, string okText)
    {
      return Show(null, message, caption, okText, null, null);
    }
    public static DialogResult Show(string message, string caption, string okText, string cancelText)
    {
      return Show(null, message, caption, okText, cancelText, null);
    }
    public static DialogResult Show(IWin32Window window, string message, string caption, string okText, string noText, string cancelText)
    {
      using (var dialog = new MessageDialog())
      {
        dialog.Message = message;
        dialog.Caption = caption;
        dialog.OkText = okText;
        dialog.NoText = noText;
        dialog.CancelText = cancelText;
        return dialog.ShowDialog(window);
      }
    }
  }
}
