namespace InnovatorAdmin.Controls
{
  partial class MergeInterface
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
      this.grid = new System.Windows.Forms.DataGridView();
      this.colPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colStatusDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnActions = new InnovatorAdmin.Controls.FlatButton();
      this.chkStatus = new System.Windows.Forms.CheckedListBox();
      this.lblFilter = new System.Windows.Forms.Label();
      this.txtFilter = new System.Windows.Forms.TextBox();
      this.conActions = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mniMarkUnresolved = new System.Windows.Forms.ToolStripMenuItem();
      this.mniMerge = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTakeLocal = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTakeRemote = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.conActions.SuspendLayout();
      this.SuspendLayout();
      // 
      // grid
      // 
      this.grid.AllowUserToAddRows = false;
      this.grid.AllowUserToDeleteRows = false;
      this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPath,
            this.colStatus,
            this.colStatusDescription});
      this.grid.Location = new System.Drawing.Point(3, 38);
      this.grid.Name = "grid";
      this.grid.ReadOnly = true;
      this.tableLayoutPanel1.SetRowSpan(this.grid, 3);
      this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.grid.Size = new System.Drawing.Size(437, 347);
      this.grid.TabIndex = 0;
      this.grid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
      this.grid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseDoubleClick);
      this.grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grid_KeyDown);
      // 
      // colPath
      // 
      this.colPath.DataPropertyName = "Path";
      this.colPath.HeaderText = "Path";
      this.colPath.Name = "colPath";
      this.colPath.ReadOnly = true;
      this.colPath.Width = 300;
      // 
      // colStatus
      // 
      this.colStatus.DataPropertyName = "ResolutionStatus";
      this.colStatus.HeaderText = "Status";
      this.colStatus.Name = "colStatus";
      this.colStatus.ReadOnly = true;
      this.colStatus.Width = 80;
      // 
      // colStatusDescription
      // 
      this.colStatusDescription.DataPropertyName = "StatusDescription";
      this.colStatusDescription.HeaderText = "Status Description";
      this.colStatusDescription.Name = "colStatusDescription";
      this.colStatusDescription.ReadOnly = true;
      this.colStatusDescription.Width = 220;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.btnActions, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.grid, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.chkStatus, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.lblFilter, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtFilter, 1, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(569, 388);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // btnActions
      // 
      this.btnActions.AutoSize = true;
      this.btnActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnActions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnActions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnActions.ForeColor = System.Drawing.Color.Black;
      this.btnActions.Location = new System.Drawing.Point(3, 3);
      this.btnActions.Name = "btnActions";
      this.btnActions.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnActions.Padding = new System.Windows.Forms.Padding(2);
      this.btnActions.Size = new System.Drawing.Size(75, 29);
      this.btnActions.TabIndex = 4;
      this.btnActions.Text = "Actions ▼";
      this.btnActions.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnActions.UseVisualStyleBackColor = false;
      this.btnActions.Click += new System.EventHandler(this.btnActions_Click);
      // 
      // chkStatus
      // 
      this.chkStatus.FormattingEnabled = true;
      this.chkStatus.Location = new System.Drawing.Point(446, 64);
      this.chkStatus.Name = "chkStatus";
      this.chkStatus.Size = new System.Drawing.Size(120, 94);
      this.chkStatus.TabIndex = 1;
      this.chkStatus.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkStatus_ItemCheck);
      // 
      // lblFilter
      // 
      this.lblFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblFilter.AutoSize = true;
      this.lblFilter.Location = new System.Drawing.Point(446, 19);
      this.lblFilter.Margin = new System.Windows.Forms.Padding(3);
      this.lblFilter.Name = "lblFilter";
      this.lblFilter.Size = new System.Drawing.Size(29, 13);
      this.lblFilter.TabIndex = 2;
      this.lblFilter.Text = "Filter";
      // 
      // txtFilter
      // 
      this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFilter.Location = new System.Drawing.Point(446, 38);
      this.txtFilter.Name = "txtFilter";
      this.txtFilter.Size = new System.Drawing.Size(120, 20);
      this.txtFilter.TabIndex = 3;
      this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
      // 
      // conActions
      // 
      this.conActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniMarkUnresolved,
            this.mniMerge,
            this.mniTakeLocal,
            this.mniTakeRemote});
      this.conActions.Name = "conActions";
      this.conActions.Size = new System.Drawing.Size(164, 92);
      // 
      // mniMarkUnresolved
      // 
      this.mniMarkUnresolved.Name = "mniMarkUnresolved";
      this.mniMarkUnresolved.Size = new System.Drawing.Size(163, 22);
      this.mniMarkUnresolved.Text = "Mark Unresolved";
      this.mniMarkUnresolved.Click += new System.EventHandler(this.mniMarkUnresolved_Click);
      // 
      // mniMerge
      // 
      this.mniMerge.Name = "mniMerge";
      this.mniMerge.Size = new System.Drawing.Size(163, 22);
      this.mniMerge.Text = "Merge";
      this.mniMerge.Click += new System.EventHandler(this.mniMerge_Click);
      // 
      // mniTakeLocal
      // 
      this.mniTakeLocal.Name = "mniTakeLocal";
      this.mniTakeLocal.Size = new System.Drawing.Size(163, 22);
      this.mniTakeLocal.Text = "Take Local";
      this.mniTakeLocal.Click += new System.EventHandler(this.mniTakeLocal_Click);
      // 
      // mniTakeRemote
      // 
      this.mniTakeRemote.Name = "mniTakeRemote";
      this.mniTakeRemote.Size = new System.Drawing.Size(163, 22);
      this.mniTakeRemote.Text = "Take Remote";
      this.mniTakeRemote.Click += new System.EventHandler(this.mniTakeRemote_Click);
      // 
      // MergeInterface
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "MergeInterface";
      this.Size = new System.Drawing.Size(569, 388);
      ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.conActions.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView grid;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.CheckedListBox chkStatus;
    private System.Windows.Forms.Label lblFilter;
    private System.Windows.Forms.TextBox txtFilter;
    private FlatButton btnActions;
    private System.Windows.Forms.ContextMenuStrip conActions;
    private System.Windows.Forms.ToolStripMenuItem mniMarkUnresolved;
    private System.Windows.Forms.ToolStripMenuItem mniMerge;
    private System.Windows.Forms.ToolStripMenuItem mniTakeLocal;
    private System.Windows.Forms.ToolStripMenuItem mniTakeRemote;
    private System.Windows.Forms.DataGridViewTextBoxColumn colPath;
    private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    private System.Windows.Forms.DataGridViewTextBoxColumn colStatusDescription;
  }
}
