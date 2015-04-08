namespace Aras.Tools.InnovatorAdmin
{
  partial class Editor
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.splitMain = new System.Windows.Forms.SplitContainer();
      this.lstItems = new System.Windows.Forms.ListBox();
      this.splitEditors = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.inputEditor = new Aras.Tools.InnovatorAdmin.EditorControl();
      this.lblConnection = new System.Windows.Forms.Label();
      this.btnEditConnections = new System.Windows.Forms.Button();
      this.lblConnectionName = new System.Windows.Forms.Label();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSubmit = new System.Windows.Forms.Button();
      this.cmbSoapAction = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.tbcOutputView = new System.Windows.Forms.TabControl();
      this.pgAmlOutput = new System.Windows.Forms.TabPage();
      this.outputEditor = new Aras.Tools.InnovatorAdmin.EditorControl();
      this.pgTableOutput = new System.Windows.Forms.TabPage();
      this.dgvItems = new System.Windows.Forms.DataGridView();
      this.btnOk = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.lblItems = new System.Windows.Forms.Label();
      this.callAction = new System.ComponentModel.BackgroundWorker();
      this.tableLayoutPanel1.SuspendLayout();
      this.splitMain.Panel1.SuspendLayout();
      this.splitMain.Panel2.SuspendLayout();
      this.splitMain.SuspendLayout();
      this.splitEditors.Panel1.SuspendLayout();
      this.splitEditors.Panel2.SuspendLayout();
      this.splitEditors.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tbcOutputView.SuspendLayout();
      this.pgAmlOutput.SuspendLayout();
      this.pgTableOutput.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.splitMain, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnOk, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.lblItems, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(727, 439);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // splitMain
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.splitMain, 3);
      this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitMain.Location = new System.Drawing.Point(3, 3);
      this.splitMain.Name = "splitMain";
      // 
      // splitMain.Panel1
      // 
      this.splitMain.Panel1.Controls.Add(this.lstItems);
      this.splitMain.Panel1Collapsed = true;
      // 
      // splitMain.Panel2
      // 
      this.splitMain.Panel2.Controls.Add(this.splitEditors);
      this.splitMain.Size = new System.Drawing.Size(721, 404);
      this.splitMain.SplitterDistance = 164;
      this.splitMain.TabIndex = 0;
      // 
      // lstItems
      // 
      this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lstItems.FormattingEnabled = true;
      this.lstItems.Location = new System.Drawing.Point(0, 0);
      this.lstItems.Name = "lstItems";
      this.lstItems.Size = new System.Drawing.Size(164, 100);
      this.lstItems.TabIndex = 0;
      this.lstItems.SelectedIndexChanged += new System.EventHandler(this.lstItems_SelectedIndexChanged);
      // 
      // splitEditors
      // 
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
      this.splitEditors.Size = new System.Drawing.Size(721, 404);
      this.splitEditors.SplitterDistance = 256;
      this.splitEditors.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 3;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.Controls.Add(this.inputEditor, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.lblConnection, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnEditConnections, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.lblConnectionName, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(721, 256);
      this.tableLayoutPanel3.TabIndex = 1;
      // 
      // inputEditor
      // 
      this.tableLayoutPanel3.SetColumnSpan(this.inputEditor, 3);
      this.inputEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.inputEditor.Location = new System.Drawing.Point(3, 32);
      this.inputEditor.Name = "inputEditor";
      this.inputEditor.Size = new System.Drawing.Size(715, 221);
      this.inputEditor.SoapAction = "ApplyAML";
      this.inputEditor.TabIndex = 0;
      this.inputEditor.RunRequested += new System.EventHandler<Aras.Tools.InnovatorAdmin.RunRequestedEventArgs>(this.inputEditor_RunRequested);
      // 
      // lblConnection
      // 
      this.lblConnection.AutoSize = true;
      this.lblConnection.Location = new System.Drawing.Point(3, 3);
      this.lblConnection.Margin = new System.Windows.Forms.Padding(3);
      this.lblConnection.Name = "lblConnection";
      this.lblConnection.Size = new System.Drawing.Size(61, 13);
      this.lblConnection.TabIndex = 1;
      this.lblConnection.Text = "Connection";
      // 
      // btnEditConnections
      // 
      this.btnEditConnections.Location = new System.Drawing.Point(76, 3);
      this.btnEditConnections.Name = "btnEditConnections";
      this.btnEditConnections.Size = new System.Drawing.Size(75, 23);
      this.btnEditConnections.TabIndex = 3;
      this.btnEditConnections.Text = "Change";
      this.btnEditConnections.UseVisualStyleBackColor = true;
      this.btnEditConnections.Click += new System.EventHandler(this.btnEditConnections_Click);
      // 
      // lblConnectionName
      // 
      this.lblConnectionName.AutoSize = true;
      this.lblConnectionName.Location = new System.Drawing.Point(70, 3);
      this.lblConnectionName.Margin = new System.Windows.Forms.Padding(3);
      this.lblConnectionName.Name = "lblConnectionName";
      this.lblConnectionName.Size = new System.Drawing.Size(0, 13);
      this.lblConnectionName.TabIndex = 4;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.btnSubmit, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.cmbSoapAction, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.tbcOutputView, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(721, 144);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // btnSubmit
      // 
      this.btnSubmit.Location = new System.Drawing.Point(201, 3);
      this.btnSubmit.Name = "btnSubmit";
      this.btnSubmit.Size = new System.Drawing.Size(75, 23);
      this.btnSubmit.TabIndex = 4;
      this.btnSubmit.Text = "Submit";
      this.btnSubmit.UseVisualStyleBackColor = true;
      this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
      // 
      // cmbSoapAction
      // 
      this.cmbSoapAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbSoapAction.FormattingEnabled = true;
      this.cmbSoapAction.Items.AddRange(new object[] {
            "ActivateActivity",
            "AddItem",
            "ApplyAML",
            "ApplyItem",
            "ApplyMethod",
            "ApplySQL",
            "ApplyUpdate",
            "BuildProcessReport",
            "CacheDiag",
            "CancelWorkflow",
            "ChangeUserPassword",
            "CheckImportedItemType",
            "ClearCache",
            "ClearHistory",
            "CloneForm",
            "CloseWorkflow",
            "CompileMethod",
            "CopyItem",
            "CopyItem2",
            "CreateItem",
            "DeleteItem",
            "DeleteUsers",
            "DeleteVersionFile",
            "EditItem",
            "EvaluateActivity",
            "ExecuteEscalations",
            "ExecuteReminders",
            "ExportItemType",
            "GenerateNewGUID",
            "GenerateNewGUIDEx",
            "GenerateParametersGrid",
            "GenerateRelationshipsTabbar",
            "GenerateRelationshipsTable",
            "GetAffectedItems",
            "GetAssignedActivities",
            "GetAssignedTasks",
            "GetConfigurableGridMetadata",
            "GetCurrentUserID",
            "GetFormForDisplay",
            "GetHistoryItems",
            "GetIdentityList",
            "GetItem",
            "GetItemAllVersions",
            "GetItemLastVersion",
            "GetItemNextStates",
            "GetItemRelationships",
            "GetItemTypeByFormID",
            "GetItemTypeForClient",
            "GetItemWhereUsed",
            "GetMainTreeItems",
            "GetNextSequence",
            "GetPermissionsForClient",
            "GetUsersList",
            "GetUserWorkingDirectory",
            "InstantiateWorkflow",
            "LoadCache",
            "LoadProcessInstance",
            "LoadVersionFile",
            "LockItem",
            "LogMessage",
            "LogOff",
            "MergeItem",
            "NewItem",
            "NewRelationship",
            "PopulateRelationshipsGrid",
            "PopulateRelationshipsTables",
            "ProcessReplicationQueue",
            "PromoteItem",
            "PurgeItem",
            "ReassignActivity",
            "RebuildKeyedName",
            "RebuildView",
            "ReplicationExecutionResult",
            "ResetAllItemsAccess",
            "ResetItemAccess",
            "ResetLifeCycle",
            "ResetServerCache",
            "SaveCache",
            "ServerErrorTest",
            "SetDefaultLifeCycle",
            "SetNullBooleanTo0",
            "SetUserWorkingDirectory",
            "SkipItem",
            "StartDefaultWorkflow",
            "StartNamedWorkflow",
            "StartWorkflow",
            "StoreVersionFile",
            "TransformVaultServerURL",
            "UnlockAll",
            "UnlockItem",
            "UpdateItem",
            "ValidateUser",
            "ValidateVote",
            "ValidateWorkflowMap"});
      this.cmbSoapAction.Location = new System.Drawing.Point(74, 3);
      this.cmbSoapAction.Name = "cmbSoapAction";
      this.cmbSoapAction.Size = new System.Drawing.Size(121, 21);
      this.cmbSoapAction.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 3);
      this.label2.Margin = new System.Windows.Forms.Padding(3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(65, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Soap Action";
      // 
      // tbcOutputView
      // 
      this.tbcOutputView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel2.SetColumnSpan(this.tbcOutputView, 3);
      this.tbcOutputView.Controls.Add(this.pgAmlOutput);
      this.tbcOutputView.Controls.Add(this.pgTableOutput);
      this.tbcOutputView.Location = new System.Drawing.Point(0, 29);
      this.tbcOutputView.Margin = new System.Windows.Forms.Padding(0);
      this.tbcOutputView.Name = "tbcOutputView";
      this.tbcOutputView.Padding = new System.Drawing.Point(0, 0);
      this.tbcOutputView.SelectedIndex = 0;
      this.tbcOutputView.Size = new System.Drawing.Size(721, 115);
      this.tbcOutputView.TabIndex = 5;
      this.tbcOutputView.SelectedIndexChanged += new System.EventHandler(this.tbcOutputView_SelectedIndexChanged);
      // 
      // pgAmlOutput
      // 
      this.pgAmlOutput.Controls.Add(this.outputEditor);
      this.pgAmlOutput.Location = new System.Drawing.Point(4, 22);
      this.pgAmlOutput.Name = "pgAmlOutput";
      this.pgAmlOutput.Size = new System.Drawing.Size(713, 89);
      this.pgAmlOutput.TabIndex = 0;
      this.pgAmlOutput.Text = "AML";
      this.pgAmlOutput.UseVisualStyleBackColor = true;
      // 
      // outputEditor
      // 
      this.outputEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.outputEditor.Location = new System.Drawing.Point(0, 0);
      this.outputEditor.Name = "outputEditor";
      this.outputEditor.Size = new System.Drawing.Size(713, 89);
      this.outputEditor.SoapAction = "ApplyAML";
      this.outputEditor.TabIndex = 0;
      // 
      // pgTableOutput
      // 
      this.pgTableOutput.Controls.Add(this.dgvItems);
      this.pgTableOutput.Location = new System.Drawing.Point(4, 22);
      this.pgTableOutput.Margin = new System.Windows.Forms.Padding(0);
      this.pgTableOutput.Name = "pgTableOutput";
      this.pgTableOutput.Size = new System.Drawing.Size(713, 89);
      this.pgTableOutput.TabIndex = 1;
      this.pgTableOutput.Text = "Table";
      this.pgTableOutput.UseVisualStyleBackColor = true;
      // 
      // dgvItems
      // 
      this.dgvItems.AllowUserToAddRows = false;
      this.dgvItems.AllowUserToDeleteRows = false;
      this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvItems.Location = new System.Drawing.Point(0, 0);
      this.dgvItems.Margin = new System.Windows.Forms.Padding(0);
      this.dgvItems.Name = "dgvItems";
      this.dgvItems.ReadOnly = true;
      this.dgvItems.Size = new System.Drawing.Size(713, 89);
      this.dgvItems.TabIndex = 0;
      // 
      // btnOk
      // 
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.AutoSize = true;
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Location = new System.Drawing.Point(568, 413);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 23);
      this.btnOk.TabIndex = 1;
      this.btnOk.Text = "&OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Visible = false;
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.AutoSize = true;
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(649, 413);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Visible = false;
      // 
      // lblItems
      // 
      this.lblItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblItems.AutoSize = true;
      this.lblItems.Location = new System.Drawing.Point(3, 417);
      this.lblItems.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
      this.lblItems.Name = "lblItems";
      this.lblItems.Size = new System.Drawing.Size(0, 13);
      this.lblItems.TabIndex = 3;
      // 
      // callAction
      // 
      this.callAction.DoWork += new System.ComponentModel.DoWorkEventHandler(this.callAction_DoWork);
      this.callAction.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.callAction_RunWorkerCompleted);
      // 
      // Editor
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(727, 439);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "Editor";
      this.Text = "AmlStudio";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.splitMain.Panel1.ResumeLayout(false);
      this.splitMain.Panel2.ResumeLayout(false);
      this.splitMain.ResumeLayout(false);
      this.splitEditors.Panel1.ResumeLayout(false);
      this.splitEditors.Panel2.ResumeLayout(false);
      this.splitEditors.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.tbcOutputView.ResumeLayout(false);
      this.pgAmlOutput.ResumeLayout(false);
      this.pgTableOutput.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.SplitContainer splitMain;
    private System.Windows.Forms.ListBox lstItems;
    private System.Windows.Forms.SplitContainer splitEditors;
    private EditorControl inputEditor;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private EditorControl outputEditor;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label lblConnection;
    private System.Windows.Forms.Button btnEditConnections;
    private System.Windows.Forms.Button btnSubmit;
    private System.Windows.Forms.ComboBox cmbSoapAction;
    private System.Windows.Forms.Label label2;
    private System.ComponentModel.BackgroundWorker callAction;
    private System.Windows.Forms.TabControl tbcOutputView;
    private System.Windows.Forms.TabPage pgAmlOutput;
    private System.Windows.Forms.TabPage pgTableOutput;
    private System.Windows.Forms.DataGridView dgvItems;
    private System.Windows.Forms.Label lblConnectionName;
    private System.Windows.Forms.Label lblItems;
  }
}