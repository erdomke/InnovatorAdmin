namespace InnovatorAdmin.Controls
{
  partial class ParameterGrid
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
      this.tblMain = new System.Windows.Forms.TableLayoutPanel();
      this.SuspendLayout();
      // 
      // tblMain
      // 
      this.tblMain.AutoSize = true;
      this.tblMain.ColumnCount = 3;
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblMain.Dock = System.Windows.Forms.DockStyle.Top;
      this.tblMain.Location = new System.Drawing.Point(0, 0);
      this.tblMain.Name = "tblMain";
      this.tblMain.RowCount = 1;
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblMain.Size = new System.Drawing.Size(314, 0);
      this.tblMain.TabIndex = 0;
      // 
      // ParameterGrid
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.AutoSize = true;
      this.BackColor = System.Drawing.Color.White;
      this.Controls.Add(this.tblMain);
      this.Name = "ParameterGrid";
      this.Size = new System.Drawing.Size(314, 10);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tblMain;
  }
}
