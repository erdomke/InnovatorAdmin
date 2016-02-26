namespace InnovatorAdmin
{
  partial class ProgressDialog
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.progBar = new System.Windows.Forms.ProgressBar();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnCancel = new InnovatorAdmin.Controls.FlatButton();
      this.label1 = new System.Windows.Forms.Label();
      this.lblTime = new System.Windows.Forms.Label();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // progressBar1
      // 
      this.progBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.progBar, 2);
      this.progBar.Location = new System.Drawing.Point(38, 41);
      this.progBar.Name = "progressBar1";
      this.progBar.Size = new System.Drawing.Size(279, 19);
      this.progBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.progBar.TabIndex = 0;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.Controls.Add(this.progBar, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.lblTime, 1, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(355, 101);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.AutoSize = true;
      this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCancel.FlatAppearance.BorderSize = 0;
      this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCancel.ForeColor = System.Drawing.Color.Black;
      this.btnCancel.Location = new System.Drawing.Point(263, 66);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Padding = new System.Windows.Forms.Padding(2);
      this.btnCancel.Size = new System.Drawing.Size(54, 27);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
      this.label1.Location = new System.Drawing.Point(38, 22);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(68, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Processing...";
      // 
      // lblTime
      // 
      this.lblTime.AutoSize = true;
      this.lblTime.Location = new System.Drawing.Point(38, 66);
      this.lblTime.Margin = new System.Windows.Forms.Padding(3);
      this.lblTime.Name = "lblTime";
      this.lblTime.Size = new System.Drawing.Size(0, 13);
      this.lblTime.TabIndex = 3;
      // 
      // ProgressDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(355, 101);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "ProgressDialog";
      this.Text = "ProgressDialog";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ProgressBar progBar;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Controls.FlatButton btnCancel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lblTime;
  }
}
