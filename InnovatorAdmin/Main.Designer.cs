namespace Aras.Tools.InnovatorAdmin
{
  partial class Main
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
      this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
      this.btnNext = new System.Windows.Forms.Button();
      this.btnPrevious = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.lblMessage = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.lblLine = new System.Windows.Forms.Label();
      this.lblLine2 = new System.Windows.Forms.Label();
      this.tblLayout.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // tblLayout
      // 
      this.tblLayout.BackColor = System.Drawing.Color.White;
      this.tblLayout.ColumnCount = 4;
      this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
      this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblLayout.Controls.Add(this.btnNext, 2, 5);
      this.tblLayout.Controls.Add(this.btnPrevious, 1, 5);
      this.tblLayout.Controls.Add(this.btnClose, 3, 5);
      this.tblLayout.Controls.Add(this.lblMessage, 1, 1);
      this.tblLayout.Controls.Add(this.pictureBox1, 0, 0);
      this.tblLayout.Controls.Add(this.lblLine, 0, 2);
      this.tblLayout.Controls.Add(this.lblLine2, 0, 4);
      this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tblLayout.Location = new System.Drawing.Point(0, 0);
      this.tblLayout.Name = "tblLayout";
      this.tblLayout.RowCount = 6;
      this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblLayout.Size = new System.Drawing.Size(599, 449);
      this.tblLayout.TabIndex = 1;
      // 
      // btnNext
      // 
      this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNext.Location = new System.Drawing.Point(440, 423);
      this.btnNext.Name = "btnNext";
      this.btnNext.Size = new System.Drawing.Size(75, 23);
      this.btnNext.TabIndex = 0;
      this.btnNext.Text = "&Next";
      this.btnNext.UseVisualStyleBackColor = true;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // btnPrevious
      // 
      this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrevious.Location = new System.Drawing.Point(359, 423);
      this.btnPrevious.Name = "btnPrevious";
      this.btnPrevious.Size = new System.Drawing.Size(75, 23);
      this.btnPrevious.TabIndex = 1;
      this.btnPrevious.Text = "&Previous";
      this.btnPrevious.UseVisualStyleBackColor = true;
      this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(521, 423);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "&Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // lblMessage
      // 
      this.lblMessage.AutoSize = true;
      this.tblLayout.SetColumnSpan(this.lblMessage, 3);
      this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMessage.Location = new System.Drawing.Point(51, 52);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 15);
      this.lblMessage.TabIndex = 4;
      // 
      // pictureBox1
      // 
      this.tblLayout.SetColumnSpan(this.pictureBox1, 4);
      this.pictureBox1.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.Header;
      this.pictureBox1.Location = new System.Drawing.Point(8, 12);
      this.pictureBox1.Margin = new System.Windows.Forms.Padding(8, 12, 3, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(348, 34);
      this.pictureBox1.TabIndex = 5;
      this.pictureBox1.TabStop = false;
      // 
      // lblLine
      // 
      this.lblLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.tblLayout.SetColumnSpan(this.lblLine, 4);
      this.lblLine.Location = new System.Drawing.Point(20, 80);
      this.lblLine.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
      this.lblLine.Name = "lblLine";
      this.lblLine.Size = new System.Drawing.Size(559, 1);
      this.lblLine.TabIndex = 2;
      // 
      // lblLine2
      // 
      this.lblLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblLine2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.tblLayout.SetColumnSpan(this.lblLine2, 4);
      this.lblLine2.Location = new System.Drawing.Point(20, 416);
      this.lblLine2.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
      this.lblLine2.Name = "lblLine2";
      this.lblLine2.Size = new System.Drawing.Size(559, 1);
      this.lblLine2.TabIndex = 0;
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(599, 449);
      this.Controls.Add(this.tblLayout);
      this.Name = "Main";
      this.Text = "Innovator Installer";
      this.tblLayout.ResumeLayout(false);
      this.tblLayout.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tblLayout;
    private System.Windows.Forms.Button btnNext;
    private System.Windows.Forms.Button btnPrevious;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label lblLine;
    private System.Windows.Forms.Label lblLine2;

  }
}

