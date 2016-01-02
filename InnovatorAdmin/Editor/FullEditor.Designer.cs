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
      this.components = new System.ComponentModel.Container();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.findReplacePanel = new System.Windows.Forms.TableLayoutPanel();
      this.txtFind = new InnovatorAdmin.Editor.EditorWinForm();
      this.txtReplace = new InnovatorAdmin.Editor.EditorWinForm();
      this.btnReplaceNext = new System.Windows.Forms.Button();
      this.btnReplaceAll = new System.Windows.Forms.Button();
      this.btnFind = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnOptions = new System.Windows.Forms.Button();
      this.btnFindAll = new System.Windows.Forms.Button();
      this.editor = new InnovatorAdmin.Editor.EditorWinForm();
      this.conOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mniNormal = new System.Windows.Forms.ToolStripMenuItem();
      this.mniExtended = new System.Windows.Forms.ToolStripMenuItem();
      this.mniRegex = new System.Windows.Forms.ToolStripMenuItem();
      this.mniXpath = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.mniMatchCase = new System.Windows.Forms.ToolStripMenuItem();
      this.tableLayoutPanel1.SuspendLayout();
      this.findReplacePanel.SuspendLayout();
      this.conOptions.SuspendLayout();
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
      this.findReplacePanel.ColumnCount = 9;
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.findReplacePanel.Controls.Add(this.txtFind, 1, 0);
      this.findReplacePanel.Controls.Add(this.txtReplace, 5, 0);
      this.findReplacePanel.Controls.Add(this.btnReplaceNext, 6, 0);
      this.findReplacePanel.Controls.Add(this.btnReplaceAll, 7, 0);
      this.findReplacePanel.Controls.Add(this.btnFind, 3, 0);
      this.findReplacePanel.Controls.Add(this.btnClose, 8, 0);
      this.findReplacePanel.Controls.Add(this.btnOptions, 2, 0);
      this.findReplacePanel.Controls.Add(this.btnFindAll, 4, 0);
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
      this.txtFind.Size = new System.Drawing.Size(153, 22);
      this.txtFind.TabIndex = 0;
      // 
      // txtReplace
      // 
      this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.txtReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtReplace.Location = new System.Drawing.Point(289, 5);
      this.txtReplace.Name = "txtReplace";
      this.txtReplace.PlaceholderText = "Replace with...";
      this.txtReplace.ReadOnly = false;
      this.txtReplace.ShowLineNumbers = false;
      this.txtReplace.ShowScrollbars = false;
      this.txtReplace.SingleLine = true;
      this.txtReplace.Size = new System.Drawing.Size(172, 20);
      this.txtReplace.TabIndex = 4;
      // 
      // btnReplaceNext
      // 
      this.btnReplaceNext.AutoSize = true;
      this.btnReplaceNext.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnReplaceNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceNext.Location = new System.Drawing.Point(467, 3);
      this.btnReplaceNext.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.btnReplaceNext.Name = "btnReplaceNext";
      this.btnReplaceNext.Size = new System.Drawing.Size(59, 25);
      this.btnReplaceNext.TabIndex = 5;
      this.btnReplaceNext.TabStop = false;
      this.btnReplaceNext.Text = "Replace";
      this.btnReplaceNext.UseVisualStyleBackColor = true;
      this.btnReplaceNext.Click += new System.EventHandler(this.btnReplaceNext_Click);
      // 
      // btnReplaceAll
      // 
      this.btnReplaceAll.AutoSize = true;
      this.btnReplaceAll.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnReplaceAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceAll.Location = new System.Drawing.Point(526, 3);
      this.btnReplaceAll.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
      this.btnReplaceAll.Name = "btnReplaceAll";
      this.btnReplaceAll.Size = new System.Drawing.Size(73, 25);
      this.btnReplaceAll.TabIndex = 6;
      this.btnReplaceAll.TabStop = false;
      this.btnReplaceAll.Text = "Replace All";
      this.btnReplaceAll.UseVisualStyleBackColor = true;
      this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
      // 
      // btnFind
      // 
      this.btnFind.AutoSize = true;
      this.btnFind.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnFind.Location = new System.Drawing.Point(212, 3);
      this.btnFind.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(41, 25);
      this.btnFind.TabIndex = 2;
      this.btnFind.TabStop = false;
      this.btnFind.Text = "Next";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
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
      this.btnClose.TabIndex = 7;
      this.btnClose.TabStop = false;
      this.btnClose.Text = "X";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnOptions
      // 
      this.btnOptions.AutoSize = true;
      this.btnOptions.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOptions.Location = new System.Drawing.Point(181, 3);
      this.btnOptions.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.btnOptions.Name = "btnOptions";
      this.btnOptions.Size = new System.Drawing.Size(31, 25);
      this.btnOptions.TabIndex = 1;
      this.btnOptions.TabStop = false;
      this.btnOptions.Text = "▼";
      this.btnOptions.UseVisualStyleBackColor = true;
      this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
      // 
      // btnFindAll
      // 
      this.btnFindAll.AutoSize = true;
      this.btnFindAll.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
      this.btnFindAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnFindAll.Location = new System.Drawing.Point(253, 3);
      this.btnFindAll.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
      this.btnFindAll.Name = "btnFindAll";
      this.btnFindAll.Size = new System.Drawing.Size(30, 25);
      this.btnFindAll.TabIndex = 3;
      this.btnFindAll.TabStop = false;
      this.btnFindAll.Text = "All";
      this.btnFindAll.UseVisualStyleBackColor = true;
      this.btnFindAll.Click += new System.EventHandler(this.btnFindAll_Click);
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
      // conOptions
      // 
      this.conOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniNormal,
            this.mniExtended,
            this.mniRegex,
            this.mniXpath,
            this.toolStripSeparator1,
            this.mniMatchCase});
      this.conOptions.Name = "conOptions";
      this.conOptions.Size = new System.Drawing.Size(218, 120);
      // 
      // mniNormal
      // 
      this.mniNormal.Checked = true;
      this.mniNormal.CheckOnClick = true;
      this.mniNormal.CheckState = System.Windows.Forms.CheckState.Checked;
      this.mniNormal.Name = "mniNormal";
      this.mniNormal.Size = new System.Drawing.Size(217, 22);
      this.mniNormal.Text = "Normal";
      this.mniNormal.Click += new System.EventHandler(this.mniNormal_Click);
      // 
      // mniExtended
      // 
      this.mniExtended.CheckOnClick = true;
      this.mniExtended.Name = "mniExtended";
      this.mniExtended.Size = new System.Drawing.Size(217, 22);
      this.mniExtended.Text = "Extended (\\n, \\r, \\t, \\0, \\x...)";
      this.mniExtended.Click += new System.EventHandler(this.mniExtended_Click);
      // 
      // mniRegex
      // 
      this.mniRegex.CheckOnClick = true;
      this.mniRegex.Name = "mniRegex";
      this.mniRegex.Size = new System.Drawing.Size(217, 22);
      this.mniRegex.Text = "Regular Expression";
      this.mniRegex.Click += new System.EventHandler(this.mniRegex_Click);
      // 
      // mniXpath
      // 
      this.mniXpath.CheckOnClick = true;
      this.mniXpath.Name = "mniXpath";
      this.mniXpath.Size = new System.Drawing.Size(217, 22);
      this.mniXpath.Text = "XPath";
      this.mniXpath.Click += new System.EventHandler(this.mniXpath_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(214, 6);
      // 
      // mniMatchCase
      // 
      this.mniMatchCase.CheckOnClick = true;
      this.mniMatchCase.Name = "mniMatchCase";
      this.mniMatchCase.Size = new System.Drawing.Size(217, 22);
      this.mniMatchCase.Text = "Match Case";
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
      this.conOptions.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel findReplacePanel;
    private EditorWinForm txtFind;
    private EditorWinForm txtReplace;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.Button btnReplaceNext;
    private System.Windows.Forms.Button btnReplaceAll;
    private EditorWinForm editor;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.ContextMenuStrip conOptions;
    private System.Windows.Forms.ToolStripMenuItem mniNormal;
    private System.Windows.Forms.ToolStripMenuItem mniExtended;
    private System.Windows.Forms.ToolStripMenuItem mniRegex;
    private System.Windows.Forms.ToolStripMenuItem mniXpath;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem mniMatchCase;
    private System.Windows.Forms.Button btnOptions;
    private System.Windows.Forms.Button btnFindAll;
  }
}
