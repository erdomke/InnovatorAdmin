using System;

namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class ImportMapping
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
      if (disposing)
      {
        if (components != null) components.Dispose();
        try
        {
          AutoSaveXslt();
        }
        catch (Exception) { }
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
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.pgSimple = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.gridPreview = new System.Windows.Forms.DataGridView();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.label3 = new System.Windows.Forms.Label();
      this.nudBatchSize = new System.Windows.Forms.NumericUpDown();
      this.pgAdvanced = new System.Windows.Forms.TabPage();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.splitContainer3 = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.xmlEditor = new Aras.Tools.InnovatorAdmin.Editor.EditorControl();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.outputEditor = new Aras.Tools.InnovatorAdmin.Editor.EditorControl();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.xsltEditor = new Aras.Tools.InnovatorAdmin.Editor.EditorControl();
      this.btnOpen = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnSave = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnTest = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnLoadMore = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.chkChecksum = new System.Windows.Forms.CheckBox();
      this.chkFileSize = new System.Windows.Forms.CheckBox();
      this.lblCount = new System.Windows.Forms.Label();
      this.countWorker = new System.ComponentModel.BackgroundWorker();
      this.timerAutoSave = new System.Windows.Forms.Timer(this.components);
      this.tabControl1.SuspendLayout();
      this.pgSimple.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridPreview)).BeginInit();
      this.tableLayoutPanel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudBatchSize)).BeginInit();
      this.pgAdvanced.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
      this.splitContainer3.Panel1.SuspendLayout();
      this.splitContainer3.Panel2.SuspendLayout();
      this.splitContainer3.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.tabControl1, 3);
      this.tabControl1.Controls.Add(this.pgSimple);
      this.tabControl1.Controls.Add(this.pgAdvanced);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(3, 3);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(601, 349);
      this.tabControl1.TabIndex = 0;
      // 
      // pgSimple
      // 
      this.pgSimple.Controls.Add(this.splitContainer1);
      this.pgSimple.Location = new System.Drawing.Point(4, 22);
      this.pgSimple.Name = "pgSimple";
      this.pgSimple.Padding = new System.Windows.Forms.Padding(3);
      this.pgSimple.Size = new System.Drawing.Size(593, 323);
      this.pgSimple.TabIndex = 0;
      this.pgSimple.Text = "Simple";
      this.pgSimple.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer1.Location = new System.Drawing.Point(3, 3);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.gridPreview);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel5);
      this.splitContainer1.Size = new System.Drawing.Size(587, 317);
      this.splitContainer1.SplitterDistance = 366;
      this.splitContainer1.TabIndex = 0;
      // 
      // gridPreview
      // 
      this.gridPreview.AllowUserToAddRows = false;
      this.gridPreview.AllowUserToDeleteRows = false;
      this.gridPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridPreview.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridPreview.Location = new System.Drawing.Point(0, 0);
      this.gridPreview.Name = "gridPreview";
      this.gridPreview.Size = new System.Drawing.Size(366, 317);
      this.gridPreview.TabIndex = 0;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 2;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.Controls.Add(this.chkFileSize, 1, 2);
      this.tableLayoutPanel5.Controls.Add(this.chkChecksum, 1, 1);
      this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
      this.tableLayoutPanel5.Controls.Add(this.nudBatchSize, 1, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 3;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(217, 317);
      this.tableLayoutPanel5.TabIndex = 0;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 3);
      this.label3.Margin = new System.Windows.Forms.Padding(3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(58, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "Batch Size";
      // 
      // nudBatchSize
      // 
      this.nudBatchSize.Location = new System.Drawing.Point(111, 3);
      this.nudBatchSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
      this.nudBatchSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.nudBatchSize.Name = "nudBatchSize";
      this.nudBatchSize.Size = new System.Drawing.Size(103, 20);
      this.nudBatchSize.TabIndex = 1;
      this.nudBatchSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
      // 
      // pgAdvanced
      // 
      this.pgAdvanced.Controls.Add(this.splitContainer2);
      this.pgAdvanced.Location = new System.Drawing.Point(4, 22);
      this.pgAdvanced.Name = "pgAdvanced";
      this.pgAdvanced.Padding = new System.Windows.Forms.Padding(3);
      this.pgAdvanced.Size = new System.Drawing.Size(593, 323);
      this.pgAdvanced.TabIndex = 1;
      this.pgAdvanced.Text = "Advanced";
      this.pgAdvanced.UseVisualStyleBackColor = true;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(3, 3);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel4);
      this.splitContainer2.Size = new System.Drawing.Size(587, 317);
      this.splitContainer2.SplitterDistance = 305;
      this.splitContainer2.TabIndex = 0;
      // 
      // splitContainer3
      // 
      this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer3.Location = new System.Drawing.Point(0, 0);
      this.splitContainer3.Margin = new System.Windows.Forms.Padding(0);
      this.splitContainer3.Name = "splitContainer3";
      this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer3.Panel1
      // 
      this.splitContainer3.Panel1.Controls.Add(this.tableLayoutPanel2);
      // 
      // splitContainer3.Panel2
      // 
      this.splitContainer3.Panel2.Controls.Add(this.tableLayoutPanel3);
      this.splitContainer3.Size = new System.Drawing.Size(305, 317);
      this.splitContainer3.SplitterDistance = 158;
      this.splitContainer3.TabIndex = 1;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.xmlEditor, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(305, 158);
      this.tableLayoutPanel2.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 3);
      this.label1.Margin = new System.Windows.Forms.Padding(3);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(31, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Input";
      // 
      // xmlEditor
      // 
      this.xmlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.xmlEditor.Helper = null;
      this.xmlEditor.Location = new System.Drawing.Point(3, 22);
      this.xmlEditor.Name = "xmlEditor";
      this.xmlEditor.ReadOnly = true;
      this.xmlEditor.Size = new System.Drawing.Size(299, 133);
      this.xmlEditor.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.outputEditor, 0, 1);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(305, 155);
      this.tableLayoutPanel3.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 3);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(39, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Output";
      // 
      // outputEditor
      // 
      this.outputEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.outputEditor.Helper = null;
      this.outputEditor.Location = new System.Drawing.Point(3, 22);
      this.outputEditor.Name = "outputEditor";
      this.outputEditor.ReadOnly = true;
      this.outputEditor.Size = new System.Drawing.Size(299, 130);
      this.outputEditor.TabIndex = 0;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 3;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.Controls.Add(this.xsltEditor, 0, 1);
      this.tableLayoutPanel4.Controls.Add(this.btnOpen, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnTest, 2, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(278, 317);
      this.tableLayoutPanel4.TabIndex = 1;
      // 
      // xsltEditor
      // 
      this.tableLayoutPanel4.SetColumnSpan(this.xsltEditor, 3);
      this.xsltEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.xsltEditor.Helper = null;
      this.xsltEditor.Location = new System.Drawing.Point(3, 36);
      this.xsltEditor.Name = "xsltEditor";
      this.xsltEditor.ReadOnly = false;
      this.xsltEditor.Size = new System.Drawing.Size(272, 278);
      this.xsltEditor.TabIndex = 0;
      this.xsltEditor.RunRequested += new System.EventHandler<Aras.Tools.InnovatorAdmin.Editor.RunRequestedEventArgs>(this.xsltEditor_RunRequested);
      // 
      // btnOpen
      // 
      this.btnOpen.AutoSize = true;
      this.btnOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOpen.FlatAppearance.BorderSize = 0;
      this.btnOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOpen.ForeColor = System.Drawing.Color.Black;
      this.btnOpen.Location = new System.Drawing.Point(3, 3);
      this.btnOpen.Name = "btnOpen";
      this.btnOpen.Padding = new System.Windows.Forms.Padding(2);
      this.btnOpen.Size = new System.Drawing.Size(75, 27);
      this.btnOpen.TabIndex = 1;
      this.btnOpen.Text = "Open";
      this.btnOpen.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnOpen.UseVisualStyleBackColor = false;
      this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
      // 
      // btnSave
      // 
      this.btnSave.AutoSize = true;
      this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnSave.FlatAppearance.BorderSize = 0;
      this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnSave.ForeColor = System.Drawing.Color.Black;
      this.btnSave.Location = new System.Drawing.Point(84, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Padding = new System.Windows.Forms.Padding(2);
      this.btnSave.Size = new System.Drawing.Size(75, 27);
      this.btnSave.TabIndex = 2;
      this.btnSave.Text = "Save";
      this.btnSave.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnSave.UseVisualStyleBackColor = false;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnTest
      // 
      this.btnTest.AutoSize = true;
      this.btnTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnTest.FlatAppearance.BorderSize = 0;
      this.btnTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnTest.ForeColor = System.Drawing.Color.Black;
      this.btnTest.Location = new System.Drawing.Point(165, 3);
      this.btnTest.Name = "btnTest";
      this.btnTest.Padding = new System.Windows.Forms.Padding(2);
      this.btnTest.Size = new System.Drawing.Size(75, 27);
      this.btnTest.TabIndex = 3;
      this.btnTest.Text = "Test";
      this.btnTest.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnTest.UseVisualStyleBackColor = false;
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnLoadMore, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.lblCount, 1, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(607, 388);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // btnLoadMore
      // 
      this.btnLoadMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLoadMore.AutoSize = true;
      this.btnLoadMore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnLoadMore.FlatAppearance.BorderSize = 0;
      this.btnLoadMore.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnLoadMore.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnLoadMore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnLoadMore.ForeColor = System.Drawing.Color.Black;
      this.btnLoadMore.Location = new System.Drawing.Point(529, 358);
      this.btnLoadMore.Name = "btnLoadMore";
      this.btnLoadMore.Padding = new System.Windows.Forms.Padding(2);
      this.btnLoadMore.Size = new System.Drawing.Size(75, 27);
      this.btnLoadMore.TabIndex = 1;
      this.btnLoadMore.Text = "Load More";
      this.btnLoadMore.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnLoadMore.UseVisualStyleBackColor = false;
      this.btnLoadMore.Click += new System.EventHandler(this.btnLoadMore_Click);
      // 
      // chkChecksum
      // 
      this.chkChecksum.AutoSize = true;
      this.chkChecksum.Location = new System.Drawing.Point(111, 29);
      this.chkChecksum.Name = "chkChecksum";
      this.chkChecksum.Size = new System.Drawing.Size(76, 17);
      this.chkChecksum.TabIndex = 0;
      this.chkChecksum.Text = "Checksum";
      this.chkChecksum.UseVisualStyleBackColor = true;
      this.chkChecksum.CheckedChanged += new System.EventHandler(this.chkChecksum_CheckedChanged);
      // 
      // chkFileSize
      // 
      this.chkFileSize.AutoSize = true;
      this.chkFileSize.Location = new System.Drawing.Point(111, 52);
      this.chkFileSize.Name = "chkFileSize";
      this.chkFileSize.Size = new System.Drawing.Size(65, 17);
      this.chkFileSize.TabIndex = 1;
      this.chkFileSize.Text = "File Size";
      this.chkFileSize.UseVisualStyleBackColor = true;
      this.chkFileSize.CheckedChanged += new System.EventHandler(this.chkFileSize_CheckedChanged);
      // 
      // lblCount
      // 
      this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.lblCount.AutoSize = true;
      this.lblCount.Location = new System.Drawing.Point(523, 365);
      this.lblCount.Name = "lblCount";
      this.lblCount.Size = new System.Drawing.Size(1, 13);
      this.lblCount.TabIndex = 3;
      // 
      // countWorker
      // 
      this.countWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.countWorker_DoWork);
      this.countWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.countWorker_RunWorkerCompleted);
      // 
      // timerAutoSave
      // 
      this.timerAutoSave.Enabled = true;
      this.timerAutoSave.Interval = 120000;
      this.timerAutoSave.Tick += new System.EventHandler(this.timerAutoSave_Tick);
      // 
      // ImportMapping
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "ImportMapping";
      this.Size = new System.Drawing.Size(607, 388);
      this.tabControl1.ResumeLayout(false);
      this.pgSimple.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.gridPreview)).EndInit();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudBatchSize)).EndInit();
      this.pgAdvanced.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.splitContainer3.Panel1.ResumeLayout(false);
      this.splitContainer3.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
      this.splitContainer3.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage pgSimple;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TabPage pgAdvanced;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private Editor.EditorControl xsltEditor;
    private System.Windows.Forms.DataGridView gridPreview;
    private Editor.EditorControl xmlEditor;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private FlatButton btnLoadMore;
    private System.Windows.Forms.CheckBox chkChecksum;
    private System.Windows.Forms.CheckBox chkFileSize;
    private System.Windows.Forms.SplitContainer splitContainer3;
    private Editor.EditorControl outputEditor;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label label2;
    private System.ComponentModel.BackgroundWorker countWorker;
    private System.Windows.Forms.Label lblCount;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private FlatButton btnOpen;
    private FlatButton btnSave;
    private System.Windows.Forms.Timer timerAutoSave;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown nudBatchSize;
    private FlatButton btnTest;
  }
}
