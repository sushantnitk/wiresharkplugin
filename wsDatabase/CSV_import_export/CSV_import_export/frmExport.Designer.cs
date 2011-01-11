namespace CSV_import_export
{
	partial class frmExport
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
			this.lbl01 = new System.Windows.Forms.Label();
			this.lbxTables = new System.Windows.Forms.ListBox();
			this.gpbSeparator = new System.Windows.Forms.GroupBox();
			this.txtSeparatorOtherChar = new System.Windows.Forms.TextBox();
			this.rdbSeparatorOther = new System.Windows.Forms.RadioButton();
			this.rdbTab = new System.Windows.Forms.RadioButton();
			this.rdbSemicolon = new System.Windows.Forms.RadioButton();
			this.gpbEncoding = new System.Windows.Forms.GroupBox();
			this.rdbUnicode = new System.Windows.Forms.RadioButton();
			this.chkFirstRowColumnNames = new System.Windows.Forms.CheckBox();
			this.btnExportToCSV = new System.Windows.Forms.Button();
			this.rdbASCII = new System.Windows.Forms.RadioButton();
			this.rdbUTF7 = new System.Windows.Forms.RadioButton();
			this.rdbUTF8 = new System.Windows.Forms.RadioButton();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.gpbSeparator.SuspendLayout();
			this.gpbEncoding.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbl01
			// 
			this.lbl01.AutoSize = true;
			this.lbl01.Location = new System.Drawing.Point(8, 14);
			this.lbl01.Name = "lbl01";
			this.lbl01.Size = new System.Drawing.Size(119, 13);
			this.lbl01.TabIndex = 0;
			this.lbl01.Text = "Select a table to export:";
			// 
			// lbxTables
			// 
			this.lbxTables.FormattingEnabled = true;
			this.lbxTables.Location = new System.Drawing.Point(11, 30);
			this.lbxTables.Name = "lbxTables";
			this.lbxTables.Size = new System.Drawing.Size(163, 212);
			this.lbxTables.TabIndex = 1;
			// 
			// gpbSeparator
			// 
			this.gpbSeparator.Controls.Add(this.txtSeparatorOtherChar);
			this.gpbSeparator.Controls.Add(this.rdbSeparatorOther);
			this.gpbSeparator.Controls.Add(this.rdbTab);
			this.gpbSeparator.Controls.Add(this.rdbSemicolon);
			this.gpbSeparator.Location = new System.Drawing.Point(194, 30);
			this.gpbSeparator.Name = "gpbSeparator";
			this.gpbSeparator.Size = new System.Drawing.Size(156, 94);
			this.gpbSeparator.TabIndex = 6;
			this.gpbSeparator.TabStop = false;
			this.gpbSeparator.Text = "Separator";
			// 
			// txtSeparatorOtherChar
			// 
			this.txtSeparatorOtherChar.Location = new System.Drawing.Point(73, 66);
			this.txtSeparatorOtherChar.MaxLength = 1;
			this.txtSeparatorOtherChar.Name = "txtSeparatorOtherChar";
			this.txtSeparatorOtherChar.Size = new System.Drawing.Size(24, 20);
			this.txtSeparatorOtherChar.TabIndex = 3;
			this.txtSeparatorOtherChar.TextChanged += new System.EventHandler(this.txtSeparatorOtherChar_TextChanged);
			// 
			// rdbSeparatorOther
			// 
			this.rdbSeparatorOther.AutoSize = true;
			this.rdbSeparatorOther.Location = new System.Drawing.Point(6, 65);
			this.rdbSeparatorOther.Name = "rdbSeparatorOther";
			this.rdbSeparatorOther.Size = new System.Drawing.Size(54, 17);
			this.rdbSeparatorOther.TabIndex = 2;
			this.rdbSeparatorOther.Text = "Other:";
			this.rdbSeparatorOther.UseVisualStyleBackColor = true;
			// 
			// rdbTab
			// 
			this.rdbTab.AutoSize = true;
			this.rdbTab.Location = new System.Drawing.Point(6, 42);
			this.rdbTab.Name = "rdbTab";
			this.rdbTab.Size = new System.Drawing.Size(46, 17);
			this.rdbTab.TabIndex = 1;
			this.rdbTab.Text = "TAB";
			this.rdbTab.UseVisualStyleBackColor = true;
			// 
			// rdbSemicolon
			// 
			this.rdbSemicolon.AutoSize = true;
			this.rdbSemicolon.Checked = true;
			this.rdbSemicolon.Location = new System.Drawing.Point(6, 19);
			this.rdbSemicolon.Name = "rdbSemicolon";
			this.rdbSemicolon.Size = new System.Drawing.Size(74, 17);
			this.rdbSemicolon.TabIndex = 0;
			this.rdbSemicolon.TabStop = true;
			this.rdbSemicolon.Text = "Semicolon";
			this.rdbSemicolon.UseVisualStyleBackColor = true;
			// 
			// gpbEncoding
			// 
			this.gpbEncoding.Controls.Add(this.rdbUTF8);
			this.gpbEncoding.Controls.Add(this.rdbUTF7);
			this.gpbEncoding.Controls.Add(this.rdbASCII);
			this.gpbEncoding.Controls.Add(this.rdbUnicode);
			this.gpbEncoding.Location = new System.Drawing.Point(194, 130);
			this.gpbEncoding.Name = "gpbEncoding";
			this.gpbEncoding.Size = new System.Drawing.Size(156, 111);
			this.gpbEncoding.TabIndex = 9;
			this.gpbEncoding.TabStop = false;
			this.gpbEncoding.Text = "Encoding";
			// 
			// rdbUnicode
			// 
			this.rdbUnicode.AutoSize = true;
			this.rdbUnicode.Checked = true;
			this.rdbUnicode.Location = new System.Drawing.Point(6, 19);
			this.rdbUnicode.Name = "rdbUnicode";
			this.rdbUnicode.Size = new System.Drawing.Size(65, 17);
			this.rdbUnicode.TabIndex = 1;
			this.rdbUnicode.TabStop = true;
			this.rdbUnicode.Text = "Unicode";
			this.rdbUnicode.UseVisualStyleBackColor = true;
			// 
			// chkFirstRowColumnNames
			// 
			this.chkFirstRowColumnNames.AutoSize = true;
			this.chkFirstRowColumnNames.Checked = true;
			this.chkFirstRowColumnNames.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkFirstRowColumnNames.Location = new System.Drawing.Point(194, 257);
			this.chkFirstRowColumnNames.Name = "chkFirstRowColumnNames";
			this.chkFirstRowColumnNames.Size = new System.Drawing.Size(156, 17);
			this.chkFirstRowColumnNames.TabIndex = 8;
			this.chkFirstRowColumnNames.Text = "First row has column names";
			this.chkFirstRowColumnNames.UseVisualStyleBackColor = true;
			// 
			// btnExportToCSV
			// 
			this.btnExportToCSV.Location = new System.Drawing.Point(11, 280);
			this.btnExportToCSV.Name = "btnExportToCSV";
			this.btnExportToCSV.Size = new System.Drawing.Size(339, 32);
			this.btnExportToCSV.TabIndex = 10;
			this.btnExportToCSV.Text = "Export to CSV";
			this.btnExportToCSV.UseVisualStyleBackColor = true;
			this.btnExportToCSV.Click += new System.EventHandler(this.btnExportToCSV_Click);
			// 
			// rdbASCII
			// 
			this.rdbASCII.AutoSize = true;
			this.rdbASCII.Location = new System.Drawing.Point(6, 42);
			this.rdbASCII.Name = "rdbASCII";
			this.rdbASCII.Size = new System.Drawing.Size(52, 17);
			this.rdbASCII.TabIndex = 2;
			this.rdbASCII.Text = "ASCII";
			this.rdbASCII.UseVisualStyleBackColor = true;
			// 
			// rdbUTF7
			// 
			this.rdbUTF7.AutoSize = true;
			this.rdbUTF7.Location = new System.Drawing.Point(6, 65);
			this.rdbUTF7.Name = "rdbUTF7";
			this.rdbUTF7.Size = new System.Drawing.Size(52, 17);
			this.rdbUTF7.TabIndex = 3;
			this.rdbUTF7.Text = "UTF7";
			this.rdbUTF7.UseVisualStyleBackColor = true;
			// 
			// rdbUTF8
			// 
			this.rdbUTF8.AutoSize = true;
			this.rdbUTF8.Location = new System.Drawing.Point(6, 88);
			this.rdbUTF8.Name = "rdbUTF8";
			this.rdbUTF8.Size = new System.Drawing.Size(52, 17);
			this.rdbUTF8.TabIndex = 4;
			this.rdbUTF8.Text = "UTF8";
			this.rdbUTF8.UseVisualStyleBackColor = true;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Location = new System.Drawing.Point(11, 248);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(102, 26);
			this.btnRefresh.TabIndex = 11;
			this.btnRefresh.Text = "Refresh list";
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// frmExport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(363, 327);
			this.Controls.Add(this.btnRefresh);
			this.Controls.Add(this.btnExportToCSV);
			this.Controls.Add(this.gpbEncoding);
			this.Controls.Add(this.chkFirstRowColumnNames);
			this.Controls.Add(this.gpbSeparator);
			this.Controls.Add(this.lbxTables);
			this.Controls.Add(this.lbl01);
			this.Name = "frmExport";
			this.Text = "frmExport";
			this.Load += new System.EventHandler(this.frmExport_Load);
			this.gpbSeparator.ResumeLayout(false);
			this.gpbSeparator.PerformLayout();
			this.gpbEncoding.ResumeLayout(false);
			this.gpbEncoding.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl01;
		private System.Windows.Forms.ListBox lbxTables;
		private System.Windows.Forms.GroupBox gpbSeparator;
		private System.Windows.Forms.TextBox txtSeparatorOtherChar;
		private System.Windows.Forms.RadioButton rdbSeparatorOther;
		private System.Windows.Forms.RadioButton rdbTab;
		private System.Windows.Forms.RadioButton rdbSemicolon;
		private System.Windows.Forms.GroupBox gpbEncoding;
		private System.Windows.Forms.RadioButton rdbUnicode;
		private System.Windows.Forms.CheckBox chkFirstRowColumnNames;
		private System.Windows.Forms.Button btnExportToCSV;
		private System.Windows.Forms.RadioButton rdbUTF8;
		private System.Windows.Forms.RadioButton rdbUTF7;
		private System.Windows.Forms.RadioButton rdbASCII;
		private System.Windows.Forms.Button btnRefresh;
	}
}