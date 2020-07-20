namespace InnovatorAdmin
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
      this.btnNext = new InnovatorAdmin.Controls.FlatButton();
      this.btnPrevious = new InnovatorAdmin.Controls.FlatButton();
      this.btnClose = new InnovatorAdmin.Controls.FlatButton();
      this.tblMain = new System.Windows.Forms.TableLayoutPanel();
      this.tblHeader = new System.Windows.Forms.TableLayoutPanel();
      this.lblClose = new System.Windows.Forms.Label();
      this.lblTitle = new InnovatorAdmin.Controls.NoCopyLabel();
      this.lblMaximize = new System.Windows.Forms.Label();
      this.lblMinimize = new System.Windows.Forms.Label();
      this.picLogo = new System.Windows.Forms.PictureBox();
      this.lblMessage = new System.Windows.Forms.Label();
      this.pnlTopLeft = new System.Windows.Forms.Panel();
      this.pnlTop = new System.Windows.Forms.Panel();
      this.pnlTopRight = new System.Windows.Forms.Panel();
      this.pnlBottomLeft = new System.Windows.Forms.Panel();
      this.pnlBottomRight = new System.Windows.Forms.Panel();
      this.pnlRight = new InnovatorAdmin.DropShadow();
      this.pnlLeft = new InnovatorAdmin.DropShadow();
      this.pnlLeftTop = new System.Windows.Forms.Panel();
      this.pnlRightTop = new System.Windows.Forms.Panel();
      this.pnlConnectionColor = new InnovatorAdmin.DropShadow();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.lblVersion = new System.Windows.Forms.Label();
      this.pnlBottom = new System.Windows.Forms.Panel();
      this.tblMain.SuspendLayout();
      this.tblHeader.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnNext
      // 
      this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNext.AutoSize = true;
      this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(51)))));
      this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(3)))), ((int)(((byte)(32)))));
      this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(3)))), ((int)(((byte)(32)))));
      this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnNext.ForeColor = System.Drawing.Color.White;
      this.btnNext.Location = new System.Drawing.Point(427, 3);
      this.btnNext.Name = "btnNext";
      this.btnNext.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnNext.Padding = new System.Windows.Forms.Padding(2);
      this.btnNext.Size = new System.Drawing.Size(71, 36);
      this.btnNext.TabIndex = 0;
      this.btnNext.Text = "&Next";
      this.btnNext.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Red;
      this.btnNext.UseVisualStyleBackColor = false;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // btnPrevious
      // 
      this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrevious.AutoSize = true;
      this.btnPrevious.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnPrevious.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPrevious.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnPrevious.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnPrevious.ForeColor = System.Drawing.Color.Black;
      this.btnPrevious.Location = new System.Drawing.Point(310, 3);
      this.btnPrevious.Name = "btnPrevious";
      this.btnPrevious.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnPrevious.Padding = new System.Windows.Forms.Padding(2);
      this.btnPrevious.Size = new System.Drawing.Size(111, 36);
      this.btnPrevious.TabIndex = 1;
      this.btnPrevious.Text = "&Previous";
      this.btnPrevious.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnPrevious.UseVisualStyleBackColor = true;
      this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.AutoSize = true;
      this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnClose.ForeColor = System.Drawing.Color.Black;
      this.btnClose.Location = new System.Drawing.Point(504, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnClose.Padding = new System.Windows.Forms.Padding(2);
      this.btnClose.Size = new System.Drawing.Size(82, 36);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "&Close";
      this.btnClose.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // tblMain
      // 
      this.tblMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tblMain.BackColor = System.Drawing.Color.White;
      this.tblMain.ColumnCount = 3;
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.Controls.Add(this.tblHeader, 1, 1);
      this.tblMain.Controls.Add(this.pnlTopLeft, 0, 0);
      this.tblMain.Controls.Add(this.pnlTop, 1, 0);
      this.tblMain.Controls.Add(this.pnlTopRight, 2, 0);
      this.tblMain.Controls.Add(this.pnlBottomLeft, 0, 5);
      this.tblMain.Controls.Add(this.pnlBottomRight, 2, 5);
      this.tblMain.Controls.Add(this.pnlRight, 2, 2);
      this.tblMain.Controls.Add(this.pnlLeft, 0, 2);
      this.tblMain.Controls.Add(this.pnlLeftTop, 0, 1);
      this.tblMain.Controls.Add(this.pnlRightTop, 2, 1);
      this.tblMain.Controls.Add(this.pnlConnectionColor, 1, 2);
      this.tblMain.Controls.Add(this.tableLayoutPanel1, 1, 4);
      this.tblMain.Controls.Add(this.pnlBottom, 1, 5);
      this.tblMain.Location = new System.Drawing.Point(1, 1);
      this.tblMain.Name = "tblMain";
      this.tblMain.RowCount = 6;
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.Size = new System.Drawing.Size(595, 382);
      this.tblMain.TabIndex = 2;
      // 
      // tblHeader
      // 
      this.tblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tblHeader.AutoSize = true;
      this.tblHeader.BackColor = System.Drawing.Color.White;
      this.tblHeader.ColumnCount = 5;
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.Controls.Add(this.lblClose, 4, 0);
      this.tblHeader.Controls.Add(this.lblTitle, 1, 0);
      this.tblHeader.Controls.Add(this.lblMaximize, 3, 0);
      this.tblHeader.Controls.Add(this.lblMinimize, 2, 0);
      this.tblHeader.Controls.Add(this.picLogo, 0, 0);
      this.tblHeader.Controls.Add(this.lblMessage, 1, 1);
      this.tblHeader.Location = new System.Drawing.Point(3, 3);
      this.tblHeader.Margin = new System.Windows.Forms.Padding(0);
      this.tblHeader.Name = "tblHeader";
      this.tblHeader.RowCount = 2;
      this.tblHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tblHeader.Size = new System.Drawing.Size(589, 74);
      this.tblHeader.TabIndex = 1;
      // 
      // lblClose
      // 
      this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblClose.BackColor = System.Drawing.Color.Transparent;
      this.lblClose.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblClose.Location = new System.Drawing.Point(565, 0);
      this.lblClose.Margin = new System.Windows.Forms.Padding(0);
      this.lblClose.Name = "lblClose";
      this.lblClose.Size = new System.Drawing.Size(24, 32);
      this.lblClose.TabIndex = 14;
      this.lblClose.Text = "r";
      this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblTitle
      // 
      this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblTitle.BackColor = System.Drawing.Color.Transparent;
      this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.ForeColor = System.Drawing.SystemColors.ControlText;
      this.lblTitle.Location = new System.Drawing.Point(80, 0);
      this.lblTitle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new System.Drawing.Size(437, 32);
      this.lblTitle.TabIndex = 11;
      this.lblTitle.Text = "Title";
      this.lblTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // lblMaximize
      // 
      this.lblMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblMaximize.BackColor = System.Drawing.Color.Transparent;
      this.lblMaximize.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMaximize.Location = new System.Drawing.Point(541, 0);
      this.lblMaximize.Margin = new System.Windows.Forms.Padding(0);
      this.lblMaximize.Name = "lblMaximize";
      this.lblMaximize.Size = new System.Drawing.Size(24, 32);
      this.lblMaximize.TabIndex = 13;
      this.lblMaximize.Text = "1";
      this.lblMaximize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblMinimize
      // 
      this.lblMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblMinimize.BackColor = System.Drawing.Color.Transparent;
      this.lblMinimize.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMinimize.Location = new System.Drawing.Point(517, 0);
      this.lblMinimize.Margin = new System.Windows.Forms.Padding(0);
      this.lblMinimize.Name = "lblMinimize";
      this.lblMinimize.Size = new System.Drawing.Size(24, 32);
      this.lblMinimize.TabIndex = 12;
      this.lblMinimize.Text = "0";
      this.lblMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // picLogo
      // 
      this.picLogo.Location = new System.Drawing.Point(16, 3);
      this.picLogo.Margin = new System.Windows.Forms.Padding(16, 3, 16, 3);
      this.picLogo.Name = "picLogo";
      this.tblHeader.SetRowSpan(this.picLogo, 2);
      this.picLogo.Size = new System.Drawing.Size(48, 48);
      this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.picLogo.TabIndex = 15;
      this.picLogo.TabStop = false;
      // 
      // lblMessage
      // 
      this.lblMessage.AutoSize = true;
      this.lblMessage.Location = new System.Drawing.Point(83, 41);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(3);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 30);
      this.lblMessage.TabIndex = 16;
      // 
      // pnlTopLeft
      // 
      this.pnlTopLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTopLeft.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlTopLeft.Location = new System.Drawing.Point(0, 0);
      this.pnlTopLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTopLeft.Name = "pnlTopLeft";
      this.pnlTopLeft.Size = new System.Drawing.Size(3, 3);
      this.pnlTopLeft.TabIndex = 6;
      // 
      // pnlTop
      // 
      this.pnlTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTop.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlTop.Location = new System.Drawing.Point(3, 0);
      this.pnlTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new System.Drawing.Size(589, 3);
      this.pnlTop.TabIndex = 7;
      // 
      // pnlTopRight
      // 
      this.pnlTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTopRight.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlTopRight.Location = new System.Drawing.Point(592, 0);
      this.pnlTopRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTopRight.Name = "pnlTopRight";
      this.pnlTopRight.Size = new System.Drawing.Size(3, 3);
      this.pnlTopRight.TabIndex = 8;
      // 
      // pnlBottomLeft
      // 
      this.pnlBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomLeft.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlBottomLeft.Location = new System.Drawing.Point(0, 379);
      this.pnlBottomLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomLeft.Name = "pnlBottomLeft";
      this.pnlBottomLeft.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomLeft.TabIndex = 10;
      // 
      // pnlBottomRight
      // 
      this.pnlBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomRight.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlBottomRight.Location = new System.Drawing.Point(592, 379);
      this.pnlBottomRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomRight.Name = "pnlBottomRight";
      this.pnlBottomRight.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomRight.TabIndex = 12;
      // 
      // pnlRight
      // 
      this.pnlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlRight.Location = new System.Drawing.Point(592, 77);
      this.pnlRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlRight.Name = "pnlRight";
      this.tblMain.SetRowSpan(this.pnlRight, 3);
      this.pnlRight.ShadowColor = System.Drawing.Color.Empty;
      this.pnlRight.ShadowExtent = 5;
      this.pnlRight.Size = new System.Drawing.Size(3, 302);
      this.pnlRight.TabIndex = 14;
      // 
      // pnlLeft
      // 
      this.pnlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlLeft.Location = new System.Drawing.Point(0, 77);
      this.pnlLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlLeft.Name = "pnlLeft";
      this.tblMain.SetRowSpan(this.pnlLeft, 3);
      this.pnlLeft.ShadowColor = System.Drawing.Color.Empty;
      this.pnlLeft.ShadowExtent = 5;
      this.pnlLeft.Size = new System.Drawing.Size(3, 302);
      this.pnlLeft.TabIndex = 13;
      // 
      // pnlLeftTop
      // 
      this.pnlLeftTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlLeftTop.Location = new System.Drawing.Point(0, 3);
      this.pnlLeftTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlLeftTop.Name = "pnlLeftTop";
      this.pnlLeftTop.Size = new System.Drawing.Size(3, 74);
      this.pnlLeftTop.TabIndex = 15;
      // 
      // pnlRightTop
      // 
      this.pnlRightTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlRightTop.Location = new System.Drawing.Point(592, 3);
      this.pnlRightTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlRightTop.Name = "pnlRightTop";
      this.pnlRightTop.Size = new System.Drawing.Size(3, 74);
      this.pnlRightTop.TabIndex = 16;
      // 
      // pnlConnectionColor
      // 
      this.pnlConnectionColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlConnectionColor.Location = new System.Drawing.Point(3, 77);
      this.pnlConnectionColor.Margin = new System.Windows.Forms.Padding(0);
      this.pnlConnectionColor.Name = "pnlConnectionColor";
      this.pnlConnectionColor.ShadowColor = System.Drawing.Color.Empty;
      this.pnlConnectionColor.ShadowExtent = 0;
      this.pnlConnectionColor.Size = new System.Drawing.Size(589, 5);
      this.pnlConnectionColor.TabIndex = 17;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.btnNext, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnPrevious, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.lblVersion, 0, 0);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 337);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 42);
      this.tableLayoutPanel1.TabIndex = 19;
      // 
      // lblVersion
      // 
      this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblVersion.AutoSize = true;
      this.lblVersion.Location = new System.Drawing.Point(3, 6);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(0, 30);
      this.lblVersion.TabIndex = 18;
      // 
      // pnlBottom
      // 
      this.pnlBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottom.Location = new System.Drawing.Point(3, 379);
      this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new System.Drawing.Size(589, 3);
      this.pnlBottom.TabIndex = 20;
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.DarkGray;
      this.ClientSize = new System.Drawing.Size(597, 384);
      this.Controls.Add(this.tblMain);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Location = new System.Drawing.Point(0, 0);
      this.Name = "Main";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "Innovator Installer";
      this.tblMain.ResumeLayout(false);
      this.tblMain.PerformLayout();
      this.tblHeader.ResumeLayout(false);
      this.tblHeader.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private Controls.FlatButton btnNext;
    private Controls.FlatButton btnPrevious;
    private Controls.FlatButton btnClose;
    private System.Windows.Forms.TableLayoutPanel tblMain;
    private System.Windows.Forms.TableLayoutPanel tblHeader;
    private System.Windows.Forms.Label lblClose;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblMaximize;
    private System.Windows.Forms.Label lblMinimize;
    private System.Windows.Forms.PictureBox picLogo;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.Panel pnlTopLeft;
    private System.Windows.Forms.Panel pnlTop;
    private System.Windows.Forms.Panel pnlTopRight;
    private System.Windows.Forms.Panel pnlBottomLeft;
    private System.Windows.Forms.Panel pnlBottomRight;
    private DropShadow pnlRight;
    private DropShadow pnlLeft;
    private System.Windows.Forms.Panel pnlLeftTop;
    private System.Windows.Forms.Panel pnlRightTop;
    private DropShadow pnlConnectionColor;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Panel pnlBottom;

  }
}

