namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class RecentlyModifiedSearch
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
      this.txtModifiedBy = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.lstItemTypes = new System.Windows.Forms.ListBox();
      this.btnAddItemTypes = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnOk = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnCancel = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.nudDays = new System.Windows.Forms.NumericUpDown();
      this.btnRemove = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudDays)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.txtModifiedBy, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label3, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.lstItemTypes, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnAddItemTypes, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnOk, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 4);
      this.tableLayoutPanel1.Controls.Add(this.nudDays, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnRemove, 2, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(288, 282);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // txtModifiedBy
      // 
      this.txtModifiedBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.txtModifiedBy, 2);
      this.txtModifiedBy.Location = new System.Drawing.Point(71, 3);
      this.txtModifiedBy.Name = "txtModifiedBy";
      this.txtModifiedBy.Size = new System.Drawing.Size(214, 20);
      this.txtModifiedBy.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 3);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(62, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Modified By";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 29);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(53, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "In the last";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(210, 29);
      this.label3.Margin = new System.Windows.Forms.Padding(3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(29, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "days";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 55);
      this.label4.Margin = new System.Windows.Forms.Padding(3);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(59, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "Item Types";
      // 
      // lstItemTypes
      // 
      this.lstItemTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lstItemTypes.DisplayMember = "KeyedName";
      this.lstItemTypes.FormattingEnabled = true;
      this.lstItemTypes.Location = new System.Drawing.Point(71, 55);
      this.lstItemTypes.Name = "lstItemTypes";
      this.tableLayoutPanel1.SetRowSpan(this.lstItemTypes, 2);
      this.lstItemTypes.Size = new System.Drawing.Size(133, 186);
      this.lstItemTypes.TabIndex = 5;
      // 
      // btnAddItemTypes
      // 
      this.btnAddItemTypes.AutoSize = true;
      this.btnAddItemTypes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnAddItemTypes.FlatAppearance.BorderSize = 0;
      this.btnAddItemTypes.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnAddItemTypes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnAddItemTypes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnAddItemTypes.ForeColor = System.Drawing.Color.Black;
      this.btnAddItemTypes.Location = new System.Drawing.Point(210, 55);
      this.btnAddItemTypes.Name = "btnAddItemTypes";
      this.btnAddItemTypes.Padding = new System.Windows.Forms.Padding(2);
      this.btnAddItemTypes.Size = new System.Drawing.Size(75, 27);
      this.btnAddItemTypes.TabIndex = 6;
      this.btnAddItemTypes.Text = "&Add";
      this.btnAddItemTypes.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnAddItemTypes.UseVisualStyleBackColor = false;
      this.btnAddItemTypes.Click += new System.EventHandler(this.btnAddItemTypes_Click);
      // 
      // btnOk
      // 
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.AutoSize = true;
      this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOk.FlatAppearance.BorderSize = 0;
      this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOk.ForeColor = System.Drawing.Color.Black;
      this.btnOk.Location = new System.Drawing.Point(129, 252);
      this.btnOk.Name = "btnOk";
      this.btnOk.Padding = new System.Windows.Forms.Padding(2);
      this.btnOk.Size = new System.Drawing.Size(75, 27);
      this.btnOk.TabIndex = 7;
      this.btnOk.Text = "&OK";
      this.btnOk.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.AutoSize = true;
      this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderSize = 0;
      this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCancel.ForeColor = System.Drawing.Color.Black;
      this.btnCancel.Location = new System.Drawing.Point(210, 252);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Padding = new System.Windows.Forms.Padding(2);
      this.btnCancel.Size = new System.Drawing.Size(75, 27);
      this.btnCancel.TabIndex = 8;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnCancel.UseVisualStyleBackColor = false;
      // 
      // nudDays
      // 
      this.nudDays.Location = new System.Drawing.Point(71, 29);
      this.nudDays.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.nudDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.nudDays.Name = "nudDays";
      this.nudDays.Size = new System.Drawing.Size(51, 20);
      this.nudDays.TabIndex = 9;
      this.nudDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // btnRemove
      // 
      this.btnRemove.AutoSize = true;
      this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnRemove.FlatAppearance.BorderSize = 0;
      this.btnRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnRemove.ForeColor = System.Drawing.Color.Black;
      this.btnRemove.Location = new System.Drawing.Point(210, 88);
      this.btnRemove.Name = "btnRemove";
      this.btnRemove.Padding = new System.Windows.Forms.Padding(2);
      this.btnRemove.Size = new System.Drawing.Size(75, 27);
      this.btnRemove.TabIndex = 10;
      this.btnRemove.Text = "&Remove";
      this.btnRemove.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnRemove.UseVisualStyleBackColor = false;
      this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
      // 
      // RecentlyModifiedSearch
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(288, 282);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "RecentlyModifiedSearch";
      this.Text = "Recently Modified Search";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudDays)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TextBox txtModifiedBy;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ListBox lstItemTypes;
    private FlatButton btnAddItemTypes;
    private FlatButton btnOk;
    private FlatButton btnCancel;
    private System.Windows.Forms.NumericUpDown nudDays;
    private FlatButton btnRemove;
  }
}