namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class Welcome
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.btnCreate = new System.Windows.Forms.Button();
      this.btnInstall = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.picAmlStudio = new System.Windows.Forms.PictureBox();
      this.toolTipManager = new System.Windows.Forms.ToolTip(this.components);
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picAmlStudio)).BeginInit();
      this.SuspendLayout();
      // 
      // btnCreate
      // 
      this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCreate.AutoSize = true;
      this.btnCreate.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.innPkg32;
      this.btnCreate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnCreate.Location = new System.Drawing.Point(103, 102);
      this.btnCreate.MinimumSize = new System.Drawing.Size(120, 40);
      this.btnCreate.Name = "btnCreate";
      this.btnCreate.Size = new System.Drawing.Size(259, 40);
      this.btnCreate.TabIndex = 3;
      this.btnCreate.Text = "Create Package";
      this.toolTipManager.SetToolTip(this.btnCreate, "Create a new package for export.");
      this.btnCreate.UseVisualStyleBackColor = true;
      this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
      // 
      // btnInstall
      // 
      this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnInstall.AutoSize = true;
      this.btnInstall.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.install32;
      this.btnInstall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnInstall.Location = new System.Drawing.Point(103, 36);
      this.btnInstall.MinimumSize = new System.Drawing.Size(120, 40);
      this.btnInstall.Name = "btnInstall";
      this.btnInstall.Size = new System.Drawing.Size(259, 40);
      this.btnInstall.TabIndex = 2;
      this.btnInstall.Text = "Install Package";
      this.toolTipManager.SetToolTip(this.btnInstall, "Install (import) solution from existing package.");
      this.btnInstall.UseVisualStyleBackColor = true;
      this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.Controls.Add(this.btnCreate, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnInstall, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.picAmlStudio, 1, 5);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 6;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(465, 219);
      this.tableLayoutPanel1.TabIndex = 4;
      // 
      // picAmlStudio
      // 
      this.picAmlStudio.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.amlStudio32;
      this.picAmlStudio.Location = new System.Drawing.Point(103, 181);
      this.picAmlStudio.Name = "picAmlStudio";
      this.picAmlStudio.Size = new System.Drawing.Size(35, 34);
      this.picAmlStudio.TabIndex = 4;
      this.picAmlStudio.TabStop = false;
      this.toolTipManager.SetToolTip(this.picAmlStudio, "Launch AML Studio");
      this.picAmlStudio.Click += new System.EventHandler(this.picAmlStudio_Click);
      // 
      // Welcome
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "Welcome";
      this.Size = new System.Drawing.Size(465, 219);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picAmlStudio)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnCreate;
    private System.Windows.Forms.Button btnInstall;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.PictureBox picAmlStudio;
    private System.Windows.Forms.ToolTip toolTipManager;
  }
}
