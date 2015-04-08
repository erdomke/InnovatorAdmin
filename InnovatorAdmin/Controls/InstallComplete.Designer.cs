namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class InstallComplete
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
      this.label1 = new System.Windows.Forms.Label();
      this.btnResetServerCache = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnResetServerCache, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(781, 420);
      this.tableLayoutPanel1.TabIndex = 5;
      // 
      // label1
      // 
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.check32;
      this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.label1.Location = new System.Drawing.Point(103, 184);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(251, 52);
      this.label1.TabIndex = 0;
      this.label1.Text = "Installation Complete!";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // btnResetServerCache
      // 
      this.btnResetServerCache.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnResetServerCache.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.btnResetServerCache, 2);
      this.btnResetServerCache.Location = new System.Drawing.Point(6, 387);
      this.btnResetServerCache.Margin = new System.Windows.Forms.Padding(6);
      this.btnResetServerCache.Name = "btnResetServerCache";
      this.btnResetServerCache.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
      this.btnResetServerCache.Size = new System.Drawing.Size(164, 27);
      this.btnResetServerCache.TabIndex = 1;
      this.btnResetServerCache.Text = "&Reset Additional App Servers...";
      this.btnResetServerCache.UseVisualStyleBackColor = true;
      this.btnResetServerCache.Click += new System.EventHandler(this.btnResetServerCache_Click);
      // 
      // InstallComplete
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "InstallComplete";
      this.Size = new System.Drawing.Size(781, 420);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnResetServerCache;
  }
}
