namespace InnovatorAdmin
{
  partial class ConnectionEditor
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
      this.components = new System.ComponentModel.Container();
      this.lblDatabase = new System.Windows.Forms.Label();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.lblPassword = new System.Windows.Forms.Label();
      this.txtUser = new System.Windows.Forms.TextBox();
      this.lblUser = new System.Windows.Forms.Label();
      this.txtUrl = new System.Windows.Forms.TextBox();
      this.lblUrl = new System.Windows.Forms.Label();
      this.txtName = new System.Windows.Forms.TextBox();
      this.lblName = new System.Windows.Forms.Label();
      this.toolTipManager = new System.Windows.Forms.ToolTip(this.components);
      this.tlpControls = new System.Windows.Forms.TableLayoutPanel();
      this.btnColor = new System.Windows.Forms.Button();
      this.lblMessage = new System.Windows.Forms.Label();
      this.cmbDatabase = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cmbType = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.cmbAuth = new System.Windows.Forms.ComboBox();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.cb_confirm = new System.Windows.Forms.CheckBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.lstConnections = new InnovatorAdmin.MultiselectListBox();
      this.btnMoveDown = new InnovatorAdmin.Controls.FlatButton();
      this.btnNew = new InnovatorAdmin.Controls.FlatButton();
      this.btnMoveUp = new InnovatorAdmin.Controls.FlatButton();
      this.btnCopy = new InnovatorAdmin.Controls.FlatButton();
      this.btnDelete = new InnovatorAdmin.Controls.FlatButton();
      this.exploreButton = new InnovatorAdmin.Controls.FlatButton();
      this.btnTest = new InnovatorAdmin.Controls.FlatButton();
      this.tlpControls.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblDatabase
      // 
      this.lblDatabase.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblDatabase.AutoSize = true;
      this.lblDatabase.Location = new System.Drawing.Point(409, 115);
      this.lblDatabase.Name = "lblDatabase";
      this.lblDatabase.Size = new System.Drawing.Size(56, 13);
      this.lblDatabase.TabIndex = 26;
      this.lblDatabase.Text = "Database:";
      this.lblDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtPassword
      // 
      this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.txtPassword, 2);
      this.txtPassword.Location = new System.Drawing.Point(471, 85);
      this.txtPassword.MaxLength = 64;
      this.txtPassword.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.PasswordChar = '*';
      this.txtPassword.Size = new System.Drawing.Size(129, 20);
      this.txtPassword.TabIndex = 7;
      this.txtPassword.WordWrap = false;
      // 
      // lblPassword
      // 
      this.lblPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblPassword.AutoSize = true;
      this.lblPassword.Location = new System.Drawing.Point(409, 88);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new System.Drawing.Size(56, 13);
      this.lblPassword.TabIndex = 24;
      this.lblPassword.Text = "Password:";
      // 
      // txtUser
      // 
      this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtUser.Location = new System.Drawing.Point(284, 85);
      this.txtUser.MaximumSize = new System.Drawing.Size(160, 4);
      this.txtUser.MaxLength = 32;
      this.txtUser.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtUser.Name = "txtUser";
      this.txtUser.Size = new System.Drawing.Size(119, 20);
      this.txtUser.TabIndex = 6;
      // 
      // lblUser
      // 
      this.lblUser.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblUser.AutoSize = true;
      this.lblUser.Location = new System.Drawing.Point(203, 88);
      this.lblUser.Name = "lblUser";
      this.lblUser.Size = new System.Drawing.Size(32, 13);
      this.lblUser.TabIndex = 22;
      this.lblUser.Text = "User:";
      this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtUrl
      // 
      this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.txtUrl, 4);
      this.txtUrl.Location = new System.Drawing.Point(284, 32);
      this.txtUrl.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtUrl.Name = "txtUrl";
      this.txtUrl.Size = new System.Drawing.Size(316, 20);
      this.txtUrl.TabIndex = 3;
      // 
      // lblUrl
      // 
      this.lblUrl.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblUrl.AutoSize = true;
      this.lblUrl.Location = new System.Drawing.Point(203, 35);
      this.lblUrl.Name = "lblUrl";
      this.lblUrl.Size = new System.Drawing.Size(32, 13);
      this.lblUrl.TabIndex = 20;
      this.lblUrl.Text = "URL:";
      this.lblUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtName
      // 
      this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.txtName, 3);
      this.txtName.Location = new System.Drawing.Point(284, 3);
      this.txtName.MaximumSize = new System.Drawing.Size(260, 4);
      this.txtName.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(260, 22);
      this.txtName.TabIndex = 1;
      // 
      // lblName
      // 
      this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblName.AutoSize = true;
      this.lblName.Location = new System.Drawing.Point(203, 8);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(38, 13);
      this.lblName.TabIndex = 32;
      this.lblName.Text = "Name:";
      this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // tlpControls
      // 
      this.tlpControls.BackColor = System.Drawing.Color.White;
      this.tlpControls.ColumnCount = 12;
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.Controls.Add(this.lstConnections, 0, 0);
      this.tlpControls.Controls.Add(this.btnMoveDown, 5, 6);
      this.tlpControls.Controls.Add(this.txtName, 8, 0);
      this.tlpControls.Controls.Add(this.btnNew, 0, 6);
      this.tlpControls.Controls.Add(this.lblName, 7, 0);
      this.tlpControls.Controls.Add(this.btnMoveUp, 4, 6);
      this.tlpControls.Controls.Add(this.txtUrl, 8, 1);
      this.tlpControls.Controls.Add(this.btnCopy, 1, 6);
      this.tlpControls.Controls.Add(this.btnDelete, 2, 6);
      this.tlpControls.Controls.Add(this.lblUrl, 7, 1);
      this.tlpControls.Controls.Add(this.btnColor, 11, 0);
      this.tlpControls.Controls.Add(this.lblUser, 7, 3);
      this.tlpControls.Controls.Add(this.lblMessage, 7, 5);
      this.tlpControls.Controls.Add(this.txtUser, 8, 3);
      this.tlpControls.Controls.Add(this.lblPassword, 9, 3);
      this.tlpControls.Controls.Add(this.txtPassword, 10, 3);
      this.tlpControls.Controls.Add(this.cmbDatabase, 10, 4);
      this.tlpControls.Controls.Add(this.lblDatabase, 9, 4);
      this.tlpControls.Controls.Add(this.label1, 7, 4);
      this.tlpControls.Controls.Add(this.cmbType, 8, 4);
      this.tlpControls.Controls.Add(this.label2, 7, 2);
      this.tlpControls.Controls.Add(this.cmbAuth, 8, 2);
      this.tlpControls.Controls.Add(this.flowLayoutPanel1, 9, 5);
      this.tlpControls.Controls.Add(this.cb_confirm, 10, 2);
      this.tlpControls.Controls.Add(this.panel1, 6, 0);
      this.tlpControls.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpControls.Location = new System.Drawing.Point(0, 0);
      this.tlpControls.Name = "tlpControls";
      this.tlpControls.RowCount = 7;
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.Size = new System.Drawing.Size(603, 207);
      this.tlpControls.TabIndex = 41;
      // 
      // btnColor
      // 
      this.btnColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnColor.Location = new System.Drawing.Point(571, 3);
      this.btnColor.Name = "btnColor";
      this.btnColor.Size = new System.Drawing.Size(29, 23);
      this.btnColor.TabIndex = 2;
      this.btnColor.UseVisualStyleBackColor = true;
      this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
      // 
      // lblMessage
      // 
      this.lblMessage.AutoSize = true;
      this.tlpControls.SetColumnSpan(this.lblMessage, 2);
      this.lblMessage.Location = new System.Drawing.Point(206, 141);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(6);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 13);
      this.lblMessage.TabIndex = 41;
      // 
      // cmbDatabase
      // 
      this.cmbDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.cmbDatabase, 2);
      this.cmbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbDatabase.FormattingEnabled = true;
      this.cmbDatabase.Location = new System.Drawing.Point(471, 111);
      this.cmbDatabase.Name = "cmbDatabase";
      this.cmbDatabase.Size = new System.Drawing.Size(129, 21);
      this.cmbDatabase.TabIndex = 9;
      this.cmbDatabase.DropDown += new System.EventHandler(this.cmbDatabase_DropDown);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(203, 115);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(34, 13);
      this.label1.TabIndex = 43;
      this.label1.Text = "Type:";
      // 
      // cmbType
      // 
      this.cmbType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbType.FormattingEnabled = true;
      this.cmbType.Location = new System.Drawing.Point(284, 111);
      this.cmbType.Name = "cmbType";
      this.cmbType.Size = new System.Drawing.Size(119, 21);
      this.cmbType.TabIndex = 8;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(203, 62);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(75, 13);
      this.label2.TabIndex = 44;
      this.label2.Text = "Authentication";
      // 
      // cmbAuth
      // 
      this.cmbAuth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbAuth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbAuth.FormattingEnabled = true;
      this.cmbAuth.Location = new System.Drawing.Point(284, 58);
      this.cmbAuth.Name = "cmbAuth";
      this.cmbAuth.Size = new System.Drawing.Size(119, 21);
      this.cmbAuth.TabIndex = 4;
      this.cmbAuth.SelectedIndexChanged += new System.EventHandler(this.cmbAuth_SelectedIndexChanged);
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.flowLayoutPanel1.AutoSize = true;
      this.tlpControls.SetColumnSpan(this.flowLayoutPanel1, 3);
      this.flowLayoutPanel1.Controls.Add(this.exploreButton);
      this.flowLayoutPanel1.Controls.Add(this.btnTest);
      this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(474, 135);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(129, 37);
      this.flowLayoutPanel1.TabIndex = 45;
      // 
      // cb_confirm
      // 
      this.cb_confirm.AutoSize = true;
      this.cb_confirm.Location = new System.Drawing.Point(470, 57);
      this.cb_confirm.Margin = new System.Windows.Forms.Padding(2);
      this.cb_confirm.Name = "cb_confirm";
      this.cb_confirm.Size = new System.Drawing.Size(61, 17);
      this.cb_confirm.TabIndex = 5;
      this.cb_confirm.Text = "Confirm";
      this.cb_confirm.UseVisualStyleBackColor = true;
      this.cb_confirm.CheckedChanged += new System.EventHandler(this.cb_confirm_CheckedChanged);
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
      this.panel1.Location = new System.Drawing.Point(198, 3);
      this.panel1.Name = "panel1";
      this.tlpControls.SetRowSpan(this.panel1, 7);
      this.panel1.Size = new System.Drawing.Size(1, 201);
      this.panel1.TabIndex = 46;
      // 
      // lstConnections
      // 
      this.lstConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.lstConnections, 6);
      this.lstConnections.DataSource = null;
      this.lstConnections.DisplayMember = "";
      this.lstConnections.Location = new System.Drawing.Point(4, 4);
      this.lstConnections.Margin = new System.Windows.Forms.Padding(4);
      this.lstConnections.Multiselect = false;
      this.lstConnections.Name = "lstConnections";
      this.tlpControls.SetRowSpan(this.lstConnections, 6);
      this.lstConnections.Size = new System.Drawing.Size(187, 165);
      this.lstConnections.TabIndex = 0;
      this.lstConnections.ValueMember = "";
      this.lstConnections.SelectionChanged += new System.EventHandler(this.lstConnections_SelectionChanged);
      this.lstConnections.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstConnections_MouseDoubleClick);
      // 
      // btnMoveDown
      // 
      this.btnMoveDown.AutoSize = true;
      this.btnMoveDown.BackColor = System.Drawing.Color.White;
      this.btnMoveDown.FlatAppearance.BorderSize = 0;
      this.btnMoveDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMoveDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnMoveDown.ForeColor = System.Drawing.Color.Black;
      this.btnMoveDown.Location = new System.Drawing.Point(163, 175);
      this.btnMoveDown.Margin = new System.Windows.Forms.Padding(2);
      this.btnMoveDown.Name = "btnMoveDown";
      this.btnMoveDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnMoveDown.Padding = new System.Windows.Forms.Padding(1);
      this.btnMoveDown.Size = new System.Drawing.Size(30, 30);
      this.btnMoveDown.TabIndex = 11;
      this.btnMoveDown.TabStop = false;
      this.btnMoveDown.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Icon;
      this.toolTipManager.SetToolTip(this.btnMoveDown, "Move Down");
      this.btnMoveDown.UseVisualStyleBackColor = false;
      this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
      // 
      // btnNew
      // 
      this.btnNew.AutoSize = true;
      this.btnNew.BackColor = System.Drawing.Color.White;
      this.btnNew.FlatAppearance.BorderSize = 0;
      this.btnNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnNew.ForeColor = System.Drawing.Color.Black;
      this.btnNew.Location = new System.Drawing.Point(2, 175);
      this.btnNew.Margin = new System.Windows.Forms.Padding(2);
      this.btnNew.Name = "btnNew";
      this.btnNew.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnNew.Padding = new System.Windows.Forms.Padding(1);
      this.btnNew.Size = new System.Drawing.Size(30, 30);
      this.btnNew.TabIndex = 7;
      this.btnNew.TabStop = false;
      this.btnNew.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Icon;
      this.toolTipManager.SetToolTip(this.btnNew, "Add New");
      this.btnNew.UseVisualStyleBackColor = false;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // btnMoveUp
      // 
      this.btnMoveUp.AutoSize = true;
      this.btnMoveUp.BackColor = System.Drawing.Color.White;
      this.btnMoveUp.FlatAppearance.BorderSize = 0;
      this.btnMoveUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMoveUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnMoveUp.ForeColor = System.Drawing.Color.Black;
      this.btnMoveUp.Location = new System.Drawing.Point(129, 175);
      this.btnMoveUp.Margin = new System.Windows.Forms.Padding(2);
      this.btnMoveUp.Name = "btnMoveUp";
      this.btnMoveUp.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnMoveUp.Padding = new System.Windows.Forms.Padding(1);
      this.btnMoveUp.Size = new System.Drawing.Size(30, 30);
      this.btnMoveUp.TabIndex = 10;
      this.btnMoveUp.TabStop = false;
      this.btnMoveUp.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Icon;
      this.toolTipManager.SetToolTip(this.btnMoveUp, "Move Up");
      this.btnMoveUp.UseVisualStyleBackColor = false;
      this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
      // 
      // btnCopy
      // 
      this.btnCopy.AutoSize = true;
      this.btnCopy.BackColor = System.Drawing.Color.White;
      this.btnCopy.FlatAppearance.BorderSize = 0;
      this.btnCopy.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCopy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnCopy.ForeColor = System.Drawing.Color.Black;
      this.btnCopy.Location = new System.Drawing.Point(36, 175);
      this.btnCopy.Margin = new System.Windows.Forms.Padding(2);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnCopy.Padding = new System.Windows.Forms.Padding(1);
      this.btnCopy.Size = new System.Drawing.Size(30, 30);
      this.btnCopy.TabIndex = 8;
      this.btnCopy.TabStop = false;
      this.btnCopy.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Icon;
      this.toolTipManager.SetToolTip(this.btnCopy, "Duplicate");
      this.btnCopy.UseVisualStyleBackColor = false;
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      // 
      // btnDelete
      // 
      this.btnDelete.AutoSize = true;
      this.btnDelete.BackColor = System.Drawing.Color.White;
      this.btnDelete.FlatAppearance.BorderSize = 0;
      this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnDelete.ForeColor = System.Drawing.Color.Black;
      this.btnDelete.Location = new System.Drawing.Point(70, 175);
      this.btnDelete.Margin = new System.Windows.Forms.Padding(2);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnDelete.Padding = new System.Windows.Forms.Padding(1);
      this.btnDelete.Size = new System.Drawing.Size(30, 30);
      this.btnDelete.TabIndex = 9;
      this.btnDelete.TabStop = false;
      this.btnDelete.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Icon;
      this.toolTipManager.SetToolTip(this.btnDelete, "Delete");
      this.btnDelete.UseVisualStyleBackColor = false;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // exploreButton
      // 
      this.exploreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.exploreButton.AutoSize = true;
      this.exploreButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.exploreButton.FlatAppearance.BorderSize = 0;
      this.exploreButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.exploreButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.exploreButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.exploreButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.exploreButton.ForeColor = System.Drawing.Color.Black;
      this.exploreButton.Location = new System.Drawing.Point(70, 3);
      this.exploreButton.Name = "exploreButton";
      this.exploreButton.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.exploreButton.Padding = new System.Windows.Forms.Padding(2);
      this.exploreButton.Size = new System.Drawing.Size(56, 31);
      this.exploreButton.TabIndex = 1;
      this.exploreButton.Text = "Explore";
      this.exploreButton.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.exploreButton.UseVisualStyleBackColor = false;
      this.exploreButton.Click += new System.EventHandler(this.exploreButton_Click);
      // 
      // btnTest
      // 
      this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnTest.AutoSize = true;
      this.btnTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.btnTest.ForeColor = System.Drawing.Color.Black;
      this.btnTest.Location = new System.Drawing.Point(3, 3);
      this.btnTest.Name = "btnTest";
      this.btnTest.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnTest.Padding = new System.Windows.Forms.Padding(2);
      this.btnTest.Size = new System.Drawing.Size(61, 31);
      this.btnTest.TabIndex = 0;
      this.btnTest.Text = "&Test";
      this.btnTest.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnTest.UseVisualStyleBackColor = true;
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // ConnectionEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tlpControls);
      this.MinimumSize = new System.Drawing.Size(425, 170);
      this.Name = "ConnectionEditor";
      this.Size = new System.Drawing.Size(603, 207);
      this.tlpControls.ResumeLayout(false);
      this.tlpControls.PerformLayout();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblDatabase;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.TextBox txtUser;
    private System.Windows.Forms.Label lblUser;
    private System.Windows.Forms.TextBox txtUrl;
    private System.Windows.Forms.Label lblUrl;
    private Controls.FlatButton btnTest;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label lblName;
    private MultiselectListBox lstConnections;
    private Controls.FlatButton btnDelete;
    private Controls.FlatButton btnNew;
    private Controls.FlatButton btnCopy;
    private Controls.FlatButton btnMoveUp;
    private Controls.FlatButton btnMoveDown;
    private System.Windows.Forms.TableLayoutPanel tlpControls;
    private System.Windows.Forms.ToolTip toolTipManager;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.ComboBox cmbDatabase;
    private System.Windows.Forms.Button btnColor;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cmbType;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cmbAuth;
    private Controls.FlatButton exploreButton;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.CheckBox cb_confirm;
    private System.Windows.Forms.Panel panel1;
  }
}
