namespace InnovatorAdmin.Controls
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.gridDiffs = new InnovatorAdmin.Controls.DataGrid();
      this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colLeftExists = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.colRightExists = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.colDiffType = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colCompare = new System.Windows.Forms.DataGridViewButtonColumn();
      this.btnPatchRightLeft = new InnovatorAdmin.Controls.FlatButton();
      this.btnPatchLeftRight = new InnovatorAdmin.Controls.FlatButton();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridDiffs)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.gridDiffs, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnPatchRightLeft, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnPatchLeftRight, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 79F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
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
      this.gridDiffs.BackgroundColor = System.Drawing.Color.White;
      this.gridDiffs.BorderStyle = System.Windows.Forms.BorderStyle.None;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.gridDiffs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.gridDiffs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridDiffs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colLeftExists,
            this.colRightExists,
            this.colDiffType,
            this.colCompare});
      this.tableLayoutPanel1.SetColumnSpan(this.gridDiffs, 2);
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.gridDiffs.DefaultCellStyle = dataGridViewCellStyle2;
      this.gridDiffs.Location = new System.Drawing.Point(3, 3);
      this.gridDiffs.Name = "gridDiffs";
      this.gridDiffs.ReadOnly = true;
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.gridDiffs.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
      this.gridDiffs.Size = new System.Drawing.Size(583, 292);
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
      // btnPatchRightLeft
      // 
      this.btnPatchRightLeft.AutoSize = true;
      this.btnPatchRightLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnPatchRightLeft.FlatAppearance.BorderSize = 0;
      this.btnPatchRightLeft.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPatchRightLeft.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPatchRightLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnPatchRightLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnPatchRightLeft.ForeColor = System.Drawing.Color.Black;
      this.btnPatchRightLeft.Location = new System.Drawing.Point(457, 301);
      this.btnPatchRightLeft.Name = "btnPatchRightLeft";
      this.btnPatchRightLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnPatchRightLeft.Padding = new System.Windows.Forms.Padding(2);
      this.btnPatchRightLeft.Size = new System.Drawing.Size(129, 46);
      this.btnPatchRightLeft.TabIndex = 1;
      this.btnPatchRightLeft.Text = "Create Patch:\r\nMake Right Like Left";
      this.btnPatchRightLeft.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnPatchRightLeft.UseVisualStyleBackColor = false;
      this.btnPatchRightLeft.Click += new System.EventHandler(this.btnPatchRightLeft_Click);
      // 
      // btnPatchLeftRight
      // 
      this.btnPatchLeftRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPatchLeftRight.AutoSize = true;
      this.btnPatchLeftRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnPatchLeftRight.FlatAppearance.BorderSize = 0;
      this.btnPatchLeftRight.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPatchLeftRight.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPatchLeftRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnPatchLeftRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnPatchLeftRight.ForeColor = System.Drawing.Color.Black;
      this.btnPatchLeftRight.Location = new System.Drawing.Point(323, 301);
      this.btnPatchLeftRight.Name = "btnPatchLeftRight";
      this.btnPatchLeftRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnPatchLeftRight.Padding = new System.Windows.Forms.Padding(2);
      this.btnPatchLeftRight.Size = new System.Drawing.Size(128, 46);
      this.btnPatchLeftRight.TabIndex = 2;
      this.btnPatchLeftRight.Text = "Create Patch:\r\nMake Left Like Right";
      this.btnPatchLeftRight.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnPatchLeftRight.UseVisualStyleBackColor = false;
      this.btnPatchLeftRight.Click += new System.EventHandler(this.btnPatchLeftRight_Click);
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
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridDiffs)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Controls.DataGrid gridDiffs;
    private System.Windows.Forms.DataGridViewTextBoxColumn colName;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colLeftExists;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colRightExists;
    private System.Windows.Forms.DataGridViewTextBoxColumn colDiffType;
    private System.Windows.Forms.DataGridViewButtonColumn colCompare;
    private FlatButton btnPatchRightLeft;
    private FlatButton btnPatchLeftRight;
  }
}
