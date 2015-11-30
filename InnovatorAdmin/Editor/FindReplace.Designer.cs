namespace Aras.Tools.InnovatorAdmin.Editor
{
  partial class FindReplace
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
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
      this.chkNormal = new System.Windows.Forms.CheckBox();
      this.chkExtended = new System.Windows.Forms.CheckBox();
      this.chkRegExp = new System.Windows.Forms.CheckBox();
      this.chkXPath = new System.Windows.Forms.CheckBox();
      this.txtFind = new System.Windows.Forms.TextBox();
      this.txtReplace = new System.Windows.Forms.TextBox();
      this.btnFind = new System.Windows.Forms.Button();
      this.btnReplaceNext = new System.Windows.Forms.Button();
      this.btnReplaceAll = new System.Windows.Forms.Button();
      this.btnShowReplace = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.tableLayoutPanel1.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      //
      // tableLayoutPanel1
      //
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtFind, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtReplace, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnFind, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnReplaceNext, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnReplaceAll, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnShowReplace, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(352, 93);
      this.tableLayoutPanel1.TabIndex = 3;
      //
      // flowLayoutPanel1
      //
      this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 4);
      this.flowLayoutPanel1.Controls.Add(this.chkCaseSensitive);
      this.flowLayoutPanel1.Controls.Add(this.chkNormal);
      this.flowLayoutPanel1.Controls.Add(this.chkExtended);
      this.flowLayoutPanel1.Controls.Add(this.chkRegExp);
      this.flowLayoutPanel1.Controls.Add(this.chkXPath);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 58);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(352, 29);
      this.flowLayoutPanel1.TabIndex = 0;
      //
      // chkCaseSensitive
      //
      this.chkCaseSensitive.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkCaseSensitive.AutoSize = true;
      this.chkCaseSensitive.FlatAppearance.BorderSize = 0;
      this.chkCaseSensitive.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkCaseSensitive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkCaseSensitive.Location = new System.Drawing.Point(3, 3);
      this.chkCaseSensitive.Name = "chkCaseSensitive";
      this.chkCaseSensitive.Size = new System.Drawing.Size(30, 23);
      this.chkCaseSensitive.TabIndex = 5;
      this.chkCaseSensitive.Text = "Aa";
      this.chkCaseSensitive.UseVisualStyleBackColor = true;
      //
      // chkNormal
      //
      this.chkNormal.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkNormal.AutoSize = true;
      this.chkNormal.Checked = true;
      this.chkNormal.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkNormal.FlatAppearance.BorderSize = 0;
      this.chkNormal.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkNormal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkNormal.Location = new System.Drawing.Point(39, 3);
      this.chkNormal.Name = "chkNormal";
      this.chkNormal.Size = new System.Drawing.Size(35, 23);
      this.chkNormal.TabIndex = 6;
      this.chkNormal.Text = "abc";
      this.chkNormal.UseVisualStyleBackColor = true;
      this.chkNormal.CheckedChanged += new System.EventHandler(this.chkNormal_CheckedChanged);
      //
      // chkExtended
      //
      this.chkExtended.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkExtended.AutoSize = true;
      this.chkExtended.FlatAppearance.BorderSize = 0;
      this.chkExtended.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkExtended.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkExtended.Location = new System.Drawing.Point(80, 3);
      this.chkExtended.Name = "chkExtended";
      this.chkExtended.Size = new System.Drawing.Size(36, 23);
      this.chkExtended.TabIndex = 7;
      this.chkExtended.Text = "\\r\\n";
      this.chkExtended.UseVisualStyleBackColor = true;
      this.chkExtended.CheckedChanged += new System.EventHandler(this.chkExtended_CheckedChanged);
      //
      // chkRegExp
      //
      this.chkRegExp.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkRegExp.AutoSize = true;
      this.chkRegExp.FlatAppearance.BorderSize = 0;
      this.chkRegExp.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkRegExp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkRegExp.Location = new System.Drawing.Point(122, 3);
      this.chkRegExp.Name = "chkRegExp";
      this.chkRegExp.Size = new System.Drawing.Size(24, 23);
      this.chkRegExp.TabIndex = 8;
      this.chkRegExp.Text = ".*";
      this.chkRegExp.UseVisualStyleBackColor = true;
      this.chkRegExp.CheckedChanged += new System.EventHandler(this.chkRegExp_CheckedChanged);
      //
      // chkXPath
      //
      this.chkXPath.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkXPath.AutoSize = true;
      this.chkXPath.FlatAppearance.BorderSize = 0;
      this.chkXPath.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkXPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkXPath.Location = new System.Drawing.Point(152, 3);
      this.chkXPath.Name = "chkXPath";
      this.chkXPath.Size = new System.Drawing.Size(51, 23);
      this.chkXPath.TabIndex = 9;
      this.chkXPath.Text = "/e[@a]";
      this.chkXPath.UseVisualStyleBackColor = true;
      this.chkXPath.CheckedChanged += new System.EventHandler(this.chkXPath_CheckedChanged);
      //
      // txtFind
      //
      this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtFind.Location = new System.Drawing.Point(38, 3);
      this.txtFind.Name = "txtFind";
      this.txtFind.Size = new System.Drawing.Size(247, 20);
      this.txtFind.TabIndex = 1;
      this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
      this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
      //
      // txtReplace
      //
      this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtReplace.Location = new System.Drawing.Point(38, 32);
      this.txtReplace.Name = "txtReplace";
      this.txtReplace.Size = new System.Drawing.Size(247, 20);
      this.txtReplace.TabIndex = 2;
      //
      // btnFind
      //
      this.btnFind.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.btnFind, 2);
      this.btnFind.FlatAppearance.BorderSize = 0;
      this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnFind.Location = new System.Drawing.Point(291, 3);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(29, 23);
      this.btnFind.TabIndex = 3;
      this.btnFind.Text = "→";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      //
      // btnReplaceNext
      //
      this.btnReplaceNext.AutoSize = true;
      this.btnReplaceNext.FlatAppearance.BorderSize = 0;
      this.btnReplaceNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceNext.Location = new System.Drawing.Point(291, 32);
      this.btnReplaceNext.Name = "btnReplaceNext";
      this.btnReplaceNext.Size = new System.Drawing.Size(23, 23);
      this.btnReplaceNext.TabIndex = 4;
      this.btnReplaceNext.Text = ">";
      this.btnReplaceNext.UseVisualStyleBackColor = true;
      //
      // btnReplaceAll
      //
      this.btnReplaceAll.AutoSize = true;
      this.btnReplaceAll.FlatAppearance.BorderSize = 0;
      this.btnReplaceAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceAll.Location = new System.Drawing.Point(320, 32);
      this.btnReplaceAll.Name = "btnReplaceAll";
      this.btnReplaceAll.Size = new System.Drawing.Size(29, 23);
      this.btnReplaceAll.TabIndex = 5;
      this.btnReplaceAll.Text = ">>";
      this.btnReplaceAll.UseVisualStyleBackColor = true;
      //
      // btnShowReplace
      //
      this.btnShowReplace.AutoSize = true;
      this.btnShowReplace.FlatAppearance.BorderSize = 0;
      this.btnShowReplace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnShowReplace.Location = new System.Drawing.Point(3, 3);
      this.btnShowReplace.Name = "btnShowReplace";
      this.btnShowReplace.Size = new System.Drawing.Size(29, 23);
      this.btnShowReplace.TabIndex = 6;
      this.btnShowReplace.Text = "▼";
      this.btnShowReplace.UseVisualStyleBackColor = true;
      this.btnShowReplace.Click += new System.EventHandler(this.btnShowReplace_Click);
      //
      // FindReplace
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(352, 56);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "FindReplace";
      this.Text = "Find/Replace";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.TextBox txtFind;
    private System.Windows.Forms.TextBox txtReplace;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.Button btnReplaceNext;
    private System.Windows.Forms.Button btnReplaceAll;
    private System.Windows.Forms.Button btnShowReplace;
    private System.Windows.Forms.CheckBox chkCaseSensitive;
    private System.Windows.Forms.CheckBox chkNormal;
    private System.Windows.Forms.CheckBox chkExtended;
    private System.Windows.Forms.CheckBox chkRegExp;
    private System.Windows.Forms.CheckBox chkXPath;

  }
}
