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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.pgSimple = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
      this.label4 = new System.Windows.Forms.Label();
      this.gridPreview = new System.Windows.Forms.DataGridView();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.label3 = new System.Windows.Forms.Label();
      this.nudBatchSize = new System.Windows.Forms.NumericUpDown();
      this.dgvMappings = new System.Windows.Forms.DataGridView();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.label6 = new System.Windows.Forms.Label();
      this.cboItemTypes = new System.Windows.Forms.ComboBox();
      this.cboProperties = new System.Windows.Forms.ComboBox();
      this.txtValue = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.lblValue = new System.Windows.Forms.Label();
      this.btnAdd = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.chkCalculated = new System.Windows.Forms.CheckBox();
      this.btnDelete = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
      this.chkPreventDuplicateFiles = new System.Windows.Forms.CheckBox();
      this.chkPreventDuplicateDocs = new System.Windows.Forms.CheckBox();
      this.label9 = new System.Windows.Forms.Label();
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
      this.lblCount = new System.Windows.Forms.Label();
      this.countWorker = new System.ComponentModel.BackgroundWorker();
      this.timerAutoSave = new System.Windows.Forms.Timer(this.components);
      this.tabControl1.SuspendLayout();
      this.pgSimple.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tableLayoutPanel6.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridPreview)).BeginInit();
      this.tableLayoutPanel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudBatchSize)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgvMappings)).BeginInit();
      this.flowLayoutPanel2.SuspendLayout();
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
      this.tabControl1.Size = new System.Drawing.Size(751, 482);
      this.tabControl1.TabIndex = 0;
      // 
      // pgSimple
      // 
      this.pgSimple.Controls.Add(this.splitContainer1);
      this.pgSimple.Location = new System.Drawing.Point(4, 22);
      this.pgSimple.Name = "pgSimple";
      this.pgSimple.Padding = new System.Windows.Forms.Padding(3);
      this.pgSimple.Size = new System.Drawing.Size(743, 456);
      this.pgSimple.TabIndex = 0;
      this.pgSimple.Text = "Simple";
      this.pgSimple.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(3, 3);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel6);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel5);
      this.splitContainer1.Size = new System.Drawing.Size(737, 450);
      this.splitContainer1.SplitterDistance = 220;
      this.splitContainer1.TabIndex = 0;
      // 
      // tableLayoutPanel6
      // 
      this.tableLayoutPanel6.ColumnCount = 1;
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel6.Controls.Add(this.label4, 0, 0);
      this.tableLayoutPanel6.Controls.Add(this.gridPreview, 0, 1);
      this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel6.Name = "tableLayoutPanel6";
      this.tableLayoutPanel6.RowCount = 2;
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel6.Size = new System.Drawing.Size(737, 220);
      this.tableLayoutPanel6.TabIndex = 1;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(3, 3);
      this.label4.Margin = new System.Windows.Forms.Padding(3);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(100, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Source Preview:";
      // 
      // gridPreview
      // 
      this.gridPreview.AllowUserToAddRows = false;
      this.gridPreview.AllowUserToDeleteRows = false;
      this.gridPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridPreview.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridPreview.Location = new System.Drawing.Point(3, 22);
      this.gridPreview.Name = "gridPreview";
      this.gridPreview.Size = new System.Drawing.Size(731, 195);
      this.gridPreview.TabIndex = 0;
      this.gridPreview.SelectionChanged += new System.EventHandler(this.gridPreview_SelectionChanged);
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 7;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.Controls.Add(this.label3, 1, 7);
      this.tableLayoutPanel5.Controls.Add(this.nudBatchSize, 2, 7);
      this.tableLayoutPanel5.Controls.Add(this.dgvMappings, 0, 6);
      this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel1, 2, 0);
      this.tableLayoutPanel5.Controls.Add(this.label6, 0, 5);
      this.tableLayoutPanel5.Controls.Add(this.cboItemTypes, 1, 3);
      this.tableLayoutPanel5.Controls.Add(this.cboProperties, 2, 3);
      this.tableLayoutPanel5.Controls.Add(this.txtValue, 4, 3);
      this.tableLayoutPanel5.Controls.Add(this.label5, 1, 2);
      this.tableLayoutPanel5.Controls.Add(this.label7, 2, 2);
      this.tableLayoutPanel5.Controls.Add(this.lblValue, 4, 2);
      this.tableLayoutPanel5.Controls.Add(this.btnAdd, 5, 3);
      this.tableLayoutPanel5.Controls.Add(this.chkCalculated, 3, 3);
      this.tableLayoutPanel5.Controls.Add(this.btnDelete, 6, 3);
      this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel2, 3, 7);
      this.tableLayoutPanel5.Controls.Add(this.label9, 0, 1);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 10;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(737, 226);
      this.tableLayoutPanel5.TabIndex = 0;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(86, 203);
      this.label3.Margin = new System.Windows.Forms.Padding(3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(58, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "Batch Size";
      // 
      // nudBatchSize
      // 
      this.nudBatchSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nudBatchSize.Location = new System.Drawing.Point(150, 203);
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
      this.nudBatchSize.Size = new System.Drawing.Size(121, 20);
      this.nudBatchSize.TabIndex = 1;
      this.nudBatchSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
      // 
      // dgvMappings
      // 
      this.dgvMappings.AllowUserToAddRows = false;
      this.dgvMappings.AllowUserToDeleteRows = false;
      this.dgvMappings.AllowUserToResizeRows = false;
      this.dgvMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dgvMappings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dgvMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.tableLayoutPanel5.SetColumnSpan(this.dgvMappings, 7);
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dgvMappings.DefaultCellStyle = dataGridViewCellStyle2;
      this.dgvMappings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.dgvMappings.Location = new System.Drawing.Point(3, 95);
      this.dgvMappings.Name = "dgvMappings";
      this.dgvMappings.ReadOnly = true;
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dgvMappings.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
      this.dgvMappings.Size = new System.Drawing.Size(731, 102);
      this.dgvMappings.TabIndex = 4;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(147, 0);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
      this.flowLayoutPanel1.TabIndex = 5;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.tableLayoutPanel5.SetColumnSpan(this.label6, 2);
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(3, 76);
      this.label6.Margin = new System.Windows.Forms.Padding(3);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(137, 13);
      this.label6.TabIndex = 6;
      this.label6.Text = "Mappings and Options:";
      // 
      // cboItemTypes
      // 
      this.cboItemTypes.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.cboItemTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboItemTypes.FormattingEnabled = true;
      this.cboItemTypes.Location = new System.Drawing.Point(23, 46);
      this.cboItemTypes.Name = "cboItemTypes";
      this.cboItemTypes.Size = new System.Drawing.Size(121, 21);
      this.cboItemTypes.TabIndex = 7;
      this.cboItemTypes.DropDown += new System.EventHandler(this.cboItemTypes_DropDown);
      this.cboItemTypes.SelectedIndexChanged += new System.EventHandler(this.cboItemTypes_SelectedIndexChanged);
      // 
      // cboProperties
      // 
      this.cboProperties.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.cboProperties.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboProperties.FormattingEnabled = true;
      this.cboProperties.Location = new System.Drawing.Point(150, 46);
      this.cboProperties.Name = "cboProperties";
      this.cboProperties.Size = new System.Drawing.Size(121, 21);
      this.cboProperties.TabIndex = 8;
      // 
      // txtValue
      // 
      this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.txtValue.Location = new System.Drawing.Point(308, 46);
      this.txtValue.Name = "txtValue";
      this.txtValue.Size = new System.Drawing.Size(312, 20);
      this.txtValue.TabIndex = 10;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(23, 23);
      this.label5.Margin = new System.Windows.Forms.Padding(3);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(31, 13);
      this.label5.TabIndex = 13;
      this.label5.Text = "Type";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(150, 23);
      this.label7.Margin = new System.Windows.Forms.Padding(3);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(46, 13);
      this.label7.TabIndex = 14;
      this.label7.Text = "Property";
      // 
      // lblValue
      // 
      this.lblValue.AutoSize = true;
      this.lblValue.Location = new System.Drawing.Point(308, 23);
      this.lblValue.Margin = new System.Windows.Forms.Padding(3);
      this.lblValue.Name = "lblValue";
      this.lblValue.Size = new System.Drawing.Size(78, 13);
      this.lblValue.TabIndex = 15;
      this.lblValue.Text = "Constant value";
      // 
      // btnAdd
      // 
      this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnAdd.AutoSize = true;
      this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnAdd.FlatAppearance.BorderSize = 0;
      this.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnAdd.ForeColor = System.Drawing.Color.Black;
      this.btnAdd.Location = new System.Drawing.Point(626, 43);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Padding = new System.Windows.Forms.Padding(2);
      this.btnAdd.Size = new System.Drawing.Size(50, 27);
      this.btnAdd.TabIndex = 12;
      this.btnAdd.Text = "Add";
      this.btnAdd.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnAdd.UseVisualStyleBackColor = false;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // chkCalculated
      // 
      this.chkCalculated.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.chkCalculated.Appearance = System.Windows.Forms.Appearance.Button;
      this.chkCalculated.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.chkCalculated.FlatAppearance.BorderSize = 0;
      this.chkCalculated.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(51)))));
      this.chkCalculated.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.chkCalculated.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.chkCalculated.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.chkCalculated.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.function14;
      this.chkCalculated.Location = new System.Drawing.Point(277, 45);
      this.chkCalculated.Name = "chkCalculated";
      this.chkCalculated.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
      this.chkCalculated.Size = new System.Drawing.Size(25, 23);
      this.chkCalculated.TabIndex = 16;
      this.chkCalculated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.chkCalculated.UseVisualStyleBackColor = false;
      this.chkCalculated.CheckedChanged += new System.EventHandler(this.chkCalculated_CheckedChanged);
      // 
      // btnDelete
      // 
      this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnDelete.AutoSize = true;
      this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnDelete.FlatAppearance.BorderSize = 0;
      this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnDelete.ForeColor = System.Drawing.Color.Black;
      this.btnDelete.Location = new System.Drawing.Point(682, 43);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Padding = new System.Windows.Forms.Padding(2);
      this.btnDelete.Size = new System.Drawing.Size(52, 27);
      this.btnDelete.TabIndex = 17;
      this.btnDelete.Text = "Delete";
      this.btnDelete.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnDelete.UseVisualStyleBackColor = false;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // flowLayoutPanel2
      // 
      this.flowLayoutPanel2.AutoSize = true;
      this.tableLayoutPanel5.SetColumnSpan(this.flowLayoutPanel2, 4);
      this.flowLayoutPanel2.Controls.Add(this.chkPreventDuplicateFiles);
      this.flowLayoutPanel2.Controls.Add(this.chkPreventDuplicateDocs);
      this.flowLayoutPanel2.Location = new System.Drawing.Point(274, 200);
      this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel2.Name = "flowLayoutPanel2";
      this.flowLayoutPanel2.Size = new System.Drawing.Size(315, 23);
      this.flowLayoutPanel2.TabIndex = 18;
      // 
      // chkPreventDuplicateFiles
      // 
      this.chkPreventDuplicateFiles.AutoSize = true;
      this.chkPreventDuplicateFiles.Checked = true;
      this.chkPreventDuplicateFiles.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkPreventDuplicateFiles.Location = new System.Drawing.Point(3, 3);
      this.chkPreventDuplicateFiles.Name = "chkPreventDuplicateFiles";
      this.chkPreventDuplicateFiles.Size = new System.Drawing.Size(135, 17);
      this.chkPreventDuplicateFiles.TabIndex = 0;
      this.chkPreventDuplicateFiles.Text = "Prevent Duplicate Files";
      this.chkPreventDuplicateFiles.UseVisualStyleBackColor = true;
      this.chkPreventDuplicateFiles.CheckedChanged += new System.EventHandler(this.chkPreventDuplicateFiles_CheckedChanged);
      // 
      // chkPreventDuplicateDocs
      // 
      this.chkPreventDuplicateDocs.AutoSize = true;
      this.chkPreventDuplicateDocs.Checked = true;
      this.chkPreventDuplicateDocs.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkPreventDuplicateDocs.Location = new System.Drawing.Point(144, 3);
      this.chkPreventDuplicateDocs.Name = "chkPreventDuplicateDocs";
      this.chkPreventDuplicateDocs.Size = new System.Drawing.Size(168, 17);
      this.chkPreventDuplicateDocs.TabIndex = 1;
      this.chkPreventDuplicateDocs.Text = "Prevent Duplicate Documents";
      this.chkPreventDuplicateDocs.UseVisualStyleBackColor = true;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.tableLayoutPanel5.SetColumnSpan(this.label9, 2);
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.Location = new System.Drawing.Point(3, 3);
      this.label9.Margin = new System.Windows.Forms.Padding(3);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(100, 13);
      this.label9.TabIndex = 19;
      this.label9.Text = "Define Mapping:";
      // 
      // pgAdvanced
      // 
      this.pgAdvanced.Controls.Add(this.splitContainer2);
      this.pgAdvanced.Location = new System.Drawing.Point(4, 22);
      this.pgAdvanced.Name = "pgAdvanced";
      this.pgAdvanced.Padding = new System.Windows.Forms.Padding(3);
      this.pgAdvanced.Size = new System.Drawing.Size(743, 456);
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
      this.splitContainer2.Size = new System.Drawing.Size(737, 450);
      this.splitContainer2.SplitterDistance = 382;
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
      this.splitContainer3.Size = new System.Drawing.Size(382, 450);
      this.splitContainer3.SplitterDistance = 223;
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
      this.tableLayoutPanel2.Size = new System.Drawing.Size(382, 223);
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
      this.xmlEditor.Size = new System.Drawing.Size(376, 198);
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
      this.tableLayoutPanel3.Size = new System.Drawing.Size(382, 223);
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
      this.outputEditor.Size = new System.Drawing.Size(376, 198);
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
      this.tableLayoutPanel4.Size = new System.Drawing.Size(351, 450);
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
      this.xsltEditor.Size = new System.Drawing.Size(345, 411);
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
      this.tableLayoutPanel1.Size = new System.Drawing.Size(757, 521);
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
      this.btnLoadMore.Location = new System.Drawing.Point(679, 491);
      this.btnLoadMore.Name = "btnLoadMore";
      this.btnLoadMore.Padding = new System.Windows.Forms.Padding(2);
      this.btnLoadMore.Size = new System.Drawing.Size(75, 27);
      this.btnLoadMore.TabIndex = 1;
      this.btnLoadMore.Text = "Load More";
      this.btnLoadMore.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnLoadMore.UseVisualStyleBackColor = false;
      this.btnLoadMore.Click += new System.EventHandler(this.btnLoadMore_Click);
      // 
      // lblCount
      // 
      this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.lblCount.AutoSize = true;
      this.lblCount.Location = new System.Drawing.Point(673, 498);
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
      this.Size = new System.Drawing.Size(757, 521);
      this.tabControl1.ResumeLayout(false);
      this.pgSimple.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tableLayoutPanel6.ResumeLayout(false);
      this.tableLayoutPanel6.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridPreview)).EndInit();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudBatchSize)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgvMappings)).EndInit();
      this.flowLayoutPanel2.ResumeLayout(false);
      this.flowLayoutPanel2.PerformLayout();
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
    private System.Windows.Forms.DataGridView dgvMappings;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox cboItemTypes;
    private System.Windows.Forms.ComboBox cboProperties;
    private System.Windows.Forms.TextBox txtValue;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label lblValue;
    private FlatButton btnAdd;
    private System.Windows.Forms.CheckBox chkCalculated;
    private FlatButton btnDelete;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    private System.Windows.Forms.CheckBox chkPreventDuplicateFiles;
    private System.Windows.Forms.CheckBox chkPreventDuplicateDocs;
    private System.Windows.Forms.Label label9;
  }
}
