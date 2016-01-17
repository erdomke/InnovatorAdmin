namespace InnovatorAdmin.Dialog
{
  partial class ConnectionAdvancedDialog
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
      this.btnOK = new InnovatorAdmin.Controls.FlatButton();
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
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.chkConfirm = new System.Windows.Forms.CheckBox();
      this.Timeout = new System.Windows.Forms.Label();
      this.txtTimeout = new System.Windows.Forms.TextBox();
      this.gridHeaders = new InnovatorAdmin.Controls.DataGrid();
      this.colName = new InnovatorAdmin.Controls.DataGridViewListColumn();
      this.colValue = new InnovatorAdmin.Controls.DataGridViewListColumn();
      this.label1 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridHeaders)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.Controls.Add(this.btnOK, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.pnlTop, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottom, 1, 6);
      this.tableLayoutPanel1.Controls.Add(this.pnlTopLeft, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlTopRight, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottomLeft, 0, 6);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottomRight, 3, 6);
      this.tableLayoutPanel1.Controls.Add(this.lblTitle, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.pnlLeft, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.pnlRight, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 4);
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
      this.tableLayoutPanel1.Size = new System.Drawing.Size(417, 292);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOK.AutoSize = true;
      this.btnOK.BackColor = System.Drawing.Color.White;
      this.btnOK.FlatAppearance.BorderSize = 0;
      this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
      this.btnOK.Location = new System.Drawing.Point(335, 253);
      this.btnOK.Name = "btnOK";
      this.btnOK.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnOK.Padding = new System.Windows.Forms.Padding(2);
      this.btnOK.Size = new System.Drawing.Size(76, 33);
      this.btnOK.TabIndex = 2;
      this.btnOK.Text = "&DONE";
      this.btnOK.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Dialog;
      this.btnOK.UseVisualStyleBackColor = false;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
      this.tableLayoutPanel1.SetColumnSpan(this.pnlTop, 2);
      this.pnlTop.Location = new System.Drawing.Point(3, 0);
      this.pnlTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new System.Drawing.Size(411, 3);
      this.pnlTop.TabIndex = 7;
      // 
      // pnlBottom
      // 
      this.pnlBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.pnlBottom, 2);
      this.pnlBottom.Location = new System.Drawing.Point(3, 289);
      this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new System.Drawing.Size(411, 3);
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
      this.pnlTopRight.Location = new System.Drawing.Point(414, 0);
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
      this.pnlBottomLeft.Location = new System.Drawing.Point(0, 289);
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
      this.pnlBottomRight.Location = new System.Drawing.Point(414, 289);
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
      this.tableLayoutPanel1.SetColumnSpan(this.lblTitle, 2);
      this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.Location = new System.Drawing.Point(6, 6);
      this.lblTitle.Margin = new System.Windows.Forms.Padding(3);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
      this.lblTitle.Size = new System.Drawing.Size(405, 28);
      this.lblTitle.TabIndex = 16;
      this.lblTitle.Text = "Advanced Settings";
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
      this.pnlLeft.Size = new System.Drawing.Size(3, 286);
      this.pnlLeft.TabIndex = 17;
      // 
      // pnlRight
      // 
      this.pnlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlRight.Location = new System.Drawing.Point(414, 3);
      this.pnlRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlRight.Name = "pnlRight";
      this.tableLayoutPanel1.SetRowSpan(this.pnlRight, 5);
      this.pnlRight.Size = new System.Drawing.Size(3, 286);
      this.pnlRight.TabIndex = 18;
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.LightGray;
      this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
      this.panel1.Location = new System.Drawing.Point(6, 61);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(405, 1);
      this.panel1.TabIndex = 19;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.chkConfirm, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.Timeout, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtTimeout, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.gridHeaders, 0, 3);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 2);
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 65);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 4;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(411, 185);
      this.tableLayoutPanel2.TabIndex = 20;
      // 
      // chkConfirm
      // 
      this.chkConfirm.AutoSize = true;
      this.chkConfirm.Location = new System.Drawing.Point(61, 3);
      this.chkConfirm.Name = "chkConfirm";
      this.chkConfirm.Size = new System.Drawing.Size(164, 19);
      this.chkConfirm.TabIndex = 0;
      this.chkConfirm.Text = "Confirm Query Execution?";
      this.chkConfirm.UseVisualStyleBackColor = true;
      // 
      // Timeout
      // 
      this.Timeout.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.Timeout.AutoSize = true;
      this.Timeout.Location = new System.Drawing.Point(3, 32);
      this.Timeout.Name = "Timeout";
      this.Timeout.Size = new System.Drawing.Size(52, 15);
      this.Timeout.TabIndex = 1;
      this.Timeout.Text = "Timeout";
      // 
      // txtTimeout
      // 
      this.txtTimeout.Location = new System.Drawing.Point(61, 28);
      this.txtTimeout.Name = "txtTimeout";
      this.txtTimeout.Size = new System.Drawing.Size(107, 23);
      this.txtTimeout.TabIndex = 2;
      // 
      // gridHeaders
      // 
      this.gridHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gridHeaders.BackgroundColor = System.Drawing.Color.White;
      this.gridHeaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.gridHeaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridHeaders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colValue});
      this.tableLayoutPanel2.SetColumnSpan(this.gridHeaders, 2);
      this.gridHeaders.Location = new System.Drawing.Point(3, 88);
      this.gridHeaders.Name = "gridHeaders";
      this.gridHeaders.RowHeadersWidth = 25;
      this.gridHeaders.Size = new System.Drawing.Size(405, 94);
      this.gridHeaders.TabIndex = 3;
      this.gridHeaders.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.gridHeaders_CellBeginEdit);
      // 
      // colName
      // 
      this.colName.DataPropertyName = "Name";
      this.colName.DataSource = null;
      this.colName.DisplayMember = null;
      this.colName.HeaderText = "Name";
      this.colName.Name = "colName";
      this.colName.ValueMember = null;
      this.colName.Width = 150;
      // 
      // colValue
      // 
      this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.colValue.DataPropertyName = "Value";
      this.colValue.DataSource = null;
      this.colValue.DisplayMember = null;
      this.colValue.HeaderText = "Value";
      this.colValue.Name = "colValue";
      this.colValue.ValueMember = null;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.label1, 2);
      this.label1.Location = new System.Drawing.Point(3, 57);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
      this.label1.Size = new System.Drawing.Size(50, 25);
      this.label1.TabIndex = 4;
      this.label1.Text = "Headers";
      // 
      // ConnectionAdvancedDialog
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.DarkGray;
      this.ClientSize = new System.Drawing.Size(419, 294);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Location = new System.Drawing.Point(0, 0);
      this.Name = "ConnectionAdvancedDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Advanced Settings";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridHeaders)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Controls.FlatButton btnOK;
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
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.CheckBox chkConfirm;
    private System.Windows.Forms.Label Timeout;
    private System.Windows.Forms.TextBox txtTimeout;
    private Controls.DataGrid gridHeaders;
    private System.Windows.Forms.Label label1;
    private Controls.DataGridViewListColumn colName;
    private Controls.DataGridViewListColumn colValue;
  }
}
