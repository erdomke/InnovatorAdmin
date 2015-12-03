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
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.connectionEditor, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnOk, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(597, 235);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.AutoSize = true;
      this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnClose.ForeColor = System.Drawing.Color.Black;
      this.btnClose.Location = new System.Drawing.Point(519, 203);
      this.btnClose.MinimumSize = new System.Drawing.Size(75, 0);
      this.btnClose.Name = "btnClose";
      this.btnClose.Padding = new System.Windows.Forms.Padding(2);
      this.btnClose.Size = new System.Drawing.Size(75, 29);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "&Cancel";
      this.btnClose.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnClose.UseVisualStyleBackColor = false;
      // 
      // connectionEditor
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.connectionEditor, 2);
      this.connectionEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.connectionEditor.Location = new System.Drawing.Point(0, 0);
      this.connectionEditor.Margin = new System.Windows.Forms.Padding(0);
      this.connectionEditor.MinimumSize = new System.Drawing.Size(425, 170);
      this.connectionEditor.MultiSelect = false;
      this.connectionEditor.Name = "connectionEditor";
      this.connectionEditor.Size = new System.Drawing.Size(597, 200);
      this.connectionEditor.TabIndex = 0;
      // 
      // btnOk
      // 
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.AutoSize = true;
      this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(51)))));
      this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(3)))), ((int)(((byte)(32)))));
      this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(3)))), ((int)(((byte)(32)))));
      this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOk.ForeColor = System.Drawing.Color.White;
      this.btnOk.Location = new System.Drawing.Point(438, 203);
      this.btnOk.MinimumSize = new System.Drawing.Size(75, 0);
      this.btnOk.Name = "btnOk";
      this.btnOk.Padding = new System.Windows.Forms.Padding(2);
      this.btnOk.Size = new System.Drawing.Size(75, 29);
      this.btnOk.TabIndex = 1;
      this.btnOk.Text = "&OK";
      this.btnOk.Theme = InnovatorAdmin.Controls.FlatButtonTheme.Red;
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // ConnectionEditorForm
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(597, 235);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "ConnectionEditorForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Connection Editor";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private InnovatorAdmin.Controls.FlatButton btnClose;
    private ConnectionEditor connectionEditor;
    private InnovatorAdmin.Controls.FlatButton btnOk;
  }
}
