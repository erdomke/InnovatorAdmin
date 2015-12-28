namespace InnovatorAdmin.Editor
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
      ICSharpCode.AvalonEdit.Document.TextDocument textDocument3 = new ICSharpCode.AvalonEdit.Document.TextDocument();
      System.ComponentModel.Design.ServiceContainer serviceContainer3 = new System.ComponentModel.Design.ServiceContainer();
      ICSharpCode.AvalonEdit.Document.UndoStack undoStack3 = new ICSharpCode.AvalonEdit.Document.UndoStack();
      ICSharpCode.AvalonEdit.Document.TextDocument textDocument4 = new ICSharpCode.AvalonEdit.Document.TextDocument();
      System.ComponentModel.Design.ServiceContainer serviceContainer4 = new System.ComponentModel.Design.ServiceContainer();
      ICSharpCode.AvalonEdit.Document.UndoStack undoStack4 = new ICSharpCode.AvalonEdit.Document.UndoStack();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
      this.chkNormal = new System.Windows.Forms.CheckBox();
      this.chkExtended = new System.Windows.Forms.CheckBox();
      this.chkRegExp = new System.Windows.Forms.CheckBox();
      this.chkXPath = new System.Windows.Forms.CheckBox();
      this.btnFind = new System.Windows.Forms.Button();
      this.btnReplaceNext = new System.Windows.Forms.Button();
      this.btnReplaceAll = new System.Windows.Forms.Button();
      this.btnShowReplace = new System.Windows.Forms.Button();
      this.toolTips = new System.Windows.Forms.ToolTip(this.components);
      this.label1 = new System.Windows.Forms.Label();
      this.txtFind = new InnovatorAdmin.Editor.EditorControl();
      this.txtReplace = new InnovatorAdmin.Editor.EditorControl();
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
      this.tableLayoutPanel1.Controls.Add(this.txtFind, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
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
      this.tableLayoutPanel1.Size = new System.Drawing.Size(431, 93);
      this.tableLayoutPanel1.TabIndex = 3;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 4);
      this.flowLayoutPanel1.Controls.Add(this.chkNormal);
      this.flowLayoutPanel1.Controls.Add(this.chkExtended);
      this.flowLayoutPanel1.Controls.Add(this.chkRegExp);
      this.flowLayoutPanel1.Controls.Add(this.chkXPath);
      this.flowLayoutPanel1.Controls.Add(this.label1);
      this.flowLayoutPanel1.Controls.Add(this.chkCaseSensitive);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 58);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(431, 29);
      this.flowLayoutPanel1.TabIndex = 99;
      // 
      // chkCaseSensitive
      // 
      this.chkCaseSensitive.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkCaseSensitive.AutoSize = true;
      this.chkCaseSensitive.FlatAppearance.BorderSize = 0;
      this.chkCaseSensitive.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.chkCaseSensitive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkCaseSensitive.Location = new System.Drawing.Point(180, 3);
      this.chkCaseSensitive.Name = "chkCaseSensitive";
      this.chkCaseSensitive.Size = new System.Drawing.Size(30, 23);
      this.chkCaseSensitive.TabIndex = 5;
      this.chkCaseSensitive.Text = "Aa";
      this.toolTips.SetToolTip(this.chkCaseSensitive, "Match case");
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
      this.chkNormal.Location = new System.Drawing.Point(3, 3);
      this.chkNormal.Name = "chkNormal";
      this.chkNormal.Size = new System.Drawing.Size(35, 23);
      this.chkNormal.TabIndex = 0;
      this.chkNormal.Text = "abc";
      this.toolTips.SetToolTip(this.chkNormal, "Mode: Normal");
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
      this.chkExtended.Location = new System.Drawing.Point(44, 3);
      this.chkExtended.Name = "chkExtended";
      this.chkExtended.Size = new System.Drawing.Size(36, 23);
      this.chkExtended.TabIndex = 1;
      this.chkExtended.Text = "\\r\\n";
      this.toolTips.SetToolTip(this.chkExtended, "Mode: Extended (\\r, \\n, \\t, ...)");
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
      this.chkRegExp.Location = new System.Drawing.Point(86, 3);
      this.chkRegExp.Name = "chkRegExp";
      this.chkRegExp.Size = new System.Drawing.Size(24, 23);
      this.chkRegExp.TabIndex = 2;
      this.chkRegExp.Text = ".*";
      this.toolTips.SetToolTip(this.chkRegExp, "Mode: Regular Expressions");
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
      this.chkXPath.Location = new System.Drawing.Point(116, 3);
      this.chkXPath.Name = "chkXPath";
      this.chkXPath.Size = new System.Drawing.Size(51, 23);
      this.chkXPath.TabIndex = 3;
      this.chkXPath.Text = "/e[@a]";
      this.toolTips.SetToolTip(this.chkXPath, "Mode: XPath");
      this.chkXPath.UseVisualStyleBackColor = true;
      this.chkXPath.CheckedChanged += new System.EventHandler(this.chkXPath_CheckedChanged);
      // 
      // btnFind
      // 
      this.btnFind.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.btnFind, 2);
      this.btnFind.FlatAppearance.BorderSize = 0;
      this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnFind.Location = new System.Drawing.Point(370, 3);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(29, 23);
      this.btnFind.TabIndex = 2;
      this.btnFind.Text = "→";
      this.toolTips.SetToolTip(this.btnFind, "Find next");
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // btnReplaceNext
      // 
      this.btnReplaceNext.AutoSize = true;
      this.btnReplaceNext.FlatAppearance.BorderSize = 0;
      this.btnReplaceNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceNext.Location = new System.Drawing.Point(370, 32);
      this.btnReplaceNext.Name = "btnReplaceNext";
      this.btnReplaceNext.Size = new System.Drawing.Size(23, 23);
      this.btnReplaceNext.TabIndex = 3;
      this.btnReplaceNext.Text = ">";
      this.toolTips.SetToolTip(this.btnReplaceNext, "Replace");
      this.btnReplaceNext.UseVisualStyleBackColor = true;
      this.btnReplaceNext.Click += new System.EventHandler(this.btnReplaceNext_Click);
      // 
      // btnReplaceAll
      // 
      this.btnReplaceAll.AutoSize = true;
      this.btnReplaceAll.FlatAppearance.BorderSize = 0;
      this.btnReplaceAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnReplaceAll.Location = new System.Drawing.Point(399, 32);
      this.btnReplaceAll.Name = "btnReplaceAll";
      this.btnReplaceAll.Size = new System.Drawing.Size(29, 23);
      this.btnReplaceAll.TabIndex = 4;
      this.btnReplaceAll.Text = ">>";
      this.toolTips.SetToolTip(this.btnReplaceAll, "Replace All");
      this.btnReplaceAll.UseVisualStyleBackColor = true;
      this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
      // 
      // btnShowReplace
      // 
      this.btnShowReplace.AutoSize = true;
      this.btnShowReplace.FlatAppearance.BorderSize = 0;
      this.btnShowReplace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnShowReplace.Location = new System.Drawing.Point(3, 3);
      this.btnShowReplace.Name = "btnShowReplace";
      this.btnShowReplace.Size = new System.Drawing.Size(29, 23);
      this.btnShowReplace.TabIndex = 5;
      this.btnShowReplace.Text = "▼";
      this.toolTips.SetToolTip(this.btnShowReplace, "Toggle between find and find and replace modes");
      this.btnShowReplace.UseVisualStyleBackColor = true;
      this.btnShowReplace.Click += new System.EventHandler(this.btnShowReplace_Click);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.BackColor = System.Drawing.Color.DimGray;
      this.label1.Location = new System.Drawing.Point(173, 3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(1, 22);
      this.label1.TabIndex = 4;
      // 
      // txtFind
      // 
      this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      textDocument3.FileName = null;
      textDocument3.ServiceProvider = serviceContainer3;
      textDocument3.Text = "";
      undoStack3.SizeLimit = 2147483647;
      textDocument3.UndoStack = undoStack3;
      this.txtFind.Document = textDocument3;
      this.txtFind.Helper = null;
      this.txtFind.Location = new System.Drawing.Point(38, 3);
      this.txtFind.Name = "txtFind";
      this.txtFind.ReadOnly = false;
      this.txtFind.ShowLineNumbers = false;
      this.txtFind.ShowScrollbars = false;
      this.txtFind.SingleLine = false;
      this.txtFind.Size = new System.Drawing.Size(326, 22);
      this.txtFind.TabIndex = 0;
      // 
      // txtReplace
      // 
      this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.txtReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      textDocument4.FileName = null;
      textDocument4.ServiceProvider = serviceContainer4;
      textDocument4.Text = "";
      undoStack4.SizeLimit = 2147483647;
      textDocument4.UndoStack = undoStack4;
      this.txtReplace.Document = textDocument4;
      this.txtReplace.Helper = null;
      this.txtReplace.Location = new System.Drawing.Point(38, 33);
      this.txtReplace.Name = "txtReplace";
      this.txtReplace.ReadOnly = false;
      this.txtReplace.ShowLineNumbers = false;
      this.txtReplace.ShowScrollbars = false;
      this.txtReplace.SingleLine = true;
      this.txtReplace.Size = new System.Drawing.Size(326, 20);
      this.txtReplace.TabIndex = 1;
      // 
      // FindReplace
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(431, 97);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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
    private EditorControl txtReplace;
    private System.Windows.Forms.ToolTip toolTips;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.Button btnReplaceNext;
    private System.Windows.Forms.Button btnReplaceAll;
    private System.Windows.Forms.Button btnShowReplace;
    private System.Windows.Forms.CheckBox chkCaseSensitive;
    private System.Windows.Forms.CheckBox chkNormal;
    private System.Windows.Forms.CheckBox chkExtended;
    private System.Windows.Forms.CheckBox chkRegExp;
    private System.Windows.Forms.CheckBox chkXPath;
    private EditorControl txtFind;
    private System.Windows.Forms.Label label1;

  }
}
