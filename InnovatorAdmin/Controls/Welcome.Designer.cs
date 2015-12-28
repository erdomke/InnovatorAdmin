namespace InnovatorAdmin.Controls
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnInstall = new InnovatorAdmin.Controls.FlatButton();
      this.btnAmlStudio = new InnovatorAdmin.Controls.FlatButton();
      this.btnCreate = new InnovatorAdmin.Controls.FlatButton();
      this.btnScripts = new InnovatorAdmin.Controls.FlatButton();
      this.toolTipManager = new System.Windows.Forms.ToolTip(this.components);
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 5;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
      this.tableLayoutPanel1.Controls.Add(this.btnInstall, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnAmlStudio, 3, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnCreate, 3, 1);
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
      this.btnInstall.Image = global::InnovatorAdmin.Properties.Resources.install32;
      this.btnInstall.Location = new System.Drawing.Point(103, 35);
      this.btnInstall.MinimumSize = new System.Drawing.Size(120, 40);
      this.btnInstall.Name = "btnInstall";
      this.btnInstall.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnInstall.Padding = new System.Windows.Forms.Padding(2);
      this.btnInstall.Size = new System.Drawing.Size(120, 61);
      this.btnInstall.TabIndex = 2;
      this.btnInstall.Text = "Install Package";
      this.btnInstall.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.btnInstall.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.toolTipManager.SetToolTip(this.btnInstall, "Install (import) solution from existing package.");
      this.btnInstall.UseVisualStyleBackColor = true;
      this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // btnAmlStudio
      // 
      this.btnAmlStudio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAmlStudio.AutoSize = true;
      this.btnAmlStudio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnAmlStudio.FlatAppearance.BorderSize = 0;
      this.btnAmlStudio.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnAmlStudio.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnAmlStudio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnAmlStudio.ForeColor = System.Drawing.Color.Black;
      this.btnAmlStudio.Image = global::InnovatorAdmin.Properties.Resources.amlStudio32black;
      this.btnAmlStudio.Location = new System.Drawing.Point(245, 122);
      this.btnAmlStudio.Name = "btnAmlStudio";
      this.btnAmlStudio.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnAmlStudio.Padding = new System.Windows.Forms.Padding(2);
      this.btnAmlStudio.Size = new System.Drawing.Size(116, 61);
      this.btnAmlStudio.TabIndex = 5;
      this.btnAmlStudio.Text = "AML Studio";
      this.btnAmlStudio.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.btnAmlStudio.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnAmlStudio.UseVisualStyleBackColor = false;
      this.btnAmlStudio.Click += new System.EventHandler(this.btnAmlStudio_Click);
      // 
      // btnCreate
      // 
      this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCreate.AutoSize = true;
      this.btnCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCreate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCreate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCreate.ForeColor = System.Drawing.Color.Black;
      this.btnCreate.Image = global::InnovatorAdmin.Properties.Resources.innPkg32;
      this.btnCreate.Location = new System.Drawing.Point(245, 35);
      this.btnCreate.MinimumSize = new System.Drawing.Size(120, 40);
      this.btnCreate.Name = "btnCreate";
      this.btnCreate.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnCreate.Padding = new System.Windows.Forms.Padding(2);
      this.btnCreate.Size = new System.Drawing.Size(120, 61);
      this.btnCreate.TabIndex = 3;
      this.btnCreate.Text = "Create Package";
      this.btnCreate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.btnCreate.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.toolTipManager.SetToolTip(this.btnCreate, "Create a new package for export.");
      this.btnCreate.UseVisualStyleBackColor = true;
      this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
      // 
      // btnScripts
      // 
      this.btnScripts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnScripts.AutoSize = true;
      this.btnScripts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnScripts.FlatAppearance.BorderSize = 0;
      this.btnScripts.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnScripts.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnScripts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnScripts.ForeColor = System.Drawing.Color.Black;
      this.btnScripts.Location = new System.Drawing.Point(103, 122);
      this.btnScripts.Name = "btnScripts";
      this.btnScripts.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnScripts.Padding = new System.Windows.Forms.Padding(2);
      this.btnScripts.Size = new System.Drawing.Size(116, 61);
      this.btnScripts.TabIndex = 6;
      this.btnScripts.Text = "Scripts";
      this.btnScripts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.btnScripts.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnScripts.UseVisualStyleBackColor = false;
      this.btnScripts.Click += new System.EventHandler(this.btnScripts_Click);
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
      this.ResumeLayout(false);

    }

    #endregion

    private FlatButton btnCreate;
    private FlatButton btnInstall;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.ToolTip toolTipManager;
    private FlatButton btnAmlStudio;
    private FlatButton btnScripts;
  }
}
