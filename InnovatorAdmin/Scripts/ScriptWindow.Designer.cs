namespace InnovatorAdmin.Scripts
{
  partial class ScriptWindow
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptWindow));
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.propGrid = new System.Windows.Forms.PropertyGrid();
      this.menuStrip = new System.Windows.Forms.ToolStrip();
      this.lblScriptName = new System.Windows.Forms.ToolStripLabel();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
      this.btnEditConnection = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
      this.mniOtherScripts = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.mniClose = new System.Windows.Forms.ToolStripMenuItem();
      this.btnSubmit = new System.Windows.Forms.ToolStripButton();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.lblConnColor = new System.Windows.Forms.Label();
      this.outputEditor = new InnovatorAdmin.Editor.EditorControl();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
      this.splitContainer1.Panel1.Controls.Add(this.menuStrip);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.outputEditor);
      this.splitContainer1.Size = new System.Drawing.Size(519, 478);
      this.splitContainer1.SplitterDistance = 216;
      this.splitContainer1.TabIndex = 0;
      // 
      // propGrid
      // 
      this.propGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
      this.propGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.propGrid.Location = new System.Drawing.Point(3, 7);
      this.propGrid.Name = "propGrid";
      this.propGrid.Size = new System.Drawing.Size(513, 181);
      this.propGrid.TabIndex = 1;
      // 
      // menuStrip
      // 
      this.menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblScriptName,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.btnEditConnection,
            this.toolStripSeparator3,
            this.toolStripDropDownButton1,
            this.btnSubmit});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(519, 25);
      this.menuStrip.TabIndex = 0;
      this.menuStrip.Text = "toolStrip1";
      // 
      // lblScriptName
      // 
      this.lblScriptName.Name = "lblScriptName";
      this.lblScriptName.Size = new System.Drawing.Size(0, 22);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
      // 
      // toolStripLabel1
      // 
      this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.toolStripLabel1.ForeColor = System.Drawing.Color.DimGray;
      this.toolStripLabel1.Name = "toolStripLabel1";
      this.toolStripLabel1.Size = new System.Drawing.Size(69, 22);
      this.toolStripLabel1.Text = "Connection:";
      // 
      // btnEditConnection
      // 
      this.btnEditConnection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnEditConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnEditConnection.Image")));
      this.btnEditConnection.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnEditConnection.Name = "btnEditConnection";
      this.btnEditConnection.Size = new System.Drawing.Size(105, 22);
      this.btnEditConnection.Text = "No Connection ▼";
      this.btnEditConnection.Click += new System.EventHandler(this.btnEditConnection_Click);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
      // 
      // toolStripDropDownButton1
      // 
      this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniOtherScripts,
            this.toolStripSeparator1,
            this.mniClose});
      this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
      this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
      this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
      this.toolStripDropDownButton1.Text = "&File";
      // 
      // mniOtherScripts
      // 
      this.mniOtherScripts.Name = "mniOtherScripts";
      this.mniOtherScripts.Size = new System.Drawing.Size(151, 22);
      this.mniOtherScripts.Text = "Other Scripts...";
      this.mniOtherScripts.Click += new System.EventHandler(this.mniOtherScripts_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(148, 6);
      // 
      // mniClose
      // 
      this.mniClose.Name = "mniClose";
      this.mniClose.Size = new System.Drawing.Size(151, 22);
      this.mniClose.Text = "Close";
      this.mniClose.Click += new System.EventHandler(this.mniClose_Click);
      // 
      // btnSubmit
      // 
      this.btnSubmit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.btnSubmit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnSubmit.Image = ((System.Drawing.Image)(resources.GetObject("btnSubmit.Image")));
      this.btnSubmit.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSubmit.Name = "btnSubmit";
      this.btnSubmit.Size = new System.Drawing.Size(45, 22);
      this.btnSubmit.Text = "► Run";
      this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.propGrid, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.lblConnColor, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(519, 191);
      this.tableLayoutPanel1.TabIndex = 2;
      // 
      // lblConnColor
      // 
      this.lblConnColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblConnColor.Location = new System.Drawing.Point(3, 0);
      this.lblConnColor.Name = "lblConnColor";
      this.lblConnColor.Size = new System.Drawing.Size(513, 4);
      this.lblConnColor.TabIndex = 2;
      // 
      // outputEditor
      // 
      this.outputEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.outputEditor.Helper = null;
      this.outputEditor.Location = new System.Drawing.Point(0, 0);
      this.outputEditor.Name = "outputEditor";
      this.outputEditor.ReadOnly = false;
      this.outputEditor.Size = new System.Drawing.Size(519, 258);
      this.outputEditor.TabIndex = 0;
      // 
      // ScriptWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(519, 478);
      this.Controls.Add(this.splitContainer1);
      this.Name = "ScriptWindow";
      this.Text = "Script";
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.ToolStrip menuStrip;
    private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    private System.Windows.Forms.ToolStripButton btnEditConnection;
    private System.Windows.Forms.ToolStripButton btnSubmit;
    private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
    private System.Windows.Forms.ToolStripMenuItem mniOtherScripts;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem mniClose;
    private System.Windows.Forms.PropertyGrid propGrid;
    private Editor.EditorControl outputEditor;
    private System.Windows.Forms.ToolStripLabel lblScriptName;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label lblConnColor;
  }
}