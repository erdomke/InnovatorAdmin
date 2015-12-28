namespace InnovatorAdmin.Editor
{
  partial class FullEditor
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.findReplacePanel = new System.Windows.Forms.TableLayoutPanel();
      this.txtFind = new InnovatorAdmin.Editor.EditorWinForm();
      this.txtReplace = new InnovatorAdmin.Editor.EditorWinForm();
      this.btnReplaceNext = new System.Windows.Forms.Button();
      this.btnReplaceAll = new System.Windows.Forms.Button();
      this.btnFind = new System.Windows.Forms.Button();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.chkNormal = new System.Windows.Forms.CheckBox();
      this.chkExtended = new System.Windows.Forms.CheckBox();
      this.chkRegExp = new System.Windows.Forms.CheckBox();
      this.chkXPath = new System.Windows.Forms.CheckBox();
      this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
      this.editor = new InnovatorAdmin.Editor.EditorWinForm();
      this.btnClose = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.findReplacePanel.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.findReplacePanel, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.editor, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(637, 324);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // findReplacePanel
      // 
      this.findReplacePanel.ColumnCount = 8;
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.Controls.Add(this.txtFind, 1, 0);
      this.findReplacePanel.Controls.Add(this.txtReplace, 4, 0);
      this.findReplacePanel.Controls.Add(this.btnReplaceNext, 5, 0);
      this.findReplacePanel.Controls.Add(this.btnReplaceAll, 6, 0);
      this.findReplacePanel.Controls.Add(this.btnFind, 3, 0);
      this.findReplacePanel.Controls.Add(this.flowLayoutPanel1, 2, 0);
      this.findReplacePanel.Controls.Add(this.btnClose, 7, 0);
      this.findReplacePanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.findReplacePanel.Location = new System.Drawing.Point(0, 0);
      this.findReplacePanel.Margin = new System.Windows.Forms.Padding(0);
      this.findReplacePanel.Name = "findReplacePanel";
      this.findReplacePanel.RowCount = 1;
      this.findReplacePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.findReplacePanel.Size = new System.Drawing.Size(637, 28);
      this.findReplacePanel.TabIndex = 4;
      // 
      // txtFind
      // 
      this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtFind.Location = new System.Drawing.Point(22, 4);
      this.txtFind.Margin = new System.Windows.Forms.Padding(22, 3, 3, 3);
      this.txtFind.Name = "txtFind";
      this.txtFind.PlaceholderText = "Find...";
      this.txtFind.ReadOnly = false;
      this.txtFind.ShowLineNumbers = false;
      this.txtFind.ShowScrollbars = false;
      this.txtFind.SingleLine = false;
      this.txtFind.Size = new System.Drawing.Size(134, 22);
      this.txtFind.TabIndex = 0;
      // 
      // txtReplace
      // 
      this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.txtReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtReplace.Location = new System.Drawing.Point(384, 5);
      this.txtReplace.Name = "txtReplace";
      this.txtReplace.PlaceholderText = "Replace with...";
      this.txtReplace.ReadOnly = false;
      this.txtReplace.ShowLineNumbers = false;
      this.txtReplace.ShowScrollbars = false;
      this.txtReplace.SingleLine = true;
      this.txtReplace.Size = new System.Drawing.Size(153, 20);
      this.txtReplace.TabIndex = 3;
      // 
      // btnReplaceNext
      // 
      this.btnReplaceNext.AutoSize = true;
      this.btnReplaceNext.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnReplaceNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceNext.Location = new System.Drawing.Point(543, 3);
      this.btnReplaceNext.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.btnReplaceNext.Name = "btnReplaceNext";
      this.btnReplaceNext.Size = new System.Drawing.Size(25, 25);
      this.btnReplaceNext.TabIndex = 4;
      this.btnReplaceNext.TabStop = false;
      this.btnReplaceNext.Text = ">";
      this.btnReplaceNext.UseVisualStyleBackColor = true;
      this.btnReplaceNext.Click += new System.EventHandler(this.btnReplaceNext_Click);
      // 
      // btnReplaceAll
      // 
      this.btnReplaceAll.AutoSize = true;
      this.btnReplaceAll.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnReplaceAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceAll.Location = new System.Drawing.Point(568, 3);
      this.btnReplaceAll.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
      this.btnReplaceAll.Name = "btnReplaceAll";
      this.btnReplaceAll.Size = new System.Drawing.Size(31, 25);
      this.btnReplaceAll.TabIndex = 5;
      this.btnReplaceAll.TabStop = false;
      this.btnReplaceAll.Text = ">>";
      this.btnReplaceAll.UseVisualStyleBackColor = true;
      this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
      // 
      // btnFind
      // 
      this.btnFind.AutoSize = true;
      this.btnFind.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnFind.Location = new System.Drawing.Point(347, 3);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(31, 25);
      this.btnFind.TabIndex = 2;
      this.btnFind.TabStop = false;
      this.btnFind.Text = "→";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
      this.flowLayoutPanel1.Controls.Add(this.chkNormal);
      this.flowLayoutPanel1.Controls.Add(this.chkExtended);
      this.flowLayoutPanel1.Controls.Add(this.chkRegExp);
      this.flowLayoutPanel1.Controls.Add(this.chkXPath);
      this.flowLayoutPanel1.Controls.Add(this.chkCaseSensitive);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(159, 1);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(185, 0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(185, 29);
      this.flowLayoutPanel1.TabIndex = 1;
      // 
      // chkNormal
      // 
      this.chkNormal.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkNormal.AutoSize = true;
      this.chkNormal.Checked = true;
      this.chkNormal.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkNormal.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.chkNormal.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkNormal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkNormal.Location = new System.Drawing.Point(0, 3);
      this.chkNormal.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.chkNormal.Name = "chkNormal";
      this.chkNormal.Size = new System.Drawing.Size(35, 23);
      this.chkNormal.TabIndex = 0;
      this.chkNormal.TabStop = false;
      this.chkNormal.Text = "abc";
      this.chkNormal.UseVisualStyleBackColor = true;
      this.chkNormal.CheckedChanged += new System.EventHandler(this.chkNormal_CheckedChanged);
      // 
      // chkExtended
      // 
      this.chkExtended.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkExtended.AutoSize = true;
      this.chkExtended.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.chkExtended.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkExtended.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkExtended.Location = new System.Drawing.Point(35, 3);
      this.chkExtended.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.chkExtended.Name = "chkExtended";
      this.chkExtended.Size = new System.Drawing.Size(36, 23);
      this.chkExtended.TabIndex = 1;
      this.chkExtended.TabStop = false;
      this.chkExtended.Text = "\\r\\n";
      this.chkExtended.UseVisualStyleBackColor = true;
      this.chkExtended.CheckedChanged += new System.EventHandler(this.chkExtended_CheckedChanged);
      // 
      // chkRegExp
      // 
      this.chkRegExp.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkRegExp.AutoSize = true;
      this.chkRegExp.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.chkRegExp.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkRegExp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkRegExp.Location = new System.Drawing.Point(71, 3);
      this.chkRegExp.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.chkRegExp.Name = "chkRegExp";
      this.chkRegExp.Size = new System.Drawing.Size(24, 23);
      this.chkRegExp.TabIndex = 2;
      this.chkRegExp.TabStop = false;
      this.chkRegExp.Text = ".*";
      this.chkRegExp.UseVisualStyleBackColor = true;
      this.chkRegExp.CheckedChanged += new System.EventHandler(this.chkRegExp_CheckedChanged);
      // 
      // chkXPath
      // 
      this.chkXPath.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkXPath.AutoSize = true;
      this.chkXPath.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.chkXPath.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkXPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkXPath.Location = new System.Drawing.Point(95, 3);
      this.chkXPath.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.chkXPath.Name = "chkXPath";
      this.chkXPath.Size = new System.Drawing.Size(51, 23);
      this.chkXPath.TabIndex = 3;
      this.chkXPath.TabStop = false;
      this.chkXPath.Text = "/e[@a]";
      this.chkXPath.UseVisualStyleBackColor = true;
      this.chkXPath.CheckedChanged += new System.EventHandler(this.chkXPath_CheckedChanged);
      // 
      // chkCaseSensitive
      // 
      this.chkCaseSensitive.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkCaseSensitive.AutoSize = true;
      this.chkCaseSensitive.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.chkCaseSensitive.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkCaseSensitive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkCaseSensitive.Location = new System.Drawing.Point(152, 3);
      this.chkCaseSensitive.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
      this.chkCaseSensitive.Name = "chkCaseSensitive";
      this.chkCaseSensitive.Size = new System.Drawing.Size(30, 23);
      this.chkCaseSensitive.TabIndex = 4;
      this.chkCaseSensitive.TabStop = false;
      this.chkCaseSensitive.Text = "Aa";
      this.chkCaseSensitive.UseVisualStyleBackColor = true;
      // 
      // editor
      // 
      this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.editor.Location = new System.Drawing.Point(3, 31);
      this.editor.Name = "editor";
      this.editor.PlaceholderText = "";
      this.editor.ReadOnly = false;
      this.editor.ShowLineNumbers = true;
      this.editor.ShowScrollbars = true;
      this.editor.SingleLine = false;
      this.editor.Size = new System.Drawing.Size(631, 290);
      this.editor.TabIndex = 5;
      this.editor.TabStop = false;
      // 
      // btnClose
      // 
      this.btnClose.AutoSize = true;
      this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnClose.Location = new System.Drawing.Point(608, 3);
      this.btnClose.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(26, 25);
      this.btnClose.TabIndex = 6;
      this.btnClose.TabStop = false;
      this.btnClose.Text = "X";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // FullEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "FullEditor";
      this.Size = new System.Drawing.Size(637, 324);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.findReplacePanel.ResumeLayout(false);
      this.findReplacePanel.PerformLayout();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel findReplacePanel;
    private EditorWinForm txtFind;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.CheckBox chkNormal;
    private System.Windows.Forms.CheckBox chkExtended;
    private System.Windows.Forms.CheckBox chkRegExp;
    private System.Windows.Forms.CheckBox chkXPath;
    private System.Windows.Forms.CheckBox chkCaseSensitive;
    private EditorWinForm txtReplace;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.Button btnReplaceNext;
    private System.Windows.Forms.Button btnReplaceAll;
    private EditorWinForm editor;
    private System.Windows.Forms.Button btnClose;
  }
}
