namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class FilterSelect<T>
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
      this.listValues = new System.Windows.Forms.ListBox();
      this.txtFilter = new System.Windows.Forms.TextBox();
      this.btnOK = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnCancel = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.lblMessage = new System.Windows.Forms.Label();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.7384F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.listValues, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtFilter, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.lblMessage, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 262);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // listValues
      // 
      this.listValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.listValues, 2);
      this.listValues.FormattingEnabled = true;
      this.listValues.Location = new System.Drawing.Point(3, 48);
      this.listValues.Name = "listValues";
      this.listValues.Size = new System.Drawing.Size(278, 173);
      this.listValues.TabIndex = 1;
      this.listValues.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listValues_KeyDown);
      this.listValues.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listValues_MouseDoubleClick);
      // 
      // txtFilter
      // 
      this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.txtFilter, 2);
      this.txtFilter.Location = new System.Drawing.Point(3, 22);
      this.txtFilter.Name = "txtFilter";
      this.txtFilter.Size = new System.Drawing.Size(278, 20);
      this.txtFilter.TabIndex = 0;
      this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
      this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOK.AutoSize = true;
      this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOK.ForeColor = System.Drawing.Color.Black;
      this.btnOK.Location = new System.Drawing.Point(147, 230);
      this.btnOK.Name = "btnOK";
      this.btnOK.Padding = new System.Windows.Forms.Padding(2);
      this.btnOK.Size = new System.Drawing.Size(65, 29);
      this.btnOK.TabIndex = 2;
      this.btnOK.Text = "&OK";
      this.btnOK.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnOK.UseVisualStyleBackColor = true;
      // 
      // btnCancel
      // 
      this.btnCancel.AutoSize = true;
      this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCancel.ForeColor = System.Drawing.Color.Black;
      this.btnCancel.Location = new System.Drawing.Point(218, 230);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Padding = new System.Windows.Forms.Padding(2);
      this.btnCancel.Size = new System.Drawing.Size(63, 29);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // lblMessage
      // 
      this.lblMessage.AutoSize = true;
      this.lblMessage.Location = new System.Drawing.Point(3, 3);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(3);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 13);
      this.lblMessage.TabIndex = 4;
      // 
      // FilterSelect
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "FilterSelect";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Select";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.ListBox listValues;
    private System.Windows.Forms.TextBox txtFilter;
    private FlatButton btnOK;
    private FlatButton btnCancel;
    private System.Windows.Forms.Label lblMessage;
  }
}
