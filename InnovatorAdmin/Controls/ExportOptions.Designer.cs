namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class ExportOptions
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
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.txtName = new System.Windows.Forms.TextBox();
      this.txtAuthor = new System.Windows.Forms.TextBox();
      this.txtWebsite = new System.Windows.Forms.TextBox();
      this.txtDescription = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.btnPackageFile = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnDbPackage = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnInstall = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnCompare = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.toolTipManager = new System.Windows.Forms.ToolTip(this.components);
      this.tableLayoutPanel2.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.AutoSize = true;
      this.tableLayoutPanel2.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtName, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtAuthor, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtWebsite, 1, 2);
      this.tableLayoutPanel2.Controls.Add(this.txtDescription, 1, 3);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.label4, 0, 3);
      this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 4);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 5;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(692, 307);
      this.tableLayoutPanel2.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(28, 6);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name";
      // 
      // txtName
      // 
      this.txtName.Location = new System.Drawing.Point(69, 3);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(267, 20);
      this.txtName.TabIndex = 1;
      this.toolTipManager.SetToolTip(this.txtName, "Package Name");
      // 
      // txtAuthor
      // 
      this.txtAuthor.Location = new System.Drawing.Point(69, 29);
      this.txtAuthor.Name = "txtAuthor";
      this.txtAuthor.Size = new System.Drawing.Size(151, 20);
      this.txtAuthor.TabIndex = 2;
      this.toolTipManager.SetToolTip(this.txtAuthor, "Package Author");
      // 
      // txtWebsite
      // 
      this.txtWebsite.Location = new System.Drawing.Point(69, 55);
      this.txtWebsite.Name = "txtWebsite";
      this.txtWebsite.Size = new System.Drawing.Size(203, 20);
      this.txtWebsite.TabIndex = 3;
      this.toolTipManager.SetToolTip(this.txtWebsite, "Author Website");
      // 
      // txtDescription
      // 
      this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtDescription.Location = new System.Drawing.Point(69, 81);
      this.txtDescription.Multiline = true;
      this.txtDescription.Name = "txtDescription";
      this.txtDescription.Size = new System.Drawing.Size(620, 50);
      this.txtDescription.TabIndex = 4;
      this.toolTipManager.SetToolTip(this.txtDescription, "Additional package information.");
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(25, 32);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(38, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Author";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(17, 58);
      this.label3.Margin = new System.Windows.Forms.Padding(3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(46, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Website";
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 84);
      this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(60, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Description";
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.Controls.Add(this.btnPackageFile);
      this.flowLayoutPanel1.Controls.Add(this.btnDbPackage);
      this.flowLayoutPanel1.Controls.Add(this.btnInstall);
      this.flowLayoutPanel1.Controls.Add(this.btnCompare);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(249, 228);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(440, 76);
      this.flowLayoutPanel1.TabIndex = 8;
      // 
      // btnPackageFile
      // 
      this.btnPackageFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPackageFile.AutoSize = true;
      this.btnPackageFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnPackageFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPackageFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPackageFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnPackageFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPackageFile.ForeColor = System.Drawing.Color.Black;
      this.btnPackageFile.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.innPkg32;
      this.btnPackageFile.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnPackageFile.Location = new System.Drawing.Point(3, 3);
      this.btnPackageFile.Name = "btnPackageFile";
      this.btnPackageFile.Padding = new System.Windows.Forms.Padding(2);
      this.btnPackageFile.Size = new System.Drawing.Size(111, 70);
      this.btnPackageFile.TabIndex = 5;
      this.btnPackageFile.Text = "&Package File(s)";
      this.btnPackageFile.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnPackageFile.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnPackageFile.UseVisualStyleBackColor = true;
      this.btnPackageFile.Click += new System.EventHandler(this.btnPackageFile_Click);
      // 
      // btnDbPackage
      // 
      this.btnDbPackage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDbPackage.AutoSize = true;
      this.btnDbPackage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnDbPackage.Enabled = false;
      this.btnDbPackage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnDbPackage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnDbPackage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnDbPackage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnDbPackage.ForeColor = System.Drawing.Color.Black;
      this.btnDbPackage.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.packageDefinition;
      this.btnDbPackage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnDbPackage.Location = new System.Drawing.Point(120, 3);
      this.btnDbPackage.Name = "btnDbPackage";
      this.btnDbPackage.Padding = new System.Windows.Forms.Padding(2);
      this.btnDbPackage.Size = new System.Drawing.Size(136, 70);
      this.btnDbPackage.TabIndex = 6;
      this.btnDbPackage.Text = "Package &Definition (Db)";
      this.btnDbPackage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnDbPackage.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnDbPackage.UseVisualStyleBackColor = true;
      this.btnDbPackage.Click += new System.EventHandler(this.btnDbPackage_Click);
      // 
      // btnInstall
      // 
      this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnInstall.AutoSize = true;
      this.btnInstall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnInstall.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnInstall.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnInstall.ForeColor = System.Drawing.Color.Black;
      this.btnInstall.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.install32;
      this.btnInstall.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnInstall.Location = new System.Drawing.Point(262, 3);
      this.btnInstall.Name = "btnInstall";
      this.btnInstall.Padding = new System.Windows.Forms.Padding(2);
      this.btnInstall.Size = new System.Drawing.Size(88, 70);
      this.btnInstall.TabIndex = 7;
      this.btnInstall.Text = "&Install (Db)";
      this.btnInstall.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnInstall.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnInstall.UseVisualStyleBackColor = true;
      this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // btnCompare
      // 
      this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCompare.AutoSize = true;
      this.btnCompare.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCompare.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCompare.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCompare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCompare.ForeColor = System.Drawing.Color.Black;
      this.btnCompare.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnCompare.Location = new System.Drawing.Point(356, 3);
      this.btnCompare.Name = "btnCompare";
      this.btnCompare.Padding = new System.Windows.Forms.Padding(2);
      this.btnCompare.Size = new System.Drawing.Size(81, 70);
      this.btnCompare.TabIndex = 8;
      this.btnCompare.Text = "&Compare";
      this.btnCompare.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnCompare.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnCompare.UseVisualStyleBackColor = true;
      this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
      // 
      // ExportOptions
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel2);
      this.Name = "ExportOptions";
      this.Size = new System.Drawing.Size(692, 307);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.TextBox txtAuthor;
    private System.Windows.Forms.TextBox txtWebsite;
    private System.Windows.Forms.TextBox txtDescription;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private FlatButton btnPackageFile;
    private FlatButton btnDbPackage;
    private FlatButton btnInstall;
    private System.Windows.Forms.ToolTip toolTipManager;
    private FlatButton btnCompare;
  }
}
