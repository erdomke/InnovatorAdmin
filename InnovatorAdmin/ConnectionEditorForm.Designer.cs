namespace InnovatorAdmin
{
  partial class ConnectionEditorForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionEditorForm));
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new InnovatorAdmin.Controls.FlatButton();
      this.connectionEditor = new InnovatorAdmin.ConnectionEditor();
      this.btnOk = new InnovatorAdmin.Controls.FlatButton();
      this.lblTitle = new InnovatorAdmin.Controls.NoCopyLabel();
      this.pnlTopLeft = new System.Windows.Forms.Panel();
      this.pnlTop = new System.Windows.Forms.Panel();
      this.pnlLeft = new System.Windows.Forms.Panel();
      this.pnlTopRight = new System.Windows.Forms.Panel();
      this.pnlRight = new System.Windows.Forms.Panel();
      this.pnlBottomLeft = new System.Windows.Forms.Panel();
      this.pnlBottom = new System.Windows.Forms.Panel();
      this.pnlBottomRight = new System.Windows.Forms.Panel();
      this.tableLayoutPanel1.SuspendLayout();
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
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this.connectionEditor, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnOk, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.lblTitle, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.pnlTopLeft, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlTop, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlLeft, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.pnlTopRight, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.pnlRight, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottomLeft, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottom, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.pnlBottomRight, 3, 4);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1327, 657);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.AutoSize = true;
      this.btnClose.BackColor = System.Drawing.Color.White;
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.FlatAppearance.BorderSize = 0;
      this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
      this.btnClose.Location = new System.Drawing.Point(1199, 615);
      this.btnClose.MinimumSize = new System.Drawing.Size(87, 0);
      this.btnClose.Name = "btnClose";
      this.btnClose.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnClose.Padding = new System.Windows.Forms.Padding(2);
      this.btnClose.Size = new System.Drawing.Size(122, 36);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "&CANCEL";
      this.btnClose.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Dialog;
      this.btnClose.UseVisualStyleBackColor = false;
      // 
      // connectionEditor
      // 
      this.connectionEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.connectionEditor, 2);
      this.connectionEditor.Location = new System.Drawing.Point(3, 50);
      this.connectionEditor.Margin = new System.Windows.Forms.Padding(0);
      this.connectionEditor.MinimumSize = new System.Drawing.Size(496, 196);
      this.connectionEditor.MultiSelect = false;
      this.connectionEditor.Name = "connectionEditor";
      this.connectionEditor.Size = new System.Drawing.Size(1321, 562);
      this.connectionEditor.TabIndex = 0;
      // 
      // btnOk
      // 
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.AutoSize = true;
      this.btnOk.BackColor = System.Drawing.Color.White;
      this.btnOk.FlatAppearance.BorderSize = 0;
      this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.btnOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
      this.btnOk.Location = new System.Drawing.Point(1052, 615);
      this.btnOk.MinimumSize = new System.Drawing.Size(87, 0);
      this.btnOk.Name = "btnOk";
      this.btnOk.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnOk.Padding = new System.Windows.Forms.Padding(2);
      this.btnOk.Size = new System.Drawing.Size(141, 36);
      this.btnOk.TabIndex = 1;
      this.btnOk.Text = "&CONNECT";
      this.btnOk.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Dialog;
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // lblTitle
      // 
      this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblTitle.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.lblTitle, 2);
      this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.Location = new System.Drawing.Point(3, 3);
      this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Padding = new System.Windows.Forms.Padding(6, 6, 0, 0);
      this.lblTitle.Size = new System.Drawing.Size(1321, 47);
      this.lblTitle.TabIndex = 3;
      this.lblTitle.Text = "Connect to a service.";
      this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
      this.pnlTopLeft.TabIndex = 4;
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
      this.pnlTop.Size = new System.Drawing.Size(1321, 3);
      this.pnlTop.TabIndex = 5;
      // 
      // pnlLeft
      // 
      this.pnlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlLeft.Location = new System.Drawing.Point(0, 3);
      this.pnlLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlLeft.Name = "pnlLeft";
      this.tableLayoutPanel1.SetRowSpan(this.pnlLeft, 3);
      this.pnlLeft.Size = new System.Drawing.Size(3, 651);
      this.pnlLeft.TabIndex = 6;
      // 
      // pnlTopRight
      // 
      this.pnlTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTopRight.Location = new System.Drawing.Point(1324, 0);
      this.pnlTopRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTopRight.Name = "pnlTopRight";
      this.pnlTopRight.Size = new System.Drawing.Size(3, 3);
      this.pnlTopRight.TabIndex = 7;
      // 
      // pnlRight
      // 
      this.pnlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlRight.Location = new System.Drawing.Point(1324, 3);
      this.pnlRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlRight.Name = "pnlRight";
      this.tableLayoutPanel1.SetRowSpan(this.pnlRight, 3);
      this.pnlRight.Size = new System.Drawing.Size(3, 651);
      this.pnlRight.TabIndex = 8;
      // 
      // pnlBottomLeft
      // 
      this.pnlBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomLeft.Location = new System.Drawing.Point(0, 654);
      this.pnlBottomLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomLeft.Name = "pnlBottomLeft";
      this.pnlBottomLeft.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomLeft.TabIndex = 9;
      // 
      // pnlBottom
      // 
      this.pnlBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.pnlBottom, 2);
      this.pnlBottom.Location = new System.Drawing.Point(3, 654);
      this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new System.Drawing.Size(1321, 3);
      this.pnlBottom.TabIndex = 10;
      // 
      // pnlBottomRight
      // 
      this.pnlBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomRight.Location = new System.Drawing.Point(1324, 654);
      this.pnlBottomRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomRight.Name = "pnlBottomRight";
      this.pnlBottomRight.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomRight.TabIndex = 11;
      // 
      // ConnectionEditorForm
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.DarkGray;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(1329, 659);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Location = new System.Drawing.Point(0, 0);
      this.Name = "ConnectionEditorForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Connect to a service.";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private InnovatorAdmin.Controls.FlatButton btnClose;
    private ConnectionEditor connectionEditor;
    private InnovatorAdmin.Controls.FlatButton btnOk;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Panel pnlTopLeft;
    private System.Windows.Forms.Panel pnlTop;
    private System.Windows.Forms.Panel pnlLeft;
    private System.Windows.Forms.Panel pnlTopRight;
    private System.Windows.Forms.Panel pnlRight;
    private System.Windows.Forms.Panel pnlBottomLeft;
    private System.Windows.Forms.Panel pnlBottom;
    private System.Windows.Forms.Panel pnlBottomRight;
  }
}
