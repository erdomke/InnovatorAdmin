using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Dialog
{
  public class ImportSelectDialog : FileDialogControlBase
  {
#region "Controls"
    private System.Windows.Forms.Button btnSelectFolder;
#endregion

    private const string FolderPlaceHolder = "__F3102E010CAE4AA4B937CABCEC5BF2FC";
    private Button btnUseAutoSave;

    private bool _checkFileExists = true;

    public override bool CheckFileExists
    {
      get { return _checkFileExists; }
      set { _checkFileExists = value; }
    }
    public override string FileName
    {
      get { return (base.FileName ?? "").EndsWith(FolderPlaceHolder) 
        ? base.FileName.Substring(0, base.FileName.Length - FolderPlaceHolder.Length) 
        : base.FileName; }
      set
      {
        base.FileName = value;
      }
    }
    public override string[] FileNames
    {
      get
      {
        var path = base.FileNames == null ? null : base.FileNames.FirstOrDefault(n => n.EndsWith(FolderPlaceHolder));
        if (path == null)
        {
          return base.FileNames; 
        }
        else
        {
          return new string[] { path.Substring(0, path.Length - FolderPlaceHolder.Length) };
        }
      }
    }

    public ImportSelectDialog()
    {
      this.InitializeComponent();
      base.CheckFileExists = false;
      btnUseAutoSave.Enabled = File.Exists(Utils.GetAppFilePath(AppFileType.ImportExtractor));
    }

    protected override void OnClosingDialog(System.ComponentModel.CancelEventArgs e)
    {
      if (_checkFileExists && Path.GetFileNameWithoutExtension(base.FileName) != FolderPlaceHolder && !File.Exists(base.FileName))
      {
        MessageBox.Show(String.Format("The file '{1}' does not exist.{0}Please select a file that exists.", Environment.NewLine, base.FileName));
        e.Cancel = true;
        return;
      }
      base.OnClosingDialog(e);
    }

    private void InitializeComponent()
    {
      this.btnSelectFolder = new System.Windows.Forms.Button();
      this.btnUseAutoSave = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnSelectFolder
      // 
      this.btnSelectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelectFolder.AutoSize = true;
      this.btnSelectFolder.Location = new System.Drawing.Point(460, 3);
      this.btnSelectFolder.MinimumSize = new System.Drawing.Size(90, 0);
      this.btnSelectFolder.Name = "btnSelectFolder";
      this.btnSelectFolder.Size = new System.Drawing.Size(90, 23);
      this.btnSelectFolder.TabIndex = 0;
      this.btnSelectFolder.Text = "&Select Folder";
      this.btnSelectFolder.UseVisualStyleBackColor = true;
      this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
      // 
      // btnUseAutoSave
      // 
      this.btnUseAutoSave.AutoSize = true;
      this.btnUseAutoSave.Location = new System.Drawing.Point(3, 3);
      this.btnUseAutoSave.MinimumSize = new System.Drawing.Size(90, 0);
      this.btnUseAutoSave.Name = "btnUseAutoSave";
      this.btnUseAutoSave.Size = new System.Drawing.Size(155, 23);
      this.btnUseAutoSave.TabIndex = 1;
      this.btnUseAutoSave.Text = "&Load Last Auto-Saved Import";
      this.btnUseAutoSave.UseVisualStyleBackColor = true;
      this.btnUseAutoSave.Click += new System.EventHandler(this.btnUseAutoSave_Click);
      // 
      // ImportSelectDialog
      // 
      this.AddExtension = false;
      this.Caption = "Select files / folder";
      this.CheckFileExists = false;
      this.Controls.Add(this.btnUseAutoSave);
      this.Controls.Add(this.btnSelectFolder);
      this.DefaultExtension = "";
      this.Name = "ImportSelectDialog";
      this.Size = new System.Drawing.Size(553, 32);
      this.StartLocation = InnovatorAdmin.Dialog.AddonWindowLocation.Bottom;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private void btnSelectFolder_Click(object sender, EventArgs e)
    {
      base.FileName = FolderPlaceHolder;
      AcceptDialog();
    }

    private void btnUseAutoSave_Click(object sender, EventArgs e)
    {
      base.FileName = Utils.GetAppFilePath(AppFileType.ImportExtractor);
      AcceptDialog();
    }
  }
}
