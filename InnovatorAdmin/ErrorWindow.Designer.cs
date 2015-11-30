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
      this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
      this.btnShowDetails = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnRetry = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnIgnore = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.btnAbort = new Aras.Tools.InnovatorAdmin.Controls.FlatButton();
      this.txtMessage = new System.Windows.Forms.TextBox();
      this.tbcMain = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.txtErrorDetails = new Aras.Tools.InnovatorAdmin.Editor.EditorControl();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.txtQuery = new Aras.Tools.InnovatorAdmin.Editor.EditorControl();
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
      this.btnShowDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.tableLayout.SetColumnSpan(this.btnShowDetails, 3);
      this.btnShowDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnShowDetails.ForeColor = System.Drawing.Color.Black;
      this.btnShowDetails.Location = new System.Drawing.Point(51, 62);
      this.btnShowDetails.Name = "btnShowDetails";
      this.btnShowDetails.Padding = new System.Windows.Forms.Padding(2);
      this.btnShowDetails.Size = new System.Drawing.Size(91, 29);
      this.btnShowDetails.TabIndex = 1;
      this.btnShowDetails.Text = "Toggle Details";
      this.btnShowDetails.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnShowDetails.UseVisualStyleBackColor = true;
      this.btnShowDetails.Click += new System.EventHandler(this.btnShowDetails_Click);
      //
      // btnRetry
      //
      this.btnRetry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRetry.AutoSize = true;
      this.btnRetry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnRetry.DialogResult = System.Windows.Forms.DialogResult.Retry;
      this.btnRetry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnRetry.ForeColor = System.Drawing.Color.Black;
      this.btnRetry.Location = new System.Drawing.Point(232, 85);
      this.btnRetry.Name = "btnRetry";
      this.btnRetry.Padding = new System.Windows.Forms.Padding(2);
      this.btnRetry.Size = new System.Drawing.Size(61, 29);
      this.btnRetry.TabIndex = 3;
      this.btnRetry.Text = "&Retry";
      this.btnRetry.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnRetry.UseVisualStyleBackColor = true;
      //
      // btnIgnore
      //
      this.btnIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnIgnore.AutoSize = true;
      this.btnIgnore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Ignore;
      this.btnIgnore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnIgnore.ForeColor = System.Drawing.Color.Black;
      this.btnIgnore.Location = new System.Drawing.Point(299, 85);
      this.btnIgnore.Name = "btnIgnore";
      this.btnIgnore.Padding = new System.Windows.Forms.Padding(2);
      this.btnIgnore.Size = new System.Drawing.Size(53, 29);
      this.btnIgnore.TabIndex = 4;
      this.btnIgnore.Text = "&Ignore";
      this.btnIgnore.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
      this.btnIgnore.UseVisualStyleBackColor = true;
      //
      // btnAbort
      //
      this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAbort.AutoSize = true;
      this.btnAbort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
      this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
      this.btnAbort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnAbort.ForeColor = System.Drawing.Color.Black;
      this.btnAbort.Location = new System.Drawing.Point(358, 85);
      this.btnAbort.Name = "btnAbort";
      this.btnAbort.Padding = new System.Windows.Forms.Padding(2);
      this.btnAbort.Size = new System.Drawing.Size(53, 29);
      this.btnAbort.TabIndex = 5;
      this.btnAbort.Text = "&Abort";
      this.btnAbort.Theme = Aras.Tools.InnovatorAdmin.Controls.FlatButtonTheme.LightGray;
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
      this.tbcMain.Location = new System.Drawing.Point(3, 97);
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
      this.txtErrorDetails.Helper = null;
      this.txtErrorDetails.Location = new System.Drawing.Point(3, 3);
      this.txtErrorDetails.Name = "txtErrorDetails";
      this.txtErrorDetails.ReadOnly = false;
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
      this.txtQuery.Helper = null;
      this.txtQuery.Location = new System.Drawing.Point(0, 0);
      this.txtQuery.Margin = new System.Windows.Forms.Padding(0);
      this.txtQuery.Name = "txtQuery";
      this.txtQuery.ReadOnly = false;
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
    private Controls.FlatButton btnShowDetails;
    private System.Windows.Forms.TabControl tbcMain;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private Controls.FlatButton btnRetry;
    private Controls.FlatButton btnIgnore;
    private Controls.FlatButton btnAbort;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private Editor.EditorControl txtErrorDetails;
    private Editor.EditorControl txtQuery;
  }
}
