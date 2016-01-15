namespace InnovatorAdmin.Editor
{
  partial class FilePathControl
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
      this.txtPath = new InnovatorAdmin.Editor.EditorWinForm();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnBrowse = new InnovatorAdmin.Controls.FlatButton();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtPath
      // 
      this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtPath.Location = new System.Drawing.Point(0, 0);
      this.txtPath.Margin = new System.Windows.Forms.Padding(0);
      this.txtPath.Name = "txtPath";
      this.txtPath.PlaceholderText = "";
      this.txtPath.ReadOnly = false;
      this.txtPath.ShowLineNumbers = false;
      this.txtPath.ShowScrollbars = false;
      this.txtPath.SingleLine = true;
      this.txtPath.Size = new System.Drawing.Size(212, 22);
      this.txtPath.TabIndex = 0;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.btnBrowse, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtPath, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(241, 22);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // btnBrowse
      // 
      this.btnBrowse.AutoSize = true;
      this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnBrowse.FlatAppearance.BorderSize = 0;
      this.btnBrowse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnBrowse.ForeColor = System.Drawing.Color.Black;
      this.btnBrowse.Location = new System.Drawing.Point(212, 0);
      this.btnBrowse.Margin = new System.Windows.Forms.Padding(0);
      this.btnBrowse.Name = "btnBrowse";
      this.btnBrowse.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnBrowse.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.btnBrowse.Size = new System.Drawing.Size(29, 22);
      this.btnBrowse.TabIndex = 1;
      this.btnBrowse.Text = "...";
      this.btnBrowse.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnBrowse.UseVisualStyleBackColor = false;
      this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
      // 
      // FilePathControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "FilePathControl";
      this.Size = new System.Drawing.Size(241, 22);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private EditorWinForm txtPath;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Controls.FlatButton btnBrowse;
  }
}
