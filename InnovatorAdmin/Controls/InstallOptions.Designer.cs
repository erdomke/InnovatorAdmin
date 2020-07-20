namespace InnovatorAdmin.Controls
{
    partial class InstallOptions
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.radManageState = new System.Windows.Forms.RadioButton();
      this.radManageStatePreview = new System.Windows.Forms.RadioButton();
      this.radAppend = new System.Windows.Forms.RadioButton();
      this.chkAddPackage = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.chkAddPackage, 1, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(681, 353);
      this.tableLayoutPanel1.TabIndex = 6;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.radManageState);
      this.groupBox1.Controls.Add(this.radManageStatePreview);
      this.groupBox1.Controls.Add(this.radAppend);
      this.groupBox1.Location = new System.Drawing.Point(103, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(475, 147);
      this.groupBox1.TabIndex = 10;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Install Mode";
      // 
      // radManageState
      // 
      this.radManageState.AutoSize = true;
      this.radManageState.Location = new System.Drawing.Point(6, 104);
      this.radManageState.Name = "radManageState";
      this.radManageState.Size = new System.Drawing.Size(153, 17);
      this.radManageState.TabIndex = 2;
      this.radManageState.Text = "Manage State (no preview)";
      this.radManageState.UseVisualStyleBackColor = true;
      // 
      // radManageStatePreview
      // 
      this.radManageStatePreview.AutoSize = true;
      this.radManageStatePreview.Location = new System.Drawing.Point(6, 55);
      this.radManageStatePreview.Name = "radManageStatePreview";
      this.radManageStatePreview.Size = new System.Drawing.Size(294, 43);
      this.radManageStatePreview.TabIndex = 1;
      this.radManageStatePreview.Text = "Manage State (with preview)\r\nAdd, edit, nullify, and/or delete items such that th" +
    "e target \r\ninstallation matches the package after installation";
      this.radManageStatePreview.UseVisualStyleBackColor = true;
      // 
      // radAppend
      // 
      this.radAppend.AutoSize = true;
      this.radAppend.Checked = true;
      this.radAppend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radAppend.Location = new System.Drawing.Point(6, 19);
      this.radAppend.Name = "radAppend";
      this.radAppend.Size = new System.Drawing.Size(235, 30);
      this.radAppend.TabIndex = 0;
      this.radAppend.TabStop = true;
      this.radAppend.Text = "Append Package\r\nAdd new items and edit existing ones";
      this.radAppend.UseVisualStyleBackColor = true;
      // 
      // chkAddPackage
      // 
      this.chkAddPackage.AutoSize = true;
      this.chkAddPackage.Location = new System.Drawing.Point(103, 333);
      this.chkAddPackage.Name = "chkAddPackage";
      this.chkAddPackage.Size = new System.Drawing.Size(152, 17);
      this.chkAddPackage.TabIndex = 9;
      this.chkAddPackage.Text = "Add Package to Database";
      this.chkAddPackage.UseVisualStyleBackColor = true;
      // 
      // InstallOptions
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "InstallOptions";
      this.Size = new System.Drawing.Size(681, 353);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox chkAddPackage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radManageState;
        private System.Windows.Forms.RadioButton radManageStatePreview;
        private System.Windows.Forms.RadioButton radAppend;
    }
}
