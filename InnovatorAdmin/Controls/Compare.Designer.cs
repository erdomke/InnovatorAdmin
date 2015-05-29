namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class Compare
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
      this.gridDiffs = new System.Windows.Forms.DataGridView();
      this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colLeftExists = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.colRightExists = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.colDiffType = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colCompare = new System.Windows.Forms.DataGridViewButtonColumn();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridDiffs)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.gridDiffs, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 79F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 350);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // gridDiffs
      // 
      this.gridDiffs.AllowUserToAddRows = false;
      this.gridDiffs.AllowUserToDeleteRows = false;
      this.gridDiffs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gridDiffs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridDiffs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colLeftExists,
            this.colRightExists,
            this.colDiffType,
            this.colCompare});
      this.tableLayoutPanel1.SetColumnSpan(this.gridDiffs, 2);
      this.gridDiffs.Location = new System.Drawing.Point(3, 3);
      this.gridDiffs.Name = "gridDiffs";
      this.gridDiffs.ReadOnly = true;
      this.gridDiffs.Size = new System.Drawing.Size(583, 323);
      this.gridDiffs.TabIndex = 0;
      this.gridDiffs.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDiffs_CellClick);
      // 
      // colName
      // 
      this.colName.DataPropertyName = "Name";
      this.colName.HeaderText = "Name";
      this.colName.Name = "colName";
      this.colName.ReadOnly = true;
      this.colName.Width = 350;
      // 
      // colLeftExists
      // 
      this.colLeftExists.DataPropertyName = "LeftExists";
      this.colLeftExists.HeaderText = "Left Exists?";
      this.colLeftExists.Name = "colLeftExists";
      this.colLeftExists.ReadOnly = true;
      this.colLeftExists.Width = 60;
      // 
      // colRightExists
      // 
      this.colRightExists.DataPropertyName = "RightExists";
      this.colRightExists.HeaderText = "Right Exists?";
      this.colRightExists.Name = "colRightExists";
      this.colRightExists.ReadOnly = true;
      this.colRightExists.Width = 60;
      // 
      // colDiffType
      // 
      this.colDiffType.DataPropertyName = "DiffType";
      this.colDiffType.HeaderText = "Diff Type";
      this.colDiffType.Name = "colDiffType";
      this.colDiffType.ReadOnly = true;
      this.colDiffType.Width = 80;
      // 
      // colCompare
      // 
      this.colCompare.HeaderText = "";
      this.colCompare.Name = "colCompare";
      this.colCompare.ReadOnly = true;
      this.colCompare.Text = "View";
      this.colCompare.UseColumnTextForButtonValue = true;
      this.colCompare.Width = 50;
      // 
      // Compare
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "Compare";
      this.Size = new System.Drawing.Size(589, 350);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.gridDiffs)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.DataGridView gridDiffs;
    private System.Windows.Forms.DataGridViewTextBoxColumn colName;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colLeftExists;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colRightExists;
    private System.Windows.Forms.DataGridViewTextBoxColumn colDiffType;
    private System.Windows.Forms.DataGridViewButtonColumn colCompare;
  }
}
