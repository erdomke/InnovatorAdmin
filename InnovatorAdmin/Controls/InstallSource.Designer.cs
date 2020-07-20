namespace InnovatorAdmin.Controls
{
  partial class InstallSource
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnInnovatorPackage = new InnovatorAdmin.Controls.FlatButton();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.chkAddPackage = new System.Windows.Forms.CheckBox();
      this.lblName = new System.Windows.Forms.Label();
      this.lblAuthor = new System.Windows.Forms.Label();
      this.lblWebsite = new System.Windows.Forms.LinkLabel();
      this.txtDescription = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.Controls.Add(this.btnInnovatorPackage, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.chkAddPackage, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this.lblName, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.lblAuthor, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.lblWebsite, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.txtDescription, 1, 4);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 7;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(558, 348);
      this.tableLayoutPanel1.TabIndex = 5;
      // 
      // btnInnovatorPackage
      // 
      this.btnInnovatorPackage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnInnovatorPackage.AutoSize = true;
      this.btnInnovatorPackage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnInnovatorPackage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnInnovatorPackage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnInnovatorPackage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnInnovatorPackage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnInnovatorPackage.ForeColor = System.Drawing.Color.Black;
      this.btnInnovatorPackage.Image = global::InnovatorAdmin.Properties.Resources.innPkg32;
      this.btnInnovatorPackage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnInnovatorPackage.Location = new System.Drawing.Point(69, 3);
      this.btnInnovatorPackage.MinimumSize = new System.Drawing.Size(120, 40);
      this.btnInnovatorPackage.Name = "btnInnovatorPackage";
      this.btnInnovatorPackage.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnInnovatorPackage.Padding = new System.Windows.Forms.Padding(2);
      this.btnInnovatorPackage.Size = new System.Drawing.Size(386, 40);
      this.btnInnovatorPackage.TabIndex = 3;
      this.btnInnovatorPackage.Text = "Select Package";
      this.btnInnovatorPackage.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnInnovatorPackage.UseVisualStyleBackColor = true;
      this.btnInnovatorPackage.Click += new System.EventHandler(this.btnInnovatorPackage_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(28, 49);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Name";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(25, 68);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(38, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Author";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(17, 87);
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
      this.label4.Location = new System.Drawing.Point(3, 106);
      this.label4.Margin = new System.Windows.Forms.Padding(3);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(60, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Description";
      // 
      // chkAddPackage
      // 
      this.chkAddPackage.AutoSize = true;
      this.chkAddPackage.Location = new System.Drawing.Point(69, 174);
      this.chkAddPackage.Name = "chkAddPackage";
      this.chkAddPackage.Size = new System.Drawing.Size(152, 17);
      this.chkAddPackage.TabIndex = 9;
      this.chkAddPackage.Text = "Add Package to Database";
      this.chkAddPackage.UseVisualStyleBackColor = true;
      // 
      // lblName
      // 
      this.lblName.AutoSize = true;
      this.lblName.Location = new System.Drawing.Point(69, 49);
      this.lblName.Margin = new System.Windows.Forms.Padding(3);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(0, 13);
      this.lblName.TabIndex = 10;
      // 
      // lblAuthor
      // 
      this.lblAuthor.AutoSize = true;
      this.lblAuthor.Location = new System.Drawing.Point(69, 68);
      this.lblAuthor.Margin = new System.Windows.Forms.Padding(3);
      this.lblAuthor.Name = "lblAuthor";
      this.lblAuthor.Size = new System.Drawing.Size(0, 13);
      this.lblAuthor.TabIndex = 11;
      // 
      // lblWebsite
      // 
      this.lblWebsite.AutoSize = true;
      this.lblWebsite.Location = new System.Drawing.Point(69, 87);
      this.lblWebsite.Margin = new System.Windows.Forms.Padding(3);
      this.lblWebsite.Name = "lblWebsite";
      this.lblWebsite.Size = new System.Drawing.Size(0, 13);
      this.lblWebsite.TabIndex = 12;
      this.lblWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblWebsite_LinkClicked);
      // 
      // txtDescription
      // 
      this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtDescription.BackColor = System.Drawing.Color.White;
      this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtDescription.Location = new System.Drawing.Point(69, 106);
      this.txtDescription.Multiline = true;
      this.txtDescription.Name = "txtDescription";
      this.txtDescription.ReadOnly = true;
      this.txtDescription.Size = new System.Drawing.Size(386, 62);
      this.txtDescription.TabIndex = 13;
      // 
      // InstallSource
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "InstallSource";
      this.Size = new System.Drawing.Size(558, 348);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private FlatButton btnInnovatorPackage;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox chkAddPackage;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.Label lblAuthor;
    private System.Windows.Forms.LinkLabel lblWebsite;
    private System.Windows.Forms.TextBox txtDescription;
  }
}
