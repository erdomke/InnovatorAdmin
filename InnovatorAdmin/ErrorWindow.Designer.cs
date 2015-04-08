namespace Aras.Tools.InnovatorAdmin
{
  partial class ErrorWindow
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorWindow));
      this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
      this.btnShowDetails = new System.Windows.Forms.Button();
      this.btnRetry = new System.Windows.Forms.Button();
      this.btnIgnore = new System.Windows.Forms.Button();
      this.btnAbort = new System.Windows.Forms.Button();
      this.txtMessage = new System.Windows.Forms.TextBox();
      this.tbcMain = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.txtErrorDetails = new EditorControl();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.txtQuery = new EditorControl();
      this.tableLayout.SuspendLayout();
      this.tbcMain.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayout
      // 
      this.tableLayout.BackColor = System.Drawing.Color.White;
      this.tableLayout.ColumnCount = 4;
      this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
      this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayout.Controls.Add(this.btnShowDetails, 1, 1);
      this.tableLayout.Controls.Add(this.btnRetry, 1, 3);
      this.tableLayout.Controls.Add(this.btnIgnore, 2, 3);
      this.tableLayout.Controls.Add(this.btnAbort, 3, 3);
      this.tableLayout.Controls.Add(this.txtMessage, 1, 0);
      this.tableLayout.Controls.Add(this.tbcMain, 0, 2);
      this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayout.Location = new System.Drawing.Point(0, 0);
      this.tableLayout.Name = "tableLayout";
      this.tableLayout.RowCount = 4;
      this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayout.Size = new System.Drawing.Size(414, 117);
      this.tableLayout.TabIndex = 0;
      // 
      // btnShowDetails
      // 
      this.btnShowDetails.AutoSize = true;
      this.tableLayout.SetColumnSpan(this.btnShowDetails, 3);
      this.btnShowDetails.Location = new System.Drawing.Point(51, 62);
      this.btnShowDetails.Name = "btnShowDetails";
      this.btnShowDetails.Size = new System.Drawing.Size(85, 23);
      this.btnShowDetails.TabIndex = 1;
      this.btnShowDetails.Text = "Toggle Details";
      this.btnShowDetails.UseVisualStyleBackColor = true;
      this.btnShowDetails.Click += new System.EventHandler(this.btnShowDetails_Click);
      // 
      // btnRetry
      // 
      this.btnRetry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRetry.AutoSize = true;
      this.btnRetry.DialogResult = System.Windows.Forms.DialogResult.Retry;
      this.btnRetry.Location = new System.Drawing.Point(232, 91);
      this.btnRetry.Name = "btnRetry";
      this.btnRetry.Size = new System.Drawing.Size(61, 23);
      this.btnRetry.TabIndex = 3;
      this.btnRetry.Text = "&Retry";
      this.btnRetry.UseVisualStyleBackColor = true;
      // 
      // btnIgnore
      // 
      this.btnIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnIgnore.AutoSize = true;
      this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Ignore;
      this.btnIgnore.Location = new System.Drawing.Point(299, 91);
      this.btnIgnore.Name = "btnIgnore";
      this.btnIgnore.Size = new System.Drawing.Size(53, 23);
      this.btnIgnore.TabIndex = 4;
      this.btnIgnore.Text = "&Ignore";
      this.btnIgnore.UseVisualStyleBackColor = true;
      // 
      // btnAbort
      // 
      this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAbort.AutoSize = true;
      this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
      this.btnAbort.Location = new System.Drawing.Point(358, 91);
      this.btnAbort.Name = "btnAbort";
      this.btnAbort.Size = new System.Drawing.Size(53, 23);
      this.btnAbort.TabIndex = 5;
      this.btnAbort.Text = "&Abort";
      this.btnAbort.UseVisualStyleBackColor = true;
      // 
      // txtMessage
      // 
      this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMessage.BackColor = System.Drawing.Color.White;
      this.tableLayout.SetColumnSpan(this.txtMessage, 3);
      this.txtMessage.Location = new System.Drawing.Point(51, 3);
      this.txtMessage.Multiline = true;
      this.txtMessage.Name = "txtMessage";
      this.txtMessage.ReadOnly = true;
      this.txtMessage.Size = new System.Drawing.Size(360, 53);
      this.txtMessage.TabIndex = 0;
      // 
      // tbcMain
      // 
      this.tbcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayout.SetColumnSpan(this.tbcMain, 4);
      this.tbcMain.Controls.Add(this.tabPage1);
      this.tbcMain.Controls.Add(this.tabPage2);
      this.tbcMain.Location = new System.Drawing.Point(3, 91);
      this.tbcMain.Name = "tbcMain";
      this.tbcMain.SelectedIndex = 0;
      this.tbcMain.Size = new System.Drawing.Size(408, 1);
      this.tbcMain.TabIndex = 2;
      this.tbcMain.Visible = false;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.txtErrorDetails);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(400, 0);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Error Details";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // txtErrorDetails
      // 
      this.txtErrorDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtErrorDetails.Location = new System.Drawing.Point(3, 3);
      this.txtErrorDetails.Name = "txtErrorDetails";
      this.txtErrorDetails.Size = new System.Drawing.Size(394, 0);
      this.txtErrorDetails.TabIndex = 0;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.tableLayoutPanel2);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(400, 0);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Query";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.txtQuery, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(394, 0);
      this.tableLayoutPanel2.TabIndex = 2;
      // 
      // txtQuery
      // 
      this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtQuery.Location = new System.Drawing.Point(0, 0);
      this.txtQuery.Margin = new System.Windows.Forms.Padding(0);
      this.txtQuery.Name = "txtQuery";
      this.txtQuery.Size = new System.Drawing.Size(394, 1);
      this.txtQuery.TabIndex = 3;
      // 
      // ErrorWindow
      // 
      this.AcceptButton = this.btnRetry;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnAbort;
      this.ClientSize = new System.Drawing.Size(414, 117);
      this.Controls.Add(this.tableLayout);
      this.Name = "ErrorWindow";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Error";
      this.tableLayout.ResumeLayout(false);
      this.tableLayout.PerformLayout();
      this.tbcMain.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayout;
    private System.Windows.Forms.TextBox txtMessage;
    private System.Windows.Forms.Button btnShowDetails;
    private System.Windows.Forms.TabControl tbcMain;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.Button btnRetry;
    private System.Windows.Forms.Button btnIgnore;
    private System.Windows.Forms.Button btnAbort;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private EditorControl txtErrorDetails;
    private EditorControl txtQuery;
  }
}