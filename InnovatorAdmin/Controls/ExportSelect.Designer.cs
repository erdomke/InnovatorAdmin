namespace Aras.Tools.InnovatorAdmin.Controls
{
  partial class ExportSelect
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
      this.gridSelected = new System.Windows.Forms.DataGridView();
      this.colTypeSelected = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colNameSelected = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.gridAvailable = new System.Windows.Forms.DataGridView();
      this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tbcSearch = new System.Windows.Forms.TabControl();
      this.pgSearchBy = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnDbPackage = new System.Windows.Forms.Button();
      this.btnPackageFile = new System.Windows.Forms.Button();
      this.btnItem = new System.Windows.Forms.Button();
      this.btnPackageFolder = new System.Windows.Forms.Button();
      this.btnAmlStudio = new System.Windows.Forms.Button();
      this.pgResults = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.txtFind = new System.Windows.Forms.TextBox();
      this.btnFind = new System.Windows.Forms.Button();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSelectAll = new System.Windows.Forms.Button();
      this.btnSelect = new System.Windows.Forms.Button();
      this.btnUnselect = new System.Windows.Forms.Button();
      this.btnUnselectAll = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.toolTipManager = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.gridSelected)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridAvailable)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tbcSearch.SuspendLayout();
      this.pgSearchBy.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.pgResults.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.SuspendLayout();
      // 
      // gridSelected
      // 
      this.gridSelected.AllowUserToAddRows = false;
      this.gridSelected.AllowUserToDeleteRows = false;
      this.gridSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gridSelected.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridSelected.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTypeSelected,
            this.colNameSelected});
      this.gridSelected.Location = new System.Drawing.Point(341, 23);
      this.gridSelected.Name = "gridSelected";
      this.gridSelected.ReadOnly = true;
      this.gridSelected.RowHeadersVisible = false;
      this.gridSelected.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.gridSelected.Size = new System.Drawing.Size(303, 402);
      this.gridSelected.TabIndex = 1;
      this.gridSelected.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridSelected_CellMouseDoubleClick);
      // 
      // colTypeSelected
      // 
      this.colTypeSelected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colTypeSelected.DataPropertyName = "Type";
      this.colTypeSelected.Frozen = true;
      this.colTypeSelected.HeaderText = "Type";
      this.colTypeSelected.MinimumWidth = 100;
      this.colTypeSelected.Name = "colTypeSelected";
      this.colTypeSelected.ReadOnly = true;
      // 
      // colNameSelected
      // 
      this.colNameSelected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.colNameSelected.DataPropertyName = "KeyedName";
      this.colNameSelected.HeaderText = "Name";
      this.colNameSelected.MinimumWidth = 150;
      this.colNameSelected.Name = "colNameSelected";
      this.colNameSelected.ReadOnly = true;
      // 
      // gridAvailable
      // 
      this.gridAvailable.AllowUserToAddRows = false;
      this.gridAvailable.AllowUserToDeleteRows = false;
      this.gridAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gridAvailable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gridAvailable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType,
            this.colName});
      this.tableLayoutPanel5.SetColumnSpan(this.gridAvailable, 2);
      this.gridAvailable.Location = new System.Drawing.Point(3, 32);
      this.gridAvailable.Name = "gridAvailable";
      this.gridAvailable.ReadOnly = true;
      this.gridAvailable.RowHeadersVisible = false;
      this.gridAvailable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.gridAvailable.Size = new System.Drawing.Size(283, 335);
      this.gridAvailable.StandardTab = true;
      this.gridAvailable.TabIndex = 0;
      this.gridAvailable.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridAvailable_CellMouseDoubleClick);
      this.gridAvailable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridAvailable_KeyDown);
      // 
      // colType
      // 
      this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colType.DataPropertyName = "Type";
      this.colType.Frozen = true;
      this.colType.HeaderText = "Type";
      this.colType.MinimumWidth = 100;
      this.colType.Name = "colType";
      this.colType.ReadOnly = true;
      // 
      // colName
      // 
      this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.colName.DataPropertyName = "KeyedName";
      this.colName.HeaderText = "Name";
      this.colName.MinimumWidth = 150;
      this.colName.Name = "colName";
      this.colName.ReadOnly = true;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.tbcSearch, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.gridSelected, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(647, 428);
      this.tableLayoutPanel1.TabIndex = 2;
      // 
      // tbcSearch
      // 
      this.tbcSearch.Controls.Add(this.pgSearchBy);
      this.tbcSearch.Controls.Add(this.pgResults);
      this.tbcSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbcSearch.Location = new System.Drawing.Point(3, 23);
      this.tbcSearch.Name = "tbcSearch";
      this.tbcSearch.SelectedIndex = 0;
      this.tbcSearch.Size = new System.Drawing.Size(303, 402);
      this.tbcSearch.TabIndex = 0;
      // 
      // pgSearchBy
      // 
      this.pgSearchBy.Controls.Add(this.tableLayoutPanel2);
      this.pgSearchBy.Location = new System.Drawing.Point(4, 22);
      this.pgSearchBy.Name = "pgSearchBy";
      this.pgSearchBy.Padding = new System.Windows.Forms.Padding(3);
      this.pgSearchBy.Size = new System.Drawing.Size(295, 376);
      this.pgSearchBy.TabIndex = 0;
      this.pgSearchBy.Text = "List items by...";
      this.pgSearchBy.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.AutoSize = true;
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.btnDbPackage, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnPackageFile, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.btnItem, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnPackageFolder, 1, 2);
      this.tableLayoutPanel2.Controls.Add(this.btnAmlStudio, 0, 3);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 4;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.Size = new System.Drawing.Size(289, 370);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // btnDbPackage
      // 
      this.btnDbPackage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDbPackage.AutoSize = true;
      this.btnDbPackage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDbPackage.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.packageDefinition;
      this.btnDbPackage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnDbPackage.Location = new System.Drawing.Point(3, 3);
      this.btnDbPackage.Name = "btnDbPackage";
      this.btnDbPackage.Size = new System.Drawing.Size(138, 70);
      this.btnDbPackage.TabIndex = 0;
      this.btnDbPackage.Text = "Package Definition (Db)";
      this.btnDbPackage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.toolTipManager.SetToolTip(this.btnDbPackage, "Use legacy Innovator package stored in source database.");
      this.btnDbPackage.UseVisualStyleBackColor = true;
      this.btnDbPackage.Click += new System.EventHandler(this.btnDbPackage_Click);
      // 
      // btnPackageFile
      // 
      this.btnPackageFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPackageFile.AutoSize = true;
      this.btnPackageFile.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.innPkg32;
      this.btnPackageFile.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnPackageFile.Location = new System.Drawing.Point(3, 79);
      this.btnPackageFile.Name = "btnPackageFile";
      this.btnPackageFile.Size = new System.Drawing.Size(138, 70);
      this.btnPackageFile.TabIndex = 2;
      this.btnPackageFile.Text = "Innovator Package (File)";
      this.btnPackageFile.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.toolTipManager.SetToolTip(this.btnPackageFile, "Use previously created Innovator package file (*.innpkg)");
      this.btnPackageFile.UseVisualStyleBackColor = true;
      this.btnPackageFile.Click += new System.EventHandler(this.btnPackageFile_Click);
      // 
      // btnItem
      // 
      this.btnItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnItem.AutoSize = true;
      this.btnItem.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.search32;
      this.btnItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnItem.Location = new System.Drawing.Point(147, 3);
      this.btnItem.Name = "btnItem";
      this.btnItem.Size = new System.Drawing.Size(139, 70);
      this.btnItem.TabIndex = 1;
      this.btnItem.Text = "Search (Db)";
      this.btnItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.toolTipManager.SetToolTip(this.btnItem, "Select items directly from source database.");
      this.btnItem.UseVisualStyleBackColor = true;
      this.btnItem.Click += new System.EventHandler(this.btnItem_Click);
      // 
      // btnPackageFolder
      // 
      this.btnPackageFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPackageFolder.AutoSize = true;
      this.btnPackageFolder.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.manifestFolder32;
      this.btnPackageFolder.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnPackageFolder.Location = new System.Drawing.Point(147, 79);
      this.btnPackageFolder.Name = "btnPackageFolder";
      this.btnPackageFolder.Size = new System.Drawing.Size(139, 70);
      this.btnPackageFolder.TabIndex = 3;
      this.btnPackageFolder.Text = "Legacy Package (Folder)";
      this.btnPackageFolder.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.toolTipManager.SetToolTip(this.btnPackageFolder, "Use previously exported legacy package folder as source.");
      this.btnPackageFolder.UseVisualStyleBackColor = true;
      // 
      // btnAmlStudio
      // 
      this.btnAmlStudio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAmlStudio.AutoSize = true;
      this.btnAmlStudio.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.amlStudio32black;
      this.btnAmlStudio.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnAmlStudio.Location = new System.Drawing.Point(3, 155);
      this.btnAmlStudio.Name = "btnAmlStudio";
      this.btnAmlStudio.Size = new System.Drawing.Size(138, 70);
      this.btnAmlStudio.TabIndex = 4;
      this.btnAmlStudio.Text = "Advanced Search";
      this.btnAmlStudio.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.toolTipManager.SetToolTip(this.btnAmlStudio, "Use previously created Innovator package file (*.innpkg)");
      this.btnAmlStudio.UseVisualStyleBackColor = true;
      this.btnAmlStudio.Click += new System.EventHandler(this.btnAmlStudio_Click);
      // 
      // pgResults
      // 
      this.pgResults.Controls.Add(this.tableLayoutPanel5);
      this.pgResults.Location = new System.Drawing.Point(4, 22);
      this.pgResults.Name = "pgResults";
      this.pgResults.Padding = new System.Windows.Forms.Padding(3);
      this.pgResults.Size = new System.Drawing.Size(295, 376);
      this.pgResults.TabIndex = 1;
      this.pgResults.Text = "Search Results";
      this.pgResults.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 2;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.Controls.Add(this.txtFind, 0, 0);
      this.tableLayoutPanel5.Controls.Add(this.gridAvailable, 0, 1);
      this.tableLayoutPanel5.Controls.Add(this.btnFind, 1, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 2;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(289, 370);
      this.tableLayoutPanel5.TabIndex = 0;
      // 
      // txtFind
      // 
      this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFind.Location = new System.Drawing.Point(3, 3);
      this.txtFind.Name = "txtFind";
      this.txtFind.Size = new System.Drawing.Size(226, 20);
      this.txtFind.TabIndex = 0;
      this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
      this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
      // 
      // btnFind
      // 
      this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFind.AutoSize = true;
      this.btnFind.Location = new System.Drawing.Point(235, 3);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(51, 23);
      this.btnFind.TabIndex = 1;
      this.btnFind.Text = "Find";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.AutoSize = true;
      this.tableLayoutPanel4.ColumnCount = 1;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.Controls.Add(this.btnSelectAll, 0, 3);
      this.tableLayoutPanel4.Controls.Add(this.btnSelect, 0, 2);
      this.tableLayoutPanel4.Controls.Add(this.btnUnselect, 0, 1);
      this.tableLayoutPanel4.Controls.Add(this.btnUnselectAll, 0, 0);
      this.tableLayoutPanel4.Location = new System.Drawing.Point(309, 20);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 4;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.Size = new System.Drawing.Size(29, 116);
      this.tableLayoutPanel4.TabIndex = 4;
      // 
      // btnSelectAll
      // 
      this.btnSelectAll.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.arrowRightAllSimpleBlack16;
      this.btnSelectAll.Location = new System.Drawing.Point(3, 90);
      this.btnSelectAll.Name = "btnSelectAll";
      this.btnSelectAll.Size = new System.Drawing.Size(23, 23);
      this.btnSelectAll.TabIndex = 3;
      this.btnSelectAll.UseVisualStyleBackColor = true;
      this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
      // 
      // btnSelect
      // 
      this.btnSelect.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.arrowRightSimpleBlack16;
      this.btnSelect.Location = new System.Drawing.Point(3, 61);
      this.btnSelect.Name = "btnSelect";
      this.btnSelect.Size = new System.Drawing.Size(23, 23);
      this.btnSelect.TabIndex = 2;
      this.btnSelect.UseVisualStyleBackColor = true;
      this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
      // 
      // btnUnselect
      // 
      this.btnUnselect.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.arrowLeftSimpleBlack16;
      this.btnUnselect.Location = new System.Drawing.Point(3, 32);
      this.btnUnselect.Name = "btnUnselect";
      this.btnUnselect.Size = new System.Drawing.Size(23, 23);
      this.btnUnselect.TabIndex = 1;
      this.btnUnselect.UseVisualStyleBackColor = true;
      this.btnUnselect.Click += new System.EventHandler(this.btnUnselect_Click);
      // 
      // btnUnselectAll
      // 
      this.btnUnselectAll.Image = global::Aras.Tools.InnovatorAdmin.Properties.Resources.arrowLeftAllSimpleBlack16;
      this.btnUnselectAll.Location = new System.Drawing.Point(3, 3);
      this.btnUnselectAll.Name = "btnUnselectAll";
      this.btnUnselectAll.Size = new System.Drawing.Size(23, 23);
      this.btnUnselectAll.TabIndex = 0;
      this.btnUnselectAll.UseVisualStyleBackColor = true;
      this.btnUnselectAll.Click += new System.EventHandler(this.btnUnselectAll_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 3);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(96, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Available items:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(341, 3);
      this.label3.Margin = new System.Windows.Forms.Padding(3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(151, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "Items selected for export:";
      // 
      // ExportSelect
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "ExportSelect";
      this.Size = new System.Drawing.Size(647, 428);
      ((System.ComponentModel.ISupportInitialize)(this.gridSelected)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridAvailable)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tbcSearch.ResumeLayout(false);
      this.pgSearchBy.ResumeLayout(false);
      this.pgSearchBy.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.pgResults.ResumeLayout(false);
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView gridSelected;
    private System.Windows.Forms.DataGridView gridAvailable;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnDbPackage;
    private System.Windows.Forms.Button btnItem;
    private System.Windows.Forms.Button btnPackageFile;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.TextBox txtFind;
    private System.Windows.Forms.Button btnSelectAll;
    private System.Windows.Forms.Button btnSelect;
    private System.Windows.Forms.Button btnUnselect;
    private System.Windows.Forms.Button btnUnselectAll;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.TabControl tbcSearch;
    private System.Windows.Forms.TabPage pgSearchBy;
    private System.Windows.Forms.Button btnPackageFolder;
    private System.Windows.Forms.TabPage pgResults;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.DataGridViewTextBoxColumn colTypeSelected;
    private System.Windows.Forms.DataGridViewTextBoxColumn colNameSelected;
    private System.Windows.Forms.DataGridViewTextBoxColumn colType;
    private System.Windows.Forms.DataGridViewTextBoxColumn colName;
    private System.Windows.Forms.ToolTip toolTipManager;
    private System.Windows.Forms.Button btnAmlStudio;
  }
}
