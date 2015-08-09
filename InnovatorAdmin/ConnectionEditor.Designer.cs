namespace Aras.Tools.InnovatorAdmin
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
      this.lstConnections = new MultiselectListBox();
      this.toolTipManager = new System.Windows.Forms.ToolTip(this.components);
      this.btnMoveDown = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnNew = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnMoveUp = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnCopy = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnDelete = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.tlpControls = new System.Windows.Forms.TableLayoutPanel();
      this.btnTest = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.lblMessage = new System.Windows.Forms.Label();
      this.cmbDatabase = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cboIomVersion = new System.Windows.Forms.ComboBox();
      this.tlpControls.SuspendLayout();
      this.SuspendLayout();
      //
      // lblDatabase
      //
      this.lblDatabase.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblDatabase.AutoSize = true;
      this.lblDatabase.Location = new System.Drawing.Point(197, 61);
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
      this.txtPassword.Location = new System.Drawing.Point(444, 84);
      this.txtPassword.MaxLength = 64;
      this.txtPassword.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.PasswordChar = '*';
      this.txtPassword.Size = new System.Drawing.Size(106, 22);
      this.txtPassword.TabIndex = 4;
      this.txtPassword.WordWrap = false;
      //
      // lblPassword
      //
      this.lblPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblPassword.AutoSize = true;
      this.lblPassword.Location = new System.Drawing.Point(382, 88);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new System.Drawing.Size(56, 13);
      this.lblPassword.TabIndex = 24;
      this.lblPassword.Text = "Password:";
      //
      // txtUser
      //
      this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtUser.Location = new System.Drawing.Point(271, 84);
      this.txtUser.MaximumSize = new System.Drawing.Size(160, 4);
      this.txtUser.MaxLength = 32;
      this.txtUser.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtUser.Name = "txtUser";
      this.txtUser.Size = new System.Drawing.Size(105, 22);
      this.txtUser.TabIndex = 3;
      //
      // lblUser
      //
      this.lblUser.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblUser.AutoSize = true;
      this.lblUser.Location = new System.Drawing.Point(197, 88);
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
      this.tlpControls.SetColumnSpan(this.txtUrl, 3);
      this.txtUrl.Location = new System.Drawing.Point(271, 31);
      this.txtUrl.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtUrl.Name = "txtUrl";
      this.txtUrl.Size = new System.Drawing.Size(279, 22);
      this.txtUrl.TabIndex = 1;
      //
      // lblUrl
      //
      this.lblUrl.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblUrl.AutoSize = true;
      this.lblUrl.Location = new System.Drawing.Point(197, 34);
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
      this.txtName.Location = new System.Drawing.Point(271, 3);
      this.txtName.MaximumSize = new System.Drawing.Size(260, 4);
      this.txtName.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(260, 22);
      this.txtName.TabIndex = 0;
      //
      // lblName
      //
      this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblName.AutoSize = true;
      this.lblName.Location = new System.Drawing.Point(197, 7);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(38, 13);
      this.lblName.TabIndex = 32;
      this.lblName.Text = "Name:";
      this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      //
      // clstConnections
      //
      this.lstConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.lstConnections, 6);
      this.lstConnections.Location = new System.Drawing.Point(3, 3);
      this.lstConnections.Name = "clstConnections";
      this.tlpControls.SetRowSpan(this.lstConnections, 6);
      this.lstConnections.Size = new System.Drawing.Size(188, 184);
      this.lstConnections.TabIndex = 6;
      this.lstConnections.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstConnections_MouseDown);
      this.lstConnections.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstConnections_MouseUp);
      //
      // btnMoveDown
      //
      this.btnMoveDown.AutoSize = true;
      this.btnMoveDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnMoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMoveDown.ForeColor = System.Drawing.Color.Black;
      this.btnMoveDown.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.down16;
      this.btnMoveDown.Location = new System.Drawing.Point(163, 193);
      this.btnMoveDown.Name = "btnMoveDown";
      this.btnMoveDown.Padding = new System.Windows.Forms.Padding(2);
      this.btnMoveDown.Size = new System.Drawing.Size(28, 28);
      this.btnMoveDown.TabIndex = 11;
      this.btnMoveDown.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.toolTipManager.SetToolTip(this.btnMoveDown, "Move Down");
      this.btnMoveDown.UseVisualStyleBackColor = true;
      this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
      //
      // btnNew
      //
      this.btnNew.AutoSize = true;
      this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnNew.ForeColor = System.Drawing.Color.Black;
      this.btnNew.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.add16;
      this.btnNew.Location = new System.Drawing.Point(3, 193);
      this.btnNew.Name = "btnNew";
      this.btnNew.Padding = new System.Windows.Forms.Padding(2);
      this.btnNew.Size = new System.Drawing.Size(28, 28);
      this.btnNew.TabIndex = 7;
      this.btnNew.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.toolTipManager.SetToolTip(this.btnNew, "Add New");
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      //
      // btnMoveUp
      //
      this.btnMoveUp.AutoSize = true;
      this.btnMoveUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnMoveUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnMoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnMoveUp.ForeColor = System.Drawing.Color.Black;
      this.btnMoveUp.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.up16;
      this.btnMoveUp.Location = new System.Drawing.Point(129, 193);
      this.btnMoveUp.Name = "btnMoveUp";
      this.btnMoveUp.Padding = new System.Windows.Forms.Padding(2);
      this.btnMoveUp.Size = new System.Drawing.Size(28, 28);
      this.btnMoveUp.TabIndex = 10;
      this.btnMoveUp.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.toolTipManager.SetToolTip(this.btnMoveUp, "Move Up");
      this.btnMoveUp.UseVisualStyleBackColor = true;
      this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
      //
      // btnCopy
      //
      this.btnCopy.AutoSize = true;
      this.btnCopy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCopy.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCopy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCopy.ForeColor = System.Drawing.Color.Black;
      this.btnCopy.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.copy16;
      this.btnCopy.Location = new System.Drawing.Point(37, 193);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Padding = new System.Windows.Forms.Padding(2);
      this.btnCopy.Size = new System.Drawing.Size(28, 28);
      this.btnCopy.TabIndex = 8;
      this.btnCopy.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.toolTipManager.SetToolTip(this.btnCopy, "Duplicate");
      this.btnCopy.UseVisualStyleBackColor = true;
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      //
      // btnDelete
      //
      this.btnDelete.AutoSize = true;
      this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnDelete.ForeColor = System.Drawing.Color.Black;
      this.btnDelete.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.remove16;
      this.btnDelete.Location = new System.Drawing.Point(71, 193);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
      this.btnDelete.Size = new System.Drawing.Size(28, 28);
      this.btnDelete.TabIndex = 9;
      this.btnDelete.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.toolTipManager.SetToolTip(this.btnDelete, "Delete");
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      //
      // tlpControls
      //
      this.tlpControls.ColumnCount = 10;
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
      this.tlpControls.Controls.Add(this.lstConnections, 0, 0);
      this.tlpControls.Controls.Add(this.btnMoveDown, 5, 6);
      this.tlpControls.Controls.Add(this.txtName, 7, 0);
      this.tlpControls.Controls.Add(this.txtPassword, 9, 3);
      this.tlpControls.Controls.Add(this.lblPassword, 8, 3);
      this.tlpControls.Controls.Add(this.btnNew, 0, 6);
      this.tlpControls.Controls.Add(this.txtUser, 7, 3);
      this.tlpControls.Controls.Add(this.btnTest, 9, 5);
      this.tlpControls.Controls.Add(this.lblName, 6, 0);
      this.tlpControls.Controls.Add(this.btnMoveUp, 4, 6);
      this.tlpControls.Controls.Add(this.txtUrl, 7, 1);
      this.tlpControls.Controls.Add(this.btnCopy, 1, 6);
      this.tlpControls.Controls.Add(this.lblDatabase, 6, 2);
      this.tlpControls.Controls.Add(this.btnDelete, 2, 6);
      this.tlpControls.Controls.Add(this.lblUrl, 6, 1);
      this.tlpControls.Controls.Add(this.lblUser, 6, 3);
      this.tlpControls.Controls.Add(this.lblMessage, 6, 5);
      this.tlpControls.Controls.Add(this.cmbDatabase, 7, 2);
      this.tlpControls.Controls.Add(this.label1, 6, 4);
      this.tlpControls.Controls.Add(this.cboIomVersion, 7, 4);
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
      this.tlpControls.Size = new System.Drawing.Size(553, 224);
      this.tlpControls.TabIndex = 41;
      //
      // btnTest
      //
      this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnTest.AutoSize = true;
      this.btnTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnTest.ForeColor = System.Drawing.Color.Black;
      this.btnTest.Location = new System.Drawing.Point(479, 139);
      this.btnTest.Name = "btnTest";
      this.btnTest.Padding = new System.Windows.Forms.Padding(2);
      this.btnTest.Size = new System.Drawing.Size(71, 29);
      this.btnTest.TabIndex = 5;
      this.btnTest.Text = "&Test";
      this.btnTest.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnTest.UseVisualStyleBackColor = true;
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      //
      // lblMessage
      //
      this.lblMessage.AutoSize = true;
      this.tlpControls.SetColumnSpan(this.lblMessage, 3);
      this.lblMessage.Location = new System.Drawing.Point(200, 142);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(6);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 13);
      this.lblMessage.TabIndex = 41;
      //
      // cmbDatabase
      //
      this.cmbDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.cmbDatabase, 3);
      this.cmbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbDatabase.FormattingEnabled = true;
      this.cmbDatabase.Location = new System.Drawing.Point(271, 57);
      this.cmbDatabase.Name = "cmbDatabase";
      this.cmbDatabase.Size = new System.Drawing.Size(279, 21);
      this.cmbDatabase.TabIndex = 2;
      this.cmbDatabase.DropDown += new System.EventHandler(this.cmbDatabase_DropDown);
      //
      // label1
      //
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(197, 112);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(68, 13);
      this.label1.TabIndex = 42;
      this.label1.Text = "IOM Version:";
      //
      // cboIomVersion
      //
      this.cboIomVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboIomVersion.FormattingEnabled = true;
      this.cboIomVersion.Location = new System.Drawing.Point(271, 112);
      this.cboIomVersion.Name = "cboIomVersion";
      this.cboIomVersion.Size = new System.Drawing.Size(105, 21);
      this.cboIomVersion.TabIndex = 43;
      //
      // ConnectionEditor
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tlpControls);
      this.MinimumSize = new System.Drawing.Size(425, 170);
      this.Name = "ConnectionEditor";
      this.Size = new System.Drawing.Size(553, 224);
      this.tlpControls.ResumeLayout(false);
      this.tlpControls.PerformLayout();
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
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cboIomVersion;
  }
}
