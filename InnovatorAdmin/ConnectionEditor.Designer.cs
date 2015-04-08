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
      this.btnTest = new System.Windows.Forms.Button();
      this.txtName = new System.Windows.Forms.TextBox();
      this.lblName = new System.Windows.Forms.Label();
      this.lstConnections = new System.Windows.Forms.CheckedListBox();
      this.btnMoveDown = new System.Windows.Forms.Button();
      this.btnMoveUp = new System.Windows.Forms.Button();
      this.btnCopy = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.toolTipManager = new System.Windows.Forms.ToolTip(this.components);
      this.tlpControls = new System.Windows.Forms.TableLayoutPanel();
      this.lblMessage = new System.Windows.Forms.Label();
      this.cmbDatabase = new System.Windows.Forms.ComboBox();
      this.tlpControls.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblDatabase
      // 
      this.lblDatabase.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblDatabase.AutoSize = true;
      this.lblDatabase.Location = new System.Drawing.Point(185, 61);
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
      this.txtPassword.Location = new System.Drawing.Point(432, 84);
      this.txtPassword.MaxLength = 64;
      this.txtPassword.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.PasswordChar = '*';
      this.txtPassword.Size = new System.Drawing.Size(118, 22);
      this.txtPassword.TabIndex = 4;
      this.txtPassword.WordWrap = false;
      // 
      // lblPassword
      // 
      this.lblPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblPassword.AutoSize = true;
      this.lblPassword.Location = new System.Drawing.Point(370, 88);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new System.Drawing.Size(56, 13);
      this.lblPassword.TabIndex = 24;
      this.lblPassword.Text = "Password:";
      // 
      // txtUser
      // 
      this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtUser.Location = new System.Drawing.Point(247, 84);
      this.txtUser.MaximumSize = new System.Drawing.Size(160, 4);
      this.txtUser.MaxLength = 32;
      this.txtUser.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtUser.Name = "txtUser";
      this.txtUser.Size = new System.Drawing.Size(117, 22);
      this.txtUser.TabIndex = 3;
      // 
      // lblUser
      // 
      this.lblUser.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblUser.AutoSize = true;
      this.lblUser.Location = new System.Drawing.Point(185, 88);
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
      this.txtUrl.Location = new System.Drawing.Point(247, 29);
      this.txtUrl.MinimumSize = new System.Drawing.Size(4, 22);
      this.txtUrl.Name = "txtUrl";
      this.txtUrl.Size = new System.Drawing.Size(303, 22);
      this.txtUrl.TabIndex = 1;
      // 
      // lblUrl
      // 
      this.lblUrl.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblUrl.AutoSize = true;
      this.lblUrl.Location = new System.Drawing.Point(185, 33);
      this.lblUrl.Name = "lblUrl";
      this.lblUrl.Size = new System.Drawing.Size(32, 13);
      this.lblUrl.TabIndex = 20;
      this.lblUrl.Text = "URL:";
      this.lblUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // btnTest
      // 
      this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnTest.Location = new System.Drawing.Point(479, 112);
      this.btnTest.Name = "btnTest";
      this.btnTest.Size = new System.Drawing.Size(71, 23);
      this.btnTest.TabIndex = 5;
      this.btnTest.Text = "&Test";
      this.btnTest.UseVisualStyleBackColor = true;
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // txtName
      // 
      this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.txtName, 3);
      this.txtName.Location = new System.Drawing.Point(247, 3);
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
      this.lblName.Location = new System.Drawing.Point(185, 6);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(38, 13);
      this.lblName.TabIndex = 32;
      this.lblName.Text = "Name:";
      this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lstConnections
      // 
      this.lstConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tlpControls.SetColumnSpan(this.lstConnections, 6);
      this.lstConnections.FormattingEnabled = true;
      this.lstConnections.Location = new System.Drawing.Point(3, 3);
      this.lstConnections.Name = "lstConnections";
      this.tlpControls.SetRowSpan(this.lstConnections, 5);
      this.lstConnections.Size = new System.Drawing.Size(176, 184);
      this.lstConnections.TabIndex = 6;
      this.lstConnections.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstConnections_ItemCheck);
      this.lstConnections.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstConnections_MouseDown);
      this.lstConnections.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstConnections_MouseUp);
      // 
      // btnMoveDown
      // 
      this.btnMoveDown.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.down16;
      this.btnMoveDown.Location = new System.Drawing.Point(154, 196);
      this.btnMoveDown.Name = "btnMoveDown";
      this.btnMoveDown.Size = new System.Drawing.Size(25, 25);
      this.btnMoveDown.TabIndex = 11;
      this.toolTipManager.SetToolTip(this.btnMoveDown, "Move Down");
      this.btnMoveDown.UseVisualStyleBackColor = true;
      this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
      // 
      // btnMoveUp
      // 
      this.btnMoveUp.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.up16;
      this.btnMoveUp.Location = new System.Drawing.Point(123, 196);
      this.btnMoveUp.Name = "btnMoveUp";
      this.btnMoveUp.Size = new System.Drawing.Size(25, 25);
      this.btnMoveUp.TabIndex = 10;
      this.toolTipManager.SetToolTip(this.btnMoveUp, "Move Up");
      this.btnMoveUp.UseVisualStyleBackColor = true;
      this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
      // 
      // btnCopy
      // 
      this.btnCopy.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.copy16;
      this.btnCopy.Location = new System.Drawing.Point(34, 196);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Size = new System.Drawing.Size(25, 25);
      this.btnCopy.TabIndex = 8;
      this.toolTipManager.SetToolTip(this.btnCopy, "Duplicate");
      this.btnCopy.UseVisualStyleBackColor = true;
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      // 
      // btnNew
      // 
      this.btnNew.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.add16;
      this.btnNew.Location = new System.Drawing.Point(3, 196);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(25, 25);
      this.btnNew.TabIndex = 7;
      this.toolTipManager.SetToolTip(this.btnNew, "Add New");
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // btnDelete
      // 
      this.btnDelete.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.remove16;
      this.btnDelete.Location = new System.Drawing.Point(65, 196);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(25, 25);
      this.btnDelete.TabIndex = 9;
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
      this.tlpControls.Controls.Add(this.btnMoveDown, 5, 5);
      this.tlpControls.Controls.Add(this.txtName, 7, 0);
      this.tlpControls.Controls.Add(this.txtPassword, 9, 3);
      this.tlpControls.Controls.Add(this.lblPassword, 8, 3);
      this.tlpControls.Controls.Add(this.btnNew, 0, 5);
      this.tlpControls.Controls.Add(this.txtUser, 7, 3);
      this.tlpControls.Controls.Add(this.btnTest, 9, 4);
      this.tlpControls.Controls.Add(this.lblName, 6, 0);
      this.tlpControls.Controls.Add(this.btnMoveUp, 4, 5);
      this.tlpControls.Controls.Add(this.txtUrl, 7, 1);
      this.tlpControls.Controls.Add(this.btnCopy, 1, 5);
      this.tlpControls.Controls.Add(this.lblDatabase, 6, 2);
      this.tlpControls.Controls.Add(this.btnDelete, 2, 5);
      this.tlpControls.Controls.Add(this.lblUrl, 6, 1);
      this.tlpControls.Controls.Add(this.lblUser, 6, 3);
      this.tlpControls.Controls.Add(this.lblMessage, 6, 4);
      this.tlpControls.Controls.Add(this.cmbDatabase, 7, 2);
      this.tlpControls.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpControls.Location = new System.Drawing.Point(0, 0);
      this.tlpControls.Name = "tlpControls";
      this.tlpControls.RowCount = 6;
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpControls.Size = new System.Drawing.Size(553, 224);
      this.tlpControls.TabIndex = 41;
      // 
      // lblMessage
      // 
      this.lblMessage.AutoSize = true;
      this.tlpControls.SetColumnSpan(this.lblMessage, 3);
      this.lblMessage.Location = new System.Drawing.Point(188, 115);
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
      this.cmbDatabase.Location = new System.Drawing.Point(247, 57);
      this.cmbDatabase.Name = "cmbDatabase";
      this.cmbDatabase.Size = new System.Drawing.Size(303, 21);
      this.cmbDatabase.TabIndex = 2;
      this.cmbDatabase.DropDown += new System.EventHandler(this.cmbDatabase_DropDown);
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
    private System.Windows.Forms.Button btnTest;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.CheckedListBox lstConnections;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Button btnCopy;
    private System.Windows.Forms.Button btnMoveUp;
    private System.Windows.Forms.Button btnMoveDown;
    private System.Windows.Forms.TableLayoutPanel tlpControls;
    private System.Windows.Forms.ToolTip toolTipManager;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.ComboBox cmbDatabase;
  }
}