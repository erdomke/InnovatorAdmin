namespace InnovatorAdmin
{
  partial class EditorWindow
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorWindow));
      this.tblMain = new System.Windows.Forms.TableLayoutPanel();
      this.tblHeader = new System.Windows.Forms.TableLayoutPanel();
      this.lblClose = new System.Windows.Forms.Label();
      this.lblTitle = new System.Windows.Forms.Label();
      this.menuStrip = new System.Windows.Forms.ToolStrip();
      this.lblConnection = new System.Windows.Forms.ToolStripLabel();
      this.btnEditConnections = new System.Windows.Forms.ToolStripButton();
      this.lblSoapAction = new System.Windows.Forms.ToolStripLabel();
      this.btnSoapAction = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
      this.mniNewWindow = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.mniClose = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
      this.mniUndo = new System.Windows.Forms.ToolStripMenuItem();
      this.mniRedo = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.mniCut = new System.Windows.Forms.ToolStripMenuItem();
      this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
      this.mniPaste = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
      this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mniInsertNewGuid = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.codeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTidy = new System.Windows.Forms.ToolStripMenuItem();
      this.mniMinify = new System.Windows.Forms.ToolStripMenuItem();
      this.commentUncommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mniToggleSingleLineComment = new System.Windows.Forms.ToolStripMenuItem();
      this.mniSingleLineComment = new System.Windows.Forms.ToolStripMenuItem();
      this.mniSingleLineUncomment = new System.Windows.Forms.ToolStripMenuItem();
      this.mniBlockComment = new System.Windows.Forms.ToolStripMenuItem();
      this.mniBlockUncomment = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.mniUppercase = new System.Windows.Forms.ToolStripMenuItem();
      this.mniLowercase = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
      this.mniDoubleToSingleQuotes = new System.Windows.Forms.ToolStripMenuItem();
      this.mniSingleToDoubleQuotes = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
      this.mniMd5Encode = new System.Windows.Forms.ToolStripMenuItem();
      this.lineOperationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mniMoveUpCurrentLine = new System.Windows.Forms.ToolStripMenuItem();
      this.mniMoveDownCurrentLine = new System.Windows.Forms.ToolStripMenuItem();
      this.xMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mniXmlToEntity = new System.Windows.Forms.ToolStripMenuItem();
      this.mniEntityToXml = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripDropDownButton4 = new System.Windows.Forms.ToolStripDropDownButton();
      this.mniFind = new System.Windows.Forms.ToolStripMenuItem();
      this.mniFindNext = new System.Windows.Forms.ToolStripMenuItem();
      this.mniFindPrevious = new System.Windows.Forms.ToolStripMenuItem();
      this.mniReplace = new System.Windows.Forms.ToolStripMenuItem();
      this.mniGoTo = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
      this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mniLocale = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTimeZone = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTimeout = new System.Windows.Forms.ToolStripMenuItem();
      this.exploreButton = new System.Windows.Forms.ToolStripButton();
      this.btnSubmit = new System.Windows.Forms.ToolStripSplitButton();
      this.mniRunAll = new System.Windows.Forms.ToolStripMenuItem();
      this.mniRunCurrent = new System.Windows.Forms.ToolStripMenuItem();
      this.mniRunCurrentNewWindow = new System.Windows.Forms.ToolStripMenuItem();
      this.lblMaximize = new System.Windows.Forms.Label();
      this.lblMinimize = new System.Windows.Forms.Label();
      this.picLogo = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.lblSelection = new System.Windows.Forms.Label();
      this.lblVersion = new System.Windows.Forms.Label();
      this.lblItems = new System.Windows.Forms.Label();
      this.splitMain = new InnovatorAdmin.Controls.SplitContainerTheme();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.treeItems = new InnovatorAdmin.TreeListView();
      this.colName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.colDescription = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.btnPanelToggle = new InnovatorAdmin.Controls.FlatButton();
      this.splitEditors = new InnovatorAdmin.Controls.SplitContainerTheme();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.inputEditor = new InnovatorAdmin.Editor.FullEditor();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.tbcOutputView = new InnovatorAdmin.Controls.FlatTabControl();
      this.pgTools = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.btnInstall = new InnovatorAdmin.Controls.FlatButton();
      this.btnCreate = new InnovatorAdmin.Controls.FlatButton();
      this.pgTextOutput = new System.Windows.Forms.TabPage();
      this.outputEditor = new InnovatorAdmin.Editor.FullEditor();
      this.pgHtml = new System.Windows.Forms.TabPage();
      this.browser = new System.Windows.Forms.WebBrowser();
      this.pgTableOutput = new System.Windows.Forms.TabPage();
      this.dgvItems = new Controls.DataGrid();
      this.conTable = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mniColumns = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
      this.scriptEditsToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTableEditsToClipboard = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTableEditsToFile = new System.Windows.Forms.ToolStripMenuItem();
      this.mniTableEditsToQueryEditor = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.mniAcceptChanges = new System.Windows.Forms.ToolStripMenuItem();
      this.mniResetChanges = new System.Windows.Forms.ToolStripMenuItem();
      this.btnOk = new InnovatorAdmin.Controls.FlatButton();
      this.btnCancel = new InnovatorAdmin.Controls.FlatButton();
      this.pnlTopLeft = new System.Windows.Forms.Panel();
      this.pnlTop = new System.Windows.Forms.Panel();
      this.pnlTopRight = new System.Windows.Forms.Panel();
      this.pnlBottomLeft = new System.Windows.Forms.Panel();
      this.pnlBottom = new System.Windows.Forms.Panel();
      this.pnlBottomRight = new System.Windows.Forms.Panel();
      this.pnlRight = new InnovatorAdmin.DropShadow();
      this.pnlLeft = new InnovatorAdmin.DropShadow();
      this.pnlLeftTop = new System.Windows.Forms.Panel();
      this.pnlRightTop = new System.Windows.Forms.Panel();
      this.dropShadow1 = new InnovatorAdmin.DropShadow();
      this.tblMain.SuspendLayout();
      this.tblHeader.SuspendLayout();
      this.menuStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
      this.tableLayoutPanel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
      this.splitMain.Panel1.SuspendLayout();
      this.splitMain.Panel2.SuspendLayout();
      this.splitMain.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.treeItems)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitEditors)).BeginInit();
      this.splitEditors.Panel1.SuspendLayout();
      this.splitEditors.Panel2.SuspendLayout();
      this.splitEditors.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tbcOutputView.SuspendLayout();
      this.pgTools.SuspendLayout();
      this.tableLayoutPanel6.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.pgTextOutput.SuspendLayout();
      this.pgHtml.SuspendLayout();
      this.pgTableOutput.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
      this.conTable.SuspendLayout();
      this.SuspendLayout();
      //
      // tblMain
      //
      this.tblMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tblMain.BackColor = System.Drawing.Color.White;
      this.tblMain.ColumnCount = 5;
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.Controls.Add(this.tblHeader, 1, 1);
      this.tblMain.Controls.Add(this.tableLayoutPanel5, 1, 4);
      this.tblMain.Controls.Add(this.splitMain, 1, 3);
      this.tblMain.Controls.Add(this.btnOk, 2, 4);
      this.tblMain.Controls.Add(this.btnCancel, 3, 4);
      this.tblMain.Controls.Add(this.pnlTopLeft, 0, 0);
      this.tblMain.Controls.Add(this.pnlTop, 1, 0);
      this.tblMain.Controls.Add(this.pnlTopRight, 4, 0);
      this.tblMain.Controls.Add(this.pnlBottomLeft, 0, 5);
      this.tblMain.Controls.Add(this.pnlBottom, 1, 5);
      this.tblMain.Controls.Add(this.pnlBottomRight, 4, 5);
      this.tblMain.Controls.Add(this.pnlRight, 4, 2);
      this.tblMain.Controls.Add(this.pnlLeft, 0, 2);
      this.tblMain.Controls.Add(this.pnlLeftTop, 0, 1);
      this.tblMain.Controls.Add(this.pnlRightTop, 4, 1);
      this.tblMain.Controls.Add(this.dropShadow1, 1, 2);
      this.tblMain.Location = new System.Drawing.Point(1, 1);
      this.tblMain.Name = "tblMain";
      this.tblMain.RowCount = 6;
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
      this.tblMain.Size = new System.Drawing.Size(732, 506);
      this.tblMain.TabIndex = 0;
      //
      // tblHeader
      //
      this.tblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tblHeader.AutoSize = true;
      this.tblHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
      this.tblHeader.ColumnCount = 5;
      this.tblMain.SetColumnSpan(this.tblHeader, 3);
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tblHeader.Controls.Add(this.lblClose, 4, 0);
      this.tblHeader.Controls.Add(this.lblTitle, 1, 0);
      this.tblHeader.Controls.Add(this.menuStrip, 1, 1);
      this.tblHeader.Controls.Add(this.lblMaximize, 3, 0);
      this.tblHeader.Controls.Add(this.lblMinimize, 2, 0);
      this.tblHeader.Controls.Add(this.picLogo, 0, 0);
      this.tblHeader.Location = new System.Drawing.Point(3, 3);
      this.tblHeader.Margin = new System.Windows.Forms.Padding(0);
      this.tblHeader.Name = "tblHeader";
      this.tblHeader.RowCount = 2;
      this.tblHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tblHeader.Size = new System.Drawing.Size(726, 63);
      this.tblHeader.TabIndex = 1;
      //
      // lblClose
      //
      this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblClose.BackColor = System.Drawing.Color.Transparent;
      this.lblClose.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblClose.Location = new System.Drawing.Point(702, 0);
      this.lblClose.Margin = new System.Windows.Forms.Padding(0);
      this.lblClose.Name = "lblClose";
      this.lblClose.Size = new System.Drawing.Size(24, 32);
      this.lblClose.TabIndex = 14;
      this.lblClose.Text = "r";
      this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      //
      // lblTitle
      //
      this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblTitle.BackColor = System.Drawing.Color.Transparent;
      this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.ForeColor = System.Drawing.SystemColors.ControlText;
      this.lblTitle.Location = new System.Drawing.Point(80, 0);
      this.lblTitle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new System.Drawing.Size(574, 32);
      this.lblTitle.TabIndex = 11;
      this.lblTitle.Text = "Title";
      this.lblTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      //
      // menuStrip
      //
      this.menuStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.menuStrip.BackColor = System.Drawing.Color.Transparent;
      this.tblHeader.SetColumnSpan(this.menuStrip, 4);
      this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
      this.menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblConnection,
            this.btnEditConnections,
            this.lblSoapAction,
            this.btnSoapAction,
            this.toolStripSeparator2,
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton4,
            this.toolStripDropDownButton3,
            this.exploreButton,
            this.btnSubmit});
      this.menuStrip.Location = new System.Drawing.Point(80, 38);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(646, 25);
      this.menuStrip.TabIndex = 1;
      this.menuStrip.Text = "toolStrip1";
      //
      // lblConnection
      //
      this.lblConnection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblConnection.ForeColor = System.Drawing.Color.Black;
      this.lblConnection.Name = "lblConnection";
      this.lblConnection.Size = new System.Drawing.Size(15, 22);
      this.lblConnection.Text = "C";
      //
      // btnEditConnections
      //
      this.btnEditConnections.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnEditConnections.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnEditConnections.Name = "btnEditConnections";
      this.btnEditConnections.Size = new System.Drawing.Size(105, 22);
      this.btnEditConnections.Text = "Not Connected ▼";
      this.btnEditConnections.ToolTipText = "Change Connection (Ctrl+O)";
      //
      // lblSoapAction
      //
      this.lblSoapAction.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSoapAction.ForeColor = System.Drawing.Color.Black;
      this.lblSoapAction.Name = "lblSoapAction";
      this.lblSoapAction.Size = new System.Drawing.Size(15, 22);
      this.lblSoapAction.Text = "A";
      //
      // btnSoapAction
      //
      this.btnSoapAction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnSoapAction.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSoapAction.Name = "btnSoapAction";
      this.btnSoapAction.Size = new System.Drawing.Size(46, 22);
      this.btnSoapAction.Text = "Action";
      this.btnSoapAction.ToolTipText = "Change Action (Ctrl+M)";
      //
      // toolStripSeparator2
      //
      this.toolStripSeparator2.AutoSize = false;
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(25, 25);
      //
      // toolStripDropDownButton2
      //
      this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniNewWindow,
            this.toolStripSeparator3,
            this.mniClose});
      this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
      this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
      this.toolStripDropDownButton2.ShowDropDownArrow = false;
      this.toolStripDropDownButton2.Size = new System.Drawing.Size(29, 22);
      this.toolStripDropDownButton2.Text = "&File";
      //
      // mniNewWindow
      //
      this.mniNewWindow.Name = "mniNewWindow";
      this.mniNewWindow.Size = new System.Drawing.Size(145, 22);
      this.mniNewWindow.Text = "New Window";
      //
      // toolStripSeparator3
      //
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(142, 6);
      //
      // mniClose
      //
      this.mniClose.Name = "mniClose";
      this.mniClose.Size = new System.Drawing.Size(145, 22);
      this.mniClose.Text = "Close";
      this.mniClose.Click += new System.EventHandler(this.mniClose_Click);
      //
      // toolStripDropDownButton1
      //
      this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniUndo,
            this.mniRedo,
            this.toolStripSeparator4,
            this.mniCut,
            this.mniCopy,
            this.mniPaste,
            this.toolStripSeparator6,
            this.insertToolStripMenuItem,
            this.toolStripSeparator5,
            this.codeToolStripMenuItem,
            this.commentUncommentToolStripMenuItem,
            this.toolStripMenuItem1,
            this.lineOperationsToolStripMenuItem,
            this.xMLToolStripMenuItem});
      this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
      this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
      this.toolStripDropDownButton1.ShowDropDownArrow = false;
      this.toolStripDropDownButton1.Size = new System.Drawing.Size(31, 22);
      this.toolStripDropDownButton1.Text = "&Edit";
      //
      // mniUndo
      //
      this.mniUndo.Name = "mniUndo";
      this.mniUndo.ShortcutKeyDisplayString = "Ctrl+Z";
      this.mniUndo.Size = new System.Drawing.Size(200, 22);
      this.mniUndo.Text = "Undo";
      //
      // mniRedo
      //
      this.mniRedo.Name = "mniRedo";
      this.mniRedo.ShortcutKeyDisplayString = "Ctrl+Y";
      this.mniRedo.Size = new System.Drawing.Size(200, 22);
      this.mniRedo.Text = "Redo";
      //
      // toolStripSeparator4
      //
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(197, 6);
      //
      // mniCut
      //
      this.mniCut.Name = "mniCut";
      this.mniCut.ShortcutKeyDisplayString = "Ctrl+X";
      this.mniCut.Size = new System.Drawing.Size(200, 22);
      this.mniCut.Text = "Cut";
      //
      // mniCopy
      //
      this.mniCopy.Name = "mniCopy";
      this.mniCopy.ShortcutKeyDisplayString = "Ctrl+C";
      this.mniCopy.Size = new System.Drawing.Size(200, 22);
      this.mniCopy.Text = "Copy";
      //
      // mniPaste
      //
      this.mniPaste.Name = "mniPaste";
      this.mniPaste.ShortcutKeyDisplayString = "Ctrl+V";
      this.mniPaste.Size = new System.Drawing.Size(200, 22);
      this.mniPaste.Text = "Paste";
      //
      // toolStripSeparator6
      //
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new System.Drawing.Size(197, 6);
      //
      // insertToolStripMenuItem
      //
      this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniInsertNewGuid});
      this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
      this.insertToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
      this.insertToolStripMenuItem.Text = "Insert";
      //
      // mniInsertNewGuid
      //
      this.mniInsertNewGuid.Name = "mniInsertNewGuid";
      this.mniInsertNewGuid.Size = new System.Drawing.Size(128, 22);
      this.mniInsertNewGuid.Text = "New GUID";
      //
      // toolStripSeparator5
      //
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(197, 6);
      //
      // codeToolStripMenuItem
      //
      this.codeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniTidy,
            this.mniMinify});
      this.codeToolStripMenuItem.Name = "codeToolStripMenuItem";
      this.codeToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
      this.codeToolStripMenuItem.Text = "Code";
      //
      // mniTidy
      //
      this.mniTidy.Name = "mniTidy";
      this.mniTidy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
      this.mniTidy.Size = new System.Drawing.Size(221, 22);
      this.mniTidy.Text = "Pretty Format / Tidy";
      //
      // mniMinify
      //
      this.mniMinify.Name = "mniMinify";
      this.mniMinify.Size = new System.Drawing.Size(221, 22);
      this.mniMinify.Text = "Minify";
      //
      // commentUncommentToolStripMenuItem
      //
      this.commentUncommentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniToggleSingleLineComment,
            this.mniSingleLineComment,
            this.mniSingleLineUncomment,
            this.mniBlockComment,
            this.mniBlockUncomment});
      this.commentUncommentToolStripMenuItem.Name = "commentUncommentToolStripMenuItem";
      this.commentUncommentToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
      this.commentUncommentToolStripMenuItem.Text = "Comment/Uncomment";
      //
      // mniToggleSingleLineComment
      //
      this.mniToggleSingleLineComment.Name = "mniToggleSingleLineComment";
      this.mniToggleSingleLineComment.Size = new System.Drawing.Size(228, 22);
      this.mniToggleSingleLineComment.Text = "Toggle Single Line Comment";
      //
      // mniSingleLineComment
      //
      this.mniSingleLineComment.Name = "mniSingleLineComment";
      this.mniSingleLineComment.Size = new System.Drawing.Size(228, 22);
      this.mniSingleLineComment.Text = "Single Line Comment";
      //
      // mniSingleLineUncomment
      //
      this.mniSingleLineUncomment.Name = "mniSingleLineUncomment";
      this.mniSingleLineUncomment.Size = new System.Drawing.Size(228, 22);
      this.mniSingleLineUncomment.Text = "Single Line Uncomment";
      //
      // mniBlockComment
      //
      this.mniBlockComment.Name = "mniBlockComment";
      this.mniBlockComment.Size = new System.Drawing.Size(228, 22);
      this.mniBlockComment.Text = "Block Comment";
      //
      // mniBlockUncomment
      //
      this.mniBlockUncomment.Name = "mniBlockUncomment";
      this.mniBlockUncomment.Size = new System.Drawing.Size(228, 22);
      this.mniBlockUncomment.Text = "Block Uncomment";
      //
      // toolStripMenuItem1
      //
      this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniUppercase,
            this.mniLowercase,
            this.toolStripSeparator7,
            this.mniDoubleToSingleQuotes,
            this.mniSingleToDoubleQuotes,
            this.toolStripSeparator8,
            this.mniMd5Encode});
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
      this.toolStripMenuItem1.Text = "Convert Characters";
      //
      // mniUppercase
      //
      this.mniUppercase.Name = "mniUppercase";
      this.mniUppercase.ShortcutKeyDisplayString = "Ctrl+Shift+U";
      this.mniUppercase.Size = new System.Drawing.Size(211, 22);
      this.mniUppercase.Text = "UPPERCASE";
      //
      // mniLowercase
      //
      this.mniLowercase.Name = "mniLowercase";
      this.mniLowercase.ShortcutKeyDisplayString = "Ctrl+U";
      this.mniLowercase.Size = new System.Drawing.Size(211, 22);
      this.mniLowercase.Text = "lowercase";
      //
      // toolStripSeparator7
      //
      this.toolStripSeparator7.Name = "toolStripSeparator7";
      this.toolStripSeparator7.Size = new System.Drawing.Size(208, 6);
      //
      // mniDoubleToSingleQuotes
      //
      this.mniDoubleToSingleQuotes.Name = "mniDoubleToSingleQuotes";
      this.mniDoubleToSingleQuotes.Size = new System.Drawing.Size(211, 22);
      this.mniDoubleToSingleQuotes.Text = "Convert \" -> \'";
      //
      // mniSingleToDoubleQuotes
      //
      this.mniSingleToDoubleQuotes.Name = "mniSingleToDoubleQuotes";
      this.mniSingleToDoubleQuotes.Size = new System.Drawing.Size(211, 22);
      this.mniSingleToDoubleQuotes.Text = "Convert \' -> \"";
      //
      // toolStripSeparator8
      //
      this.toolStripSeparator8.Name = "toolStripSeparator8";
      this.toolStripSeparator8.Size = new System.Drawing.Size(208, 6);
      //
      // mniMd5Encode
      //
      this.mniMd5Encode.Name = "mniMd5Encode";
      this.mniMd5Encode.Size = new System.Drawing.Size(211, 22);
      this.mniMd5Encode.Text = "MD5 Encode";
      //
      // lineOperationsToolStripMenuItem
      //
      this.lineOperationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniMoveUpCurrentLine,
            this.mniMoveDownCurrentLine});
      this.lineOperationsToolStripMenuItem.Name = "lineOperationsToolStripMenuItem";
      this.lineOperationsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
      this.lineOperationsToolStripMenuItem.Text = "Line Operations";
      //
      // mniMoveUpCurrentLine
      //
      this.mniMoveUpCurrentLine.Name = "mniMoveUpCurrentLine";
      this.mniMoveUpCurrentLine.ShortcutKeyDisplayString = "Ctrl+Shift+Up";
      this.mniMoveUpCurrentLine.Size = new System.Drawing.Size(303, 22);
      this.mniMoveUpCurrentLine.Text = "Move Up Current Line";
      //
      // mniMoveDownCurrentLine
      //
      this.mniMoveDownCurrentLine.Name = "mniMoveDownCurrentLine";
      this.mniMoveDownCurrentLine.ShortcutKeyDisplayString = "Ctrl+Shift+Down";
      this.mniMoveDownCurrentLine.Size = new System.Drawing.Size(303, 22);
      this.mniMoveDownCurrentLine.Text = "Move Down Current Line";
      //
      // xMLToolStripMenuItem
      //
      this.xMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniXmlToEntity,
            this.mniEntityToXml});
      this.xMLToolStripMenuItem.Name = "xMLToolStripMenuItem";
      this.xMLToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
      this.xMLToolStripMenuItem.Text = "XML";
      //
      // mniXmlToEntity
      //
      this.mniXmlToEntity.Name = "mniXmlToEntity";
      this.mniXmlToEntity.Size = new System.Drawing.Size(326, 22);
      this.mniXmlToEntity.Text = "Convert Selection XML to Text (<> => &&lt;&&gt;)";
      //
      // mniEntityToXml
      //
      this.mniEntityToXml.Name = "mniEntityToXml";
      this.mniEntityToXml.Size = new System.Drawing.Size(326, 22);
      this.mniEntityToXml.Text = "Convert Selection Text to XML (&&lt;&&gt; => <>)";
      //
      // toolStripDropDownButton4
      //
      this.toolStripDropDownButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniFind,
            this.mniFindNext,
            this.mniFindPrevious,
            this.mniReplace,
            this.mniGoTo});
      this.toolStripDropDownButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton4.Image")));
      this.toolStripDropDownButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripDropDownButton4.Name = "toolStripDropDownButton4";
      this.toolStripDropDownButton4.ShowDropDownArrow = false;
      this.toolStripDropDownButton4.Size = new System.Drawing.Size(46, 22);
      this.toolStripDropDownButton4.Text = "&Search";
      //
      // mniFind
      //
      this.mniFind.Name = "mniFind";
      this.mniFind.ShortcutKeyDisplayString = "Ctrl+F";
      this.mniFind.Size = new System.Drawing.Size(196, 22);
      this.mniFind.Text = "Find...";
      //
      // mniFindNext
      //
      this.mniFindNext.Name = "mniFindNext";
      this.mniFindNext.ShortcutKeyDisplayString = "F3";
      this.mniFindNext.Size = new System.Drawing.Size(196, 22);
      this.mniFindNext.Text = "Find Next";
      //
      // mniFindPrevious
      //
      this.mniFindPrevious.Name = "mniFindPrevious";
      this.mniFindPrevious.ShortcutKeyDisplayString = "Shift+F3";
      this.mniFindPrevious.Size = new System.Drawing.Size(196, 22);
      this.mniFindPrevious.Text = "Find Previous";
      //
      // mniReplace
      //
      this.mniReplace.Name = "mniReplace";
      this.mniReplace.ShortcutKeyDisplayString = "Ctrl+H";
      this.mniReplace.Size = new System.Drawing.Size(196, 22);
      this.mniReplace.Text = "Replace...";
      //
      // mniGoTo
      //
      this.mniGoTo.Name = "mniGoTo";
      this.mniGoTo.ShortcutKeyDisplayString = "Ctrl+G";
      this.mniGoTo.Size = new System.Drawing.Size(196, 22);
      this.mniGoTo.Text = "Go To...";
      //
      // toolStripDropDownButton3
      //
      this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem});
      this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
      this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
      this.toolStripDropDownButton3.ShowDropDownArrow = false;
      this.toolStripDropDownButton3.Size = new System.Drawing.Size(40, 22);
      this.toolStripDropDownButton3.Text = "&Tools";
      //
      // preferencesToolStripMenuItem
      //
      this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniLocale,
            this.mniTimeZone,
            this.mniTimeout});
      this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
      this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
      this.preferencesToolStripMenuItem.Text = "Preferences";
      //
      // mniLocale
      //
      this.mniLocale.Name = "mniLocale";
      this.mniLocale.ShortcutKeyDisplayString = "";
      this.mniLocale.Size = new System.Drawing.Size(140, 22);
      this.mniLocale.Text = "Locale...";
      this.mniLocale.Click += new System.EventHandler(this.mniLocale_Click);
      //
      // mniTimeZone
      //
      this.mniTimeZone.Name = "mniTimeZone";
      this.mniTimeZone.ShortcutKeyDisplayString = "";
      this.mniTimeZone.Size = new System.Drawing.Size(140, 22);
      this.mniTimeZone.Text = "Time Zone...";
      this.mniTimeZone.Click += new System.EventHandler(this.mniTimeZone_Click);
      //
      // mniTimeout
      //
      this.mniTimeout.Name = "mniTimeout";
      this.mniTimeout.Size = new System.Drawing.Size(140, 22);
      this.mniTimeout.Text = "Timeout...";
      this.mniTimeout.Click += new System.EventHandler(this.mniTimeout_Click);
      //
      // exploreButton
      //
      this.exploreButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.exploreButton.Image = ((System.Drawing.Image)(resources.GetObject("exploreButton.Image")));
      this.exploreButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.exploreButton.Name = "exploreButton";
      this.exploreButton.Size = new System.Drawing.Size(58, 22);
      this.exploreButton.Text = "E&xplore...";
      this.exploreButton.Click += new System.EventHandler(this.exploreButton_Click);
      //
      // btnSubmit
      //
      this.btnSubmit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.btnSubmit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnSubmit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniRunAll,
            this.mniRunCurrent,
            this.mniRunCurrentNewWindow});
      this.btnSubmit.Image = ((System.Drawing.Image)(resources.GetObject("btnSubmit.Image")));
      this.btnSubmit.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSubmit.Name = "btnSubmit";
      this.btnSubmit.Size = new System.Drawing.Size(57, 22);
      this.btnSubmit.Text = "► &Run";
      this.btnSubmit.ToolTipText = "Run";
      this.btnSubmit.ButtonClick += new System.EventHandler(this.btnSubmit_Click);
      //
      // mniRunAll
      //
      this.mniRunAll.Name = "mniRunAll";
      this.mniRunAll.ShortcutKeyDisplayString = "F5";
      this.mniRunAll.Size = new System.Drawing.Size(225, 22);
      this.mniRunAll.Text = "Run All";
      this.mniRunAll.Click += new System.EventHandler(this.mniRunAll_Click);
      //
      // mniRunCurrent
      //
      this.mniRunCurrent.Name = "mniRunCurrent";
      this.mniRunCurrent.ShortcutKeyDisplayString = "Ctrl+Enter";
      this.mniRunCurrent.Size = new System.Drawing.Size(225, 22);
      this.mniRunCurrent.Text = "Run Current";
      this.mniRunCurrent.Click += new System.EventHandler(this.mniRunCurrent_Click);
      //
      // mniRunCurrentNewWindow
      //
      this.mniRunCurrentNewWindow.Name = "mniRunCurrentNewWindow";
      this.mniRunCurrentNewWindow.Size = new System.Drawing.Size(225, 22);
      this.mniRunCurrentNewWindow.Text = "Run Current in New Window";
      this.mniRunCurrentNewWindow.Click += new System.EventHandler(this.mniRunCurrentNewWindow_Click);
      //
      // lblMaximize
      //
      this.lblMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblMaximize.BackColor = System.Drawing.Color.Transparent;
      this.lblMaximize.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMaximize.Location = new System.Drawing.Point(678, 0);
      this.lblMaximize.Margin = new System.Windows.Forms.Padding(0);
      this.lblMaximize.Name = "lblMaximize";
      this.lblMaximize.Size = new System.Drawing.Size(24, 32);
      this.lblMaximize.TabIndex = 13;
      this.lblMaximize.Text = "1";
      this.lblMaximize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      //
      // lblMinimize
      //
      this.lblMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblMinimize.BackColor = System.Drawing.Color.Transparent;
      this.lblMinimize.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMinimize.Location = new System.Drawing.Point(654, 0);
      this.lblMinimize.Margin = new System.Windows.Forms.Padding(0);
      this.lblMinimize.Name = "lblMinimize";
      this.lblMinimize.Size = new System.Drawing.Size(24, 32);
      this.lblMinimize.TabIndex = 12;
      this.lblMinimize.Text = "0";
      this.lblMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      //
      // picLogo
      //
      this.picLogo.Location = new System.Drawing.Point(16, 3);
      this.picLogo.Margin = new System.Windows.Forms.Padding(16, 3, 16, 3);
      this.picLogo.Name = "picLogo";
      this.tblHeader.SetRowSpan(this.picLogo, 2);
      this.picLogo.Size = new System.Drawing.Size(48, 48);
      this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.picLogo.TabIndex = 15;
      this.picLogo.TabStop = false;
      //
      // tableLayoutPanel5
      //
      this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel5.BackColor = System.Drawing.Color.Transparent;
      this.tableLayoutPanel5.ColumnCount = 3;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.tableLayoutPanel5.Controls.Add(this.lblSelection, 2, 0);
      this.tableLayoutPanel5.Controls.Add(this.lblVersion, 1, 0);
      this.tableLayoutPanel5.Controls.Add(this.lblItems, 0, 0);
      this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 469);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 1;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(540, 29);
      this.tableLayoutPanel5.TabIndex = 0;
      //
      // lblSelection
      //
      this.lblSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.lblSelection.AutoEllipsis = true;
      this.lblSelection.Location = new System.Drawing.Point(363, 7);
      this.lblSelection.Name = "lblSelection";
      this.lblSelection.Size = new System.Drawing.Size(174, 15);
      this.lblSelection.TabIndex = 4;
      //
      // lblVersion
      //
      this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.lblVersion.AutoEllipsis = true;
      this.lblVersion.Location = new System.Drawing.Point(183, 7);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(174, 15);
      this.lblVersion.TabIndex = 5;
      //
      // lblItems
      //
      this.lblItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.lblItems.AutoEllipsis = true;
      this.lblItems.Location = new System.Drawing.Point(3, 7);
      this.lblItems.Margin = new System.Windows.Forms.Padding(3);
      this.lblItems.Name = "lblItems";
      this.lblItems.Size = new System.Drawing.Size(174, 15);
      this.lblItems.TabIndex = 3;
      //
      // splitMain
      //
      this.tblMain.SetColumnSpan(this.splitMain, 3);
      this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitMain.Location = new System.Drawing.Point(6, 74);
      this.splitMain.Name = "splitMain";
      //
      // splitMain.Panel1
      //
      this.splitMain.Panel1.Controls.Add(this.tableLayoutPanel4);
      //
      // splitMain.Panel2
      //
      this.splitMain.Panel2.Controls.Add(this.splitEditors);
      this.splitMain.Size = new System.Drawing.Size(720, 387);
      this.splitMain.SplitterDistance = 220;
      this.splitMain.SplitterWidth = 5;
      this.splitMain.TabIndex = 0;
      //
      // tableLayoutPanel4
      //
      this.tableLayoutPanel4.ColumnCount = 1;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.Controls.Add(this.treeItems, 0, 1);
      this.tableLayoutPanel4.Controls.Add(this.btnPanelToggle, 0, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(220, 387);
      this.tableLayoutPanel4.TabIndex = 1;
      //
      // treeItems
      //
      this.treeItems.AllColumns.Add(this.colName);
      this.treeItems.AllColumns.Add(this.colDescription);
      this.treeItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.treeItems.CellEditUseWholeCell = false;
      this.treeItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colDescription});
      this.treeItems.Cursor = System.Windows.Forms.Cursors.Default;
      this.treeItems.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeItems.HighlightBackgroundColor = System.Drawing.Color.Empty;
      this.treeItems.HighlightForegroundColor = System.Drawing.Color.Empty;
      this.treeItems.Location = new System.Drawing.Point(0, 29);
      this.treeItems.Margin = new System.Windows.Forms.Padding(0);
      this.treeItems.Name = "treeItems";
      this.treeItems.ShowGroups = false;
      this.treeItems.Size = new System.Drawing.Size(220, 358);
      this.treeItems.TabIndex = 0;
      this.treeItems.UseCompatibleStateImageBehavior = false;
      this.treeItems.View = System.Windows.Forms.View.Details;
      this.treeItems.VirtualMode = true;
      this.treeItems.ModelDoubleClick += new System.EventHandler<InnovatorAdmin.ModelDoubleClickEventArgs>(this.treeItems_ModelDoubleClick);
      this.treeItems.Expanding += new System.EventHandler<BrightIdeasSoftware.TreeBranchExpandingEventArgs>(this.treeItems_Expanding);
      this.treeItems.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.treeItems_CellRightClick);
      this.treeItems.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.treeItems_CellToolTipShowing);
      //
      // colName
      //
      this.colName.AspectName = "Name";
      this.colName.ImageAspectName = "";
      this.colName.Text = "Name";
      this.colName.Width = 150;
      //
      // colDescription
      //
      this.colDescription.AspectName = "Description";
      this.colDescription.FillsFreeSpace = true;
      this.colDescription.Text = "Description";
      this.colDescription.Width = 200;
      //
      // btnPanelToggle
      //
      this.btnPanelToggle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPanelToggle.AutoSize = true;
      this.btnPanelToggle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnPanelToggle.FlatAppearance.BorderSize = 0;
      this.btnPanelToggle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPanelToggle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnPanelToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnPanelToggle.ForeColor = System.Drawing.Color.Black;
      this.btnPanelToggle.Location = new System.Drawing.Point(0, 0);
      this.btnPanelToggle.Margin = new System.Windows.Forms.Padding(0);
      this.btnPanelToggle.Name = "btnPanelToggle";
      this.btnPanelToggle.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnPanelToggle.Padding = new System.Windows.Forms.Padding(2);
      this.btnPanelToggle.Size = new System.Drawing.Size(220, 29);
      this.btnPanelToggle.TabIndex = 1;
      this.btnPanelToggle.Text = "Table of Contents";
      this.btnPanelToggle.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnPanelToggle.UseVisualStyleBackColor = false;
      this.btnPanelToggle.Click += new System.EventHandler(this.btnPanelToggle_Click);
      //
      // splitEditors
      //
      this.splitEditors.BackColor = System.Drawing.Color.Transparent;
      this.splitEditors.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitEditors.Location = new System.Drawing.Point(0, 0);
      this.splitEditors.Name = "splitEditors";
      this.splitEditors.Orientation = System.Windows.Forms.Orientation.Horizontal;
      //
      // splitEditors.Panel1
      //
      this.splitEditors.Panel1.Controls.Add(this.tableLayoutPanel3);
      //
      // splitEditors.Panel2
      //
      this.splitEditors.Panel2.Controls.Add(this.tableLayoutPanel2);
      this.splitEditors.Size = new System.Drawing.Size(495, 387);
      this.splitEditors.SplitterDistance = 114;
      this.splitEditors.SplitterWidth = 5;
      this.splitEditors.TabIndex = 0;
      //
      // tableLayoutPanel3
      //
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
      this.tableLayoutPanel3.Controls.Add(this.inputEditor, 0, 1);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(495, 114);
      this.tableLayoutPanel3.TabIndex = 1;
      //
      // inputEditor
      //
      this.inputEditor.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel3.SetColumnSpan(this.inputEditor, 2);
      this.inputEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.inputEditor.Location = new System.Drawing.Point(3, 3);
      this.inputEditor.Name = "inputEditor";
      this.inputEditor.ReadOnly = false;
      this.inputEditor.Size = new System.Drawing.Size(489, 108);
      this.inputEditor.TabIndex = 0;
      this.inputEditor.RunRequested += new System.EventHandler<InnovatorAdmin.Editor.RunRequestedEventArgs>(this.inputEditor_RunRequested);
      //
      // tableLayoutPanel2
      //
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.tbcOutputView, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(495, 268);
      this.tableLayoutPanel2.TabIndex = 0;
      //
      // tbcOutputView
      //
      this.tbcOutputView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel2.SetColumnSpan(this.tbcOutputView, 3);
      this.tbcOutputView.Controls.Add(this.pgTools);
      this.tbcOutputView.Controls.Add(this.pgTextOutput);
      this.tbcOutputView.Controls.Add(this.pgHtml);
      this.tbcOutputView.Controls.Add(this.pgTableOutput);
      this.tbcOutputView.DrawBorder = false;
      this.tbcOutputView.Location = new System.Drawing.Point(0, 0);
      this.tbcOutputView.Margin = new System.Windows.Forms.Padding(0);
      this.tbcOutputView.myBackColor = System.Drawing.Color.White;
      this.tbcOutputView.Name = "tbcOutputView";
      this.tbcOutputView.Padding = new System.Drawing.Point(0, 0);
      this.tbcOutputView.SelectedIndex = 0;
      this.tbcOutputView.Size = new System.Drawing.Size(495, 268);
      this.tbcOutputView.TabIndex = 5;
      this.tbcOutputView.SelectedIndexChanged += new System.EventHandler(this.tbcOutputView_SelectedIndexChanged);
      //
      // pgTools
      //
      this.pgTools.Controls.Add(this.tableLayoutPanel6);
      this.pgTools.Location = new System.Drawing.Point(4, 25);
      this.pgTools.Name = "pgTools";
      this.pgTools.Padding = new System.Windows.Forms.Padding(3);
      this.pgTools.Size = new System.Drawing.Size(487, 239);
      this.pgTools.TabIndex = 3;
      this.pgTools.Text = "Tools";
      this.pgTools.UseVisualStyleBackColor = true;
      //
      // tableLayoutPanel6
      //
      this.tableLayoutPanel6.ColumnCount = 1;
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel6.Controls.Add(this.flowLayoutPanel1, 0, 1);
      this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel6.Name = "tableLayoutPanel6";
      this.tableLayoutPanel6.RowCount = 2;
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel6.Size = new System.Drawing.Size(481, 233);
      this.tableLayoutPanel6.TabIndex = 5;
      //
      // flowLayoutPanel1
      //
      this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel1.Controls.Add(this.btnInstall);
      this.flowLayoutPanel1.Controls.Add(this.btnCreate);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(17, 17);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(17);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(447, 199);
      this.flowLayoutPanel1.TabIndex = 4;
      //
      // btnInstall
      //
      this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnInstall.AutoSize = true;
      this.btnInstall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnInstall.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnInstall.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnInstall.ForeColor = System.Drawing.Color.Black;
      this.btnInstall.Image = global::InnovatorAdmin.Properties.Resources.install32;
      this.btnInstall.Location = new System.Drawing.Point(3, 3);
      this.btnInstall.MinimumSize = new System.Drawing.Size(140, 46);
      this.btnInstall.Name = "btnInstall";
      this.btnInstall.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnInstall.Padding = new System.Windows.Forms.Padding(2);
      this.btnInstall.Size = new System.Drawing.Size(140, 70);
      this.btnInstall.TabIndex = 2;
      this.btnInstall.Text = "Install Package";
      this.btnInstall.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.btnInstall.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnInstall.UseVisualStyleBackColor = true;
      this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
      //
      // btnCreate
      //
      this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCreate.AutoSize = true;
      this.btnCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCreate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCreate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCreate.ForeColor = System.Drawing.Color.Black;
      this.btnCreate.Image = global::InnovatorAdmin.Properties.Resources.innPkg32;
      this.btnCreate.Location = new System.Drawing.Point(149, 3);
      this.btnCreate.MinimumSize = new System.Drawing.Size(140, 46);
      this.btnCreate.Name = "btnCreate";
      this.btnCreate.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnCreate.Padding = new System.Windows.Forms.Padding(2);
      this.btnCreate.Size = new System.Drawing.Size(140, 70);
      this.btnCreate.TabIndex = 3;
      this.btnCreate.Text = "Create Package";
      this.btnCreate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.btnCreate.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnCreate.UseVisualStyleBackColor = true;
      this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
      //
      // pgTextOutput
      //
      this.pgTextOutput.Controls.Add(this.outputEditor);
      this.pgTextOutput.Location = new System.Drawing.Point(4, 25);
      this.pgTextOutput.Name = "pgTextOutput";
      this.pgTextOutput.Size = new System.Drawing.Size(487, 239);
      this.pgTextOutput.TabIndex = 0;
      this.pgTextOutput.Text = "Code";
      this.pgTextOutput.UseVisualStyleBackColor = true;
      //
      // outputEditor
      //
      this.outputEditor.BackColor = System.Drawing.Color.White;
      this.outputEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.outputEditor.Location = new System.Drawing.Point(0, 0);
      this.outputEditor.Name = "outputEditor";
      this.outputEditor.ReadOnly = true;
      this.outputEditor.Size = new System.Drawing.Size(487, 239);
      this.outputEditor.TabIndex = 0;
      //
      // pgHtml
      //
      this.pgHtml.Controls.Add(this.browser);
      this.pgHtml.Location = new System.Drawing.Point(4, 25);
      this.pgHtml.Name = "pgHtml";
      this.pgHtml.Padding = new System.Windows.Forms.Padding(3);
      this.pgHtml.Size = new System.Drawing.Size(487, 239);
      this.pgHtml.TabIndex = 2;
      this.pgHtml.Text = "Report";
      this.pgHtml.UseVisualStyleBackColor = true;
      //
      // browser
      //
      this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
      this.browser.Location = new System.Drawing.Point(3, 3);
      this.browser.MinimumSize = new System.Drawing.Size(23, 23);
      this.browser.Name = "browser";
      this.browser.Size = new System.Drawing.Size(481, 233);
      this.browser.TabIndex = 0;
      //
      // pgTableOutput
      //
      this.pgTableOutput.Controls.Add(this.dgvItems);
      this.pgTableOutput.Location = new System.Drawing.Point(4, 25);
      this.pgTableOutput.Margin = new System.Windows.Forms.Padding(0);
      this.pgTableOutput.Name = "pgTableOutput";
      this.pgTableOutput.Size = new System.Drawing.Size(487, 239);
      this.pgTableOutput.TabIndex = 1;
      this.pgTableOutput.Text = "Table";
      this.pgTableOutput.UseVisualStyleBackColor = true;
      //
      // dgvItems
      //
      this.dgvItems.BackgroundColor = System.Drawing.Color.White;
      this.dgvItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvItems.ContextMenuStrip = this.conTable;
      this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvItems.Location = new System.Drawing.Point(0, 0);
      this.dgvItems.Margin = new System.Windows.Forms.Padding(0);
      this.dgvItems.Name = "dgvItems";
      this.dgvItems.Size = new System.Drawing.Size(487, 239);
      this.dgvItems.TabIndex = 0;
      //
      // conTable
      //
      this.conTable.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniColumns,
            this.toolStripSeparator9,
            this.scriptEditsToToolStripMenuItem,
            this.toolStripSeparator1,
            this.mniAcceptChanges,
            this.mniResetChanges});
      this.conTable.Name = "conTable";
      this.conTable.Size = new System.Drawing.Size(161, 104);
      //
      // mniColumns
      //
      this.mniColumns.Name = "mniColumns";
      this.mniColumns.Size = new System.Drawing.Size(160, 22);
      this.mniColumns.Text = "Columns...";
      this.mniColumns.Click += new System.EventHandler(this.mniColumns_Click);
      //
      // toolStripSeparator9
      //
      this.toolStripSeparator9.Name = "toolStripSeparator9";
      this.toolStripSeparator9.Size = new System.Drawing.Size(157, 6);
      //
      // scriptEditsToToolStripMenuItem
      //
      this.scriptEditsToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniTableEditsToClipboard,
            this.mniTableEditsToFile,
            this.mniTableEditsToQueryEditor});
      this.scriptEditsToToolStripMenuItem.Name = "scriptEditsToToolStripMenuItem";
      this.scriptEditsToToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
      this.scriptEditsToToolStripMenuItem.Text = "Script Edits To";
      //
      // mniTableEditsToClipboard
      //
      this.mniTableEditsToClipboard.Name = "mniTableEditsToClipboard";
      this.mniTableEditsToClipboard.Size = new System.Drawing.Size(140, 22);
      this.mniTableEditsToClipboard.Text = "Clipboard";
      this.mniTableEditsToClipboard.Click += new System.EventHandler(this.mniTableEditsToClipboard_Click);
      //
      // mniTableEditsToFile
      //
      this.mniTableEditsToFile.Name = "mniTableEditsToFile";
      this.mniTableEditsToFile.Size = new System.Drawing.Size(140, 22);
      this.mniTableEditsToFile.Text = "File...";
      this.mniTableEditsToFile.Click += new System.EventHandler(this.mniTableEditsToFile_Click);
      //
      // mniTableEditsToQueryEditor
      //
      this.mniTableEditsToQueryEditor.Name = "mniTableEditsToQueryEditor";
      this.mniTableEditsToQueryEditor.Size = new System.Drawing.Size(140, 22);
      this.mniTableEditsToQueryEditor.Text = "Query Editor";
      this.mniTableEditsToQueryEditor.Click += new System.EventHandler(this.mniTableEditsToQueryEditor_Click);
      //
      // toolStripSeparator1
      //
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
      //
      // mniAcceptChanges
      //
      this.mniAcceptChanges.Name = "mniAcceptChanges";
      this.mniAcceptChanges.Size = new System.Drawing.Size(160, 22);
      this.mniAcceptChanges.Text = "Accept Changes";
      this.mniAcceptChanges.Click += new System.EventHandler(this.mniAcceptChanges_Click);
      //
      // mniResetChanges
      //
      this.mniResetChanges.Name = "mniResetChanges";
      this.mniResetChanges.Size = new System.Drawing.Size(160, 22);
      this.mniResetChanges.Text = "Reset Changes";
      this.mniResetChanges.Click += new System.EventHandler(this.mniResetChanges_Click);
      //
      // btnOk
      //
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.AutoSize = true;
      this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnOk.ForeColor = System.Drawing.Color.Black;
      this.btnOk.Location = new System.Drawing.Point(546, 467);
      this.btnOk.Name = "btnOk";
      this.btnOk.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnOk.Padding = new System.Windows.Forms.Padding(2);
      this.btnOk.Size = new System.Drawing.Size(87, 33);
      this.btnOk.TabIndex = 1;
      this.btnOk.Text = "&OK";
      this.btnOk.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnOk.UseVisualStyleBackColor = true;
      //
      // btnCancel
      //
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.AutoSize = true;
      this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
      this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnCancel.ForeColor = System.Drawing.Color.Black;
      this.btnCancel.Location = new System.Drawing.Point(639, 467);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      this.btnCancel.Padding = new System.Windows.Forms.Padding(2);
      this.btnCancel.Size = new System.Drawing.Size(87, 33);
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.Theme = InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnCancel.UseVisualStyleBackColor = true;
      //
      // pnlTopLeft
      //
      this.pnlTopLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTopLeft.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlTopLeft.Location = new System.Drawing.Point(0, 0);
      this.pnlTopLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTopLeft.Name = "pnlTopLeft";
      this.pnlTopLeft.Size = new System.Drawing.Size(3, 3);
      this.pnlTopLeft.TabIndex = 6;
      //
      // pnlTop
      //
      this.pnlTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTop.BackColor = System.Drawing.Color.WhiteSmoke;
      this.tblMain.SetColumnSpan(this.pnlTop, 3);
      this.pnlTop.Location = new System.Drawing.Point(3, 0);
      this.pnlTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new System.Drawing.Size(726, 3);
      this.pnlTop.TabIndex = 7;
      //
      // pnlTopRight
      //
      this.pnlTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlTopRight.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlTopRight.Location = new System.Drawing.Point(729, 0);
      this.pnlTopRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlTopRight.Name = "pnlTopRight";
      this.pnlTopRight.Size = new System.Drawing.Size(3, 3);
      this.pnlTopRight.TabIndex = 8;
      //
      // pnlBottomLeft
      //
      this.pnlBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomLeft.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlBottomLeft.Location = new System.Drawing.Point(0, 503);
      this.pnlBottomLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomLeft.Name = "pnlBottomLeft";
      this.pnlBottomLeft.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomLeft.TabIndex = 10;
      //
      // pnlBottom
      //
      this.pnlBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottom.BackColor = System.Drawing.Color.WhiteSmoke;
      this.tblMain.SetColumnSpan(this.pnlBottom, 3);
      this.pnlBottom.Location = new System.Drawing.Point(3, 503);
      this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new System.Drawing.Size(726, 3);
      this.pnlBottom.TabIndex = 11;
      //
      // pnlBottomRight
      //
      this.pnlBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlBottomRight.BackColor = System.Drawing.Color.WhiteSmoke;
      this.pnlBottomRight.Location = new System.Drawing.Point(729, 503);
      this.pnlBottomRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlBottomRight.Name = "pnlBottomRight";
      this.pnlBottomRight.Size = new System.Drawing.Size(3, 3);
      this.pnlBottomRight.TabIndex = 12;
      //
      // pnlRight
      //
      this.pnlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlRight.Location = new System.Drawing.Point(729, 66);
      this.pnlRight.Margin = new System.Windows.Forms.Padding(0);
      this.pnlRight.Name = "pnlRight";
      this.tblMain.SetRowSpan(this.pnlRight, 3);
      this.pnlRight.ShadowExtent = 5;
      this.pnlRight.Size = new System.Drawing.Size(3, 437);
      this.pnlRight.TabIndex = 14;
      //
      // pnlLeft
      //
      this.pnlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlLeft.Location = new System.Drawing.Point(0, 66);
      this.pnlLeft.Margin = new System.Windows.Forms.Padding(0);
      this.pnlLeft.Name = "pnlLeft";
      this.tblMain.SetRowSpan(this.pnlLeft, 3);
      this.pnlLeft.ShadowExtent = 5;
      this.pnlLeft.Size = new System.Drawing.Size(3, 437);
      this.pnlLeft.TabIndex = 13;
      //
      // pnlLeftTop
      //
      this.pnlLeftTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlLeftTop.Location = new System.Drawing.Point(0, 3);
      this.pnlLeftTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlLeftTop.Name = "pnlLeftTop";
      this.pnlLeftTop.Size = new System.Drawing.Size(3, 63);
      this.pnlLeftTop.TabIndex = 15;
      //
      // pnlRightTop
      //
      this.pnlRightTop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlRightTop.Location = new System.Drawing.Point(729, 3);
      this.pnlRightTop.Margin = new System.Windows.Forms.Padding(0);
      this.pnlRightTop.Name = "pnlRightTop";
      this.pnlRightTop.Size = new System.Drawing.Size(3, 63);
      this.pnlRightTop.TabIndex = 16;
      //
      // dropShadow1
      //
      this.dropShadow1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tblMain.SetColumnSpan(this.dropShadow1, 3);
      this.dropShadow1.Location = new System.Drawing.Point(3, 66);
      this.dropShadow1.Margin = new System.Windows.Forms.Padding(0);
      this.dropShadow1.Name = "dropShadow1";
      this.dropShadow1.ShadowExtent = 0;
      this.dropShadow1.Size = new System.Drawing.Size(726, 5);
      this.dropShadow1.TabIndex = 17;
      //
      // EditorWindow
      //
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.DarkGray;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(734, 508);
      this.Controls.Add(this.tblMain);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Location = new System.Drawing.Point(0, 0);
      this.Name = "EditorWindow";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "Innovator Admin";
      this.tblMain.ResumeLayout(false);
      this.tblMain.PerformLayout();
      this.tblHeader.ResumeLayout(false);
      this.tblHeader.PerformLayout();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.splitMain.Panel1.ResumeLayout(false);
      this.splitMain.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
      this.splitMain.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.treeItems)).EndInit();
      this.splitEditors.Panel1.ResumeLayout(false);
      this.splitEditors.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitEditors)).EndInit();
      this.splitEditors.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tbcOutputView.ResumeLayout(false);
      this.pgTools.ResumeLayout(false);
      this.tableLayoutPanel6.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.pgTextOutput.ResumeLayout(false);
      this.pgHtml.ResumeLayout(false);
      this.pgTableOutput.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
      this.conTable.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tblMain;
    private Controls.SplitContainerTheme splitMain;
    private Controls.FlatButton btnOk;
    private Controls.FlatButton btnCancel;
    private System.Windows.Forms.Label lblItems;
    private System.Windows.Forms.ContextMenuStrip conTable;
    private System.Windows.Forms.ToolStripMenuItem scriptEditsToToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mniTableEditsToQueryEditor;
    private System.Windows.Forms.ToolStripMenuItem mniTableEditsToClipboard;
    private System.Windows.Forms.ToolStripMenuItem mniTableEditsToFile;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem mniAcceptChanges;
    private System.Windows.Forms.ToolStripMenuItem mniResetChanges;
    private System.Windows.Forms.ToolStrip menuStrip;
    private System.Windows.Forms.ToolStripLabel lblConnection;
    private System.Windows.Forms.ToolStripButton btnEditConnections;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
    private System.Windows.Forms.ToolStripMenuItem mniTidy;
    private System.Windows.Forms.ToolStripLabel lblSoapAction;
    private System.Windows.Forms.ToolStripButton btnSoapAction;
    private System.Windows.Forms.ToolStripSplitButton btnSubmit;
    private System.Windows.Forms.ToolStripMenuItem mniRunAll;
    private System.Windows.Forms.ToolStripMenuItem mniRunCurrent;
    private System.Windows.Forms.ToolStripMenuItem mniRunCurrentNewWindow;
    private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
    private System.Windows.Forms.ToolStripMenuItem mniNewWindow;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripMenuItem mniClose;
    private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mniLocale;
    private System.Windows.Forms.ToolStripMenuItem mniTimeZone;
    private System.Windows.Forms.ToolStripMenuItem mniTimeout;
    private System.Windows.Forms.ToolStripButton exploreButton;
    private Controls.SplitContainerTheme splitEditors;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private Editor.FullEditor inputEditor;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private Controls.FlatTabControl tbcOutputView;
    private System.Windows.Forms.TabPage pgTextOutput;
    private Editor.FullEditor outputEditor;
    private System.Windows.Forms.TabPage pgTableOutput;
    private Controls.DataGrid dgvItems;
    private TreeListView treeItems;
    private BrightIdeasSoftware.OLVColumn colName;
    private BrightIdeasSoftware.OLVColumn colDescription;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem mniUppercase;
    private System.Windows.Forms.ToolStripMenuItem mniLowercase;
    private System.Windows.Forms.ToolStripMenuItem lineOperationsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mniMoveUpCurrentLine;
    private System.Windows.Forms.ToolStripMenuItem mniMoveDownCurrentLine;
    private System.Windows.Forms.ToolStripMenuItem xMLToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mniXmlToEntity;
    private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mniInsertNewGuid;
    private System.Windows.Forms.ToolStripMenuItem mniEntityToXml;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private Controls.FlatButton btnPanelToggle;
    private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
    private System.Windows.Forms.ToolStripMenuItem mniUndo;
    private System.Windows.Forms.ToolStripMenuItem mniRedo;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripMenuItem mniCut;
    private System.Windows.Forms.ToolStripMenuItem mniCopy;
    private System.Windows.Forms.ToolStripMenuItem mniPaste;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
    private System.Windows.Forms.ToolStripMenuItem mniDoubleToSingleQuotes;
    private System.Windows.Forms.ToolStripMenuItem mniSingleToDoubleQuotes;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
    private System.Windows.Forms.ToolStripMenuItem mniMd5Encode;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Label lblSelection;
    private System.Windows.Forms.TabPage pgHtml;
    private System.Windows.Forms.TabPage pgTools;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private Controls.FlatButton btnInstall;
    private Controls.FlatButton btnCreate;
    private System.Windows.Forms.ToolStripMenuItem mniColumns;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
    private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton4;
    private System.Windows.Forms.ToolStripMenuItem mniFind;
    private System.Windows.Forms.ToolStripMenuItem mniFindNext;
    private System.Windows.Forms.ToolStripMenuItem mniFindPrevious;
    private System.Windows.Forms.ToolStripMenuItem mniReplace;
    private System.Windows.Forms.ToolStripMenuItem mniGoTo;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.WebBrowser browser;
    private System.Windows.Forms.ToolStripMenuItem codeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mniMinify;
    private System.Windows.Forms.ToolStripMenuItem commentUncommentToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mniToggleSingleLineComment;
    private System.Windows.Forms.ToolStripMenuItem mniSingleLineComment;
    private System.Windows.Forms.ToolStripMenuItem mniSingleLineUncomment;
    private System.Windows.Forms.ToolStripMenuItem mniBlockComment;
    private System.Windows.Forms.ToolStripMenuItem mniBlockUncomment;
    private System.Windows.Forms.Panel pnlTopLeft;
    private System.Windows.Forms.Panel pnlTop;
    private System.Windows.Forms.Panel pnlTopRight;
    private System.Windows.Forms.Panel pnlBottomLeft;
    private System.Windows.Forms.Panel pnlBottom;
    private System.Windows.Forms.Panel pnlBottomRight;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblClose;
    private System.Windows.Forms.Label lblMaximize;
    private System.Windows.Forms.Label lblMinimize;
    private System.Windows.Forms.TableLayoutPanel tblHeader;
    private DropShadow pnlLeft;
    private DropShadow pnlRight;
    private System.Windows.Forms.PictureBox picLogo;
    private System.Windows.Forms.Panel pnlLeftTop;
    private System.Windows.Forms.Panel pnlRightTop;
    private DropShadow dropShadow1;
  }
}
