namespace Aras.Tools.InnovatorAdmin
{
  partial class ParameterWindow
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.lstItems = new System.Windows.Forms.ListBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.txtValue = new System.Windows.Forms.TextBox();
      this.txtName = new System.Windows.Forms.TextBox();
      this.cmbType = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.btnCancel = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnOk = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.bs = new System.Windows.Forms.BindingSource(this.components);
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.bs)).BeginInit();
      this.SuspendLayout();
      //
      // tableLayoutPanel1
      //
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
      this.tableLayoutPanel1.Controls.Add(this.lstItems, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(365, 192);
      this.tableLayoutPanel1.TabIndex = 0;
      //
      // lstItems
      //
      this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lstItems.FormattingEnabled = true;
      this.lstItems.Location = new System.Drawing.Point(3, 3);
      this.lstItems.Name = "lstItems";
      this.lstItems.Size = new System.Drawing.Size(140, 153);
      this.lstItems.TabIndex = 0;
      //
      // tableLayoutPanel2
      //
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.txtValue, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtName, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.cmbType, 1, 2);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(149, 3);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 3;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.Size = new System.Drawing.Size(213, 153);
      this.tableLayoutPanel2.TabIndex = 1;
      //
      // txtValue
      //
      this.txtValue.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtValue.Location = new System.Drawing.Point(47, 29);
      this.txtValue.Multiline = true;
      this.txtValue.Name = "txtValue";
      this.txtValue.Size = new System.Drawing.Size(163, 94);
      this.txtValue.TabIndex = 2;
      //
      // txtName
      //
      this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtName.Location = new System.Drawing.Point(47, 3);
      this.txtName.Name = "txtName";
      this.txtName.ReadOnly = true;
      this.txtName.Size = new System.Drawing.Size(163, 20);
      this.txtName.TabIndex = 3;
      //
      // cmbType
      //
      this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbType.FormattingEnabled = true;
      this.cmbType.Location = new System.Drawing.Point(47, 129);
      this.cmbType.Name = "cmbType";
      this.cmbType.Size = new System.Drawing.Size(121, 21);
      this.cmbType.TabIndex = 4;
      //
      // label1
      //
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 3);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Name:";
      //
      // label2
      //
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 29);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(37, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Value:";
      //
      // label3
      //
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 129);
      this.label3.Margin = new System.Windows.Forms.Padding(3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(34, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "Type:";
      //
      // flowLayoutPanel1
      //
      this.flowLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
      this.flowLayoutPanel1.Controls.Add(this.btnCancel);
      this.flowLayoutPanel1.Controls.Add(this.btnOk);
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 159);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(365, 33);
      this.flowLayoutPanel1.TabIndex = 2;
      //
      // btnCancel
      //
      this.btnCancel.AutoSize = true;
      this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCancel.FlatAppearance.BorderSize = 0;
      this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCancel.ForeColor = System.Drawing.Color.Black;
      this.btnCancel.Location = new System.Drawing.Point(287, 3);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Padding = new System.Windows.Forms.Padding(2);
      this.btnCancel.Size = new System.Drawing.Size(75, 27);
      this.btnCancel.TabIndex = 0;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      //
      // btnOk
      //
      this.btnOk.AutoSize = true;
      this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOk.FlatAppearance.BorderSize = 0;
      this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOk.ForeColor = System.Drawing.Color.Black;
      this.btnOk.Location = new System.Drawing.Point(206, 3);
      this.btnOk.Name = "btnOk";
      this.btnOk.Padding = new System.Windows.Forms.Padding(2);
      this.btnOk.Size = new System.Drawing.Size(75, 27);
      this.btnOk.TabIndex = 1;
      this.btnOk.Text = "&OK";
      this.btnOk.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      //
      // ParameterWindow
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(365, 192);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "ParameterWindow";
      this.Text = "Enter Parameters";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.bs)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.ListBox lstItems;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TextBox txtValue;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.ComboBox cmbType;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private Controls.FlatButton btnCancel;
    private Controls.FlatButton btnOk;
    private System.Windows.Forms.BindingSource bs;
  }
}
