namespace InnovatorAdmin.Dialog
{
  partial class ColumnSelect
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnMoveUp = new InnovatorAdmin.Controls.FlatButton();
      this.grid = new InnovatorAdmin.Controls.DataGrid();
      this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.colOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colPropertyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.btnCancel = new InnovatorAdmin.Controls.FlatButton();
      this.lblMessage = new System.Windows.Forms.Label();
      this.pnlTop = new System.Windows.Forms.Panel();
      this.pnlBottom = new System.Windows.Forms.Panel();
      this.pnlTopLeft = new System.Windows.Forms.Panel();
      this.pnlTopRight = new System.Windows.Forms.Panel();
      this.pnlBottomLeft = new System.Windows.Forms.Panel();
      this.pnlBottomRight = new System.Windows.Forms.Panel();
      this.lblTitle = new System.Windows.Forms.Label();
      this.pnlLeft = new System.Windows.Forms.Panel();
      this.pnlRight = new System.Windows.Forms.Panel();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnMoveDown = new InnovatorAdmin.Controls.FlatButton();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
      this.SuspendLayout();
      //
      // tableLayoutPanel1
      //
      this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 5;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.Controls.Add(this.btnMoveDown, 2, 5);
      this.tableLayoutPanel1.Controls.Add(this.btnMoveUp, 3, 4);
      this.tableLayoutPanel1.Controls.Add(this.grid, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.btnCancel, 3, 5);
      this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.pnlTop, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottom, 1, 6);
      this.tableLayoutPanel1.Controls.Add(this.pnlTopLeft, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlTopRight, 4, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottomLeft, 0, 6);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottomRight, 4, 6);
      this.tableLayoutPanel1.Controls.Add(this.lblTitle, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.pnlLeft, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.pnlRight, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 3);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 7;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(432, 405);
      this.tableLayoutPanel1.TabIndex = 0;
      //
      // btnMoveUp
      //
      this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnMoveUp.AutoSize = true;
      this.btnMoveUp.BackColor = System.Drawing.Color.White;
      this.btnMoveUp.FlatAppearance.BorderSize = 0;
      this.btnMoveUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMoveUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnMoveUp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
      this.btnMoveUp.Location = new System.Drawing.Point(248, 366);
      this.btnMoveUp.Name = "btnMoveUp";
      this.btnMoveUp.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnMoveUp.Padding = new System.Windows.Forms.Padding(2);
      this.btnMoveUp.Size = new System.Drawing.Size(44, 33);
      this.btnMoveUp.TabIndex = 21;
      this.btnMoveUp.Text = "Up";
      this.btnMoveUp.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Icon;
      this.btnMoveUp.UseVisualStyleBackColor = false;
      this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
      //
      // grid
      //
      this.grid.AllowUserToAddRows = false;
      this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grid.BackgroundColor = System.Drawing.Color.White;
      this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colVisible,
            this.colOrder,
            this.colPropertyName});
      this.tableLayoutPanel1.SetColumnSpan(this.grid, 3);
      this.grid.Location = new System.Drawing.Point(6, 68);
      this.grid.Name = "grid";
      this.grid.Size = new System.Drawing.Size(420, 292);
      this.grid.TabIndex = 20;
      //
      // colName
      //
      this.colName.DataPropertyName = "HeaderText";
      this.colName.HeaderText = "Name";
      this.colName.Name = "colName";
      this.colName.ReadOnly = true;
      this.colName.Width = 200;
      //
      // colVisible
      //
      this.colVisible.DataPropertyName = "Visible";
      this.colVisible.HeaderText = "Visible";
      this.colVisible.Name = "colVisible";
      this.colVisible.Width = 40;
      //
      // colOrder
      //
      this.colOrder.DataPropertyName = "DisplayIndex";
      this.colOrder.HeaderText = "Order";
      this.colOrder.Name = "colOrder";
      //
      // colPropertyName
      //
      this.colPropertyName.DataPropertyName = "DataPropertyName";
      this.colPropertyName.HeaderText = "PropertyName";
      this.colPropertyName.Name = "colPropertyName";
      this.colPropertyName.Width = 125;
      //
      // btnCancel
      //
      this.btnCancel.AutoSize = true;
      this.btnCancel.BackColor = System.Drawing.Color.White;
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderSize = 0;
      this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
      this.btnCancel.Location = new System.Drawing.Point(353, 366);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnCancel.Padding = new System.Windows.Forms.Padding(2);
      this.btnCancel.Size = new System.Drawing.Size(73, 33);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "&CLOSE";
      this.btnCancel.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Dialog;
      this.btnCancel.UseVisualStyleBackColor = false;
      //
      // lblMessage
      //
      this.lblMessage.AutoSize = true;
      this.lblMessage.Location = new System.Drawing.Point(6, 40);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(3);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 15);
      this.lblMessage.TabIndex = 4;
      //
      // pnlTop
      //
      this.pnlTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.pnlTop, 3);
      this.pnlTop.Location = new System.Drawing.Point(3, 0);
      this.pnlTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new System.Drawing.Size(426, 3);
      this.pnlTop.TabIndex = 7;
      //
      // pnlBottom
      //
      this.pnlBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.pnlBottom, 3);
      this.pnlBottom.Location = new System.Drawing.Point(3, 402);
      this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new System.Drawing.Size(426, 3);
      this.pnlBottom.TabIndex = 8;
      //
      // pnlTopLeft
      //
      this.pnlTopLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTopLeft.Location = new System.Drawing.Point(0, 0);
      this.pnlTopLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTopLeft.Name = "pnlTopLeft";
      this.pnlTopLeft.Size = new System.Drawing.Size(3, 3);
      this.pnlTopLeft.TabIndex = 10;
      //
      // pnlTopRight
      //
      this.pnlTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTopRight.Location = new System.Drawing.Point(429, 0);
      this.pnlTopRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTopRight.Name = "pnlTopRight";
      this.pnlTopRight.Size = new System.Drawing.Size(3, 3);
      this.pnlTopRight.TabIndex = 11;
      //
      // pnlBottomLeft
      //
      this.pnlBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomLeft.Location = new System.Drawing.Point(0, 402);
      this.pnlBottomLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomLeft.Name = "pnlBottomLeft";
      this.pnlBottomLeft.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomLeft.TabIndex = 12;
      //
      // pnlBottomRight
      //
      this.pnlBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomRight.Location = new System.Drawing.Point(429, 402);
      this.pnlBottomRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomRight.Name = "pnlBottomRight";
      this.pnlBottomRight.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomRight.TabIndex = 13;
      //
      // lblTitle
      //
      this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblTitle.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.lblTitle, 3);
      this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.Location = new System.Drawing.Point(6, 6);
      this.lblTitle.Margin = new System.Windows.Forms.Padding(3);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
      this.lblTitle.Size = new System.Drawing.Size(420, 28);
      this.lblTitle.TabIndex = 16;
      this.lblTitle.Text = "Column Select";
      //
      // pnlLeft
      //
      this.pnlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlLeft.Location = new System.Drawing.Point(0, 3);
      this.pnlLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlLeft.Name = "pnlLeft";
      this.tableLayoutPanel1.SetRowSpan(this.pnlLeft, 5);
      this.pnlLeft.Size = new System.Drawing.Size(3, 399);
      this.pnlLeft.TabIndex = 17;
      //
      // pnlRight
      //
      this.pnlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlRight.Location = new System.Drawing.Point(429, 3);
      this.pnlRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlRight.Name = "pnlRight";
      this.tableLayoutPanel1.SetRowSpan(this.pnlRight, 5);
      this.pnlRight.Size = new System.Drawing.Size(3, 399);
      this.pnlRight.TabIndex = 18;
      //
      // panel1
      //
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.LightGray;
      this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
      this.panel1.Location = new System.Drawing.Point(6, 61);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(420, 1);
      this.panel1.TabIndex = 19;
      //
      // btnMoveDown
      //
      this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnMoveDown.AutoSize = true;
      this.btnMoveDown.BackColor = System.Drawing.Color.White;
      this.btnMoveDown.FlatAppearance.BorderSize = 0;
      this.btnMoveDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMoveDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnMoveDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
      this.btnMoveDown.Location = new System.Drawing.Point(298, 366);
      this.btnMoveDown.Name = "btnMoveDown";
      this.btnMoveDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnMoveDown.Padding = new System.Windows.Forms.Padding(2);
      this.btnMoveDown.Size = new System.Drawing.Size(49, 33);
      this.btnMoveDown.TabIndex = 22;
      this.btnMoveDown.Text = "Down";
      this.btnMoveDown.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Icon;
      this.btnMoveDown.UseVisualStyleBackColor = false;
      this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
      //
      // ColumnSelect
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.DarkGray;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(434, 407);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Location = new System.Drawing.Point(0, 0);
      this.Name = "ColumnSelect";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Column Select";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Controls.FlatButton btnCancel;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.Panel pnlTop;
    private System.Windows.Forms.Panel pnlBottom;
    private System.Windows.Forms.Panel pnlTopLeft;
    private System.Windows.Forms.Panel pnlTopRight;
    private System.Windows.Forms.Panel pnlBottomLeft;
    private System.Windows.Forms.Panel pnlBottomRight;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Panel pnlLeft;
    private System.Windows.Forms.Panel pnlRight;
    private System.Windows.Forms.Panel panel1;
    private Controls.DataGrid grid;
    private System.Windows.Forms.DataGridViewTextBoxColumn colName;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
    private System.Windows.Forms.DataGridViewTextBoxColumn colOrder;
    private System.Windows.Forms.DataGridViewTextBoxColumn colPropertyName;
    private Controls.FlatButton btnMoveUp;
    private Controls.FlatButton btnMoveDown;
  }
}
