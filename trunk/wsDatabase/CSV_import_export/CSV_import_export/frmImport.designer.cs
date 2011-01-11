namespace CSV_import_export
{
	partial class frmImport
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtFileToImport = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.dataGridView_preView = new System.Windows.Forms.DataGridView();
			this.chkFirstRowColumnNames = new System.Windows.Forms.CheckBox();
			this.gpbSeparator = new System.Windows.Forms.GroupBox();
			this.txtSeparatorOtherChar = new System.Windows.Forms.TextBox();
			this.rdbSeparatorOther = new System.Windows.Forms.RadioButton();
			this.rdbTab = new System.Windows.Forms.RadioButton();
			this.rdbSemicolon = new System.Windows.Forms.RadioButton();
			this.btnPreview = new System.Windows.Forms.Button();
			this.gpbEncoding = new System.Windows.Forms.GroupBox();
			this.rdbOEM = new System.Windows.Forms.RadioButton();
			this.rdbUnicode = new System.Windows.Forms.RadioButton();
			this.rdbAnsi = new System.Windows.Forms.RadioButton();
			this.btnSave_DataSet = new System.Windows.Forms.Button();
			this.lblProgress = new System.Windows.Forms.Label();
			this.btnSave_Direct = new System.Windows.Forms.Button();
			this.lblOwner = new System.Windows.Forms.Label();
			this.lblTableName = new System.Windows.Forms.Label();
			this.txtOwner = new System.Windows.Forms.TextBox();
			this.txtTableName = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_preView)).BeginInit();
			this.gpbSeparator.SuspendLayout();
			this.gpbEncoding.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "CSV file to load:";
			// 
			// txtFileToImport
			// 
			this.txtFileToImport.Location = new System.Drawing.Point(101, 12);
			this.txtFileToImport.Name = "txtFileToImport";
			this.txtFileToImport.Size = new System.Drawing.Size(292, 20);
			this.txtFileToImport.TabIndex = 1;
			this.txtFileToImport.TextChanged += new System.EventHandler(this.tbFile_TextChanged);
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(399, 12);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(108, 22);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnFileOpen_Click);
			// 
			// dataGridView_preView
			// 
			this.dataGridView_preView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView_preView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_preView.Location = new System.Drawing.Point(12, 155);
			this.dataGridView_preView.Name = "dataGridView_preView";
			this.dataGridView_preView.Size = new System.Drawing.Size(495, 222);
			this.dataGridView_preView.TabIndex = 3;
			// 
			// chkFirstRowColumnNames
			// 
			this.chkFirstRowColumnNames.AutoSize = true;
			this.chkFirstRowColumnNames.Checked = true;
			this.chkFirstRowColumnNames.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkFirstRowColumnNames.Location = new System.Drawing.Point(311, 56);
			this.chkFirstRowColumnNames.Name = "chkFirstRowColumnNames";
			this.chkFirstRowColumnNames.Size = new System.Drawing.Size(156, 17);
			this.chkFirstRowColumnNames.TabIndex = 4;
			this.chkFirstRowColumnNames.Text = "First row has column names";
			this.chkFirstRowColumnNames.UseVisualStyleBackColor = true;
			// 
			// gpbSeparator
			// 
			this.gpbSeparator.Controls.Add(this.txtSeparatorOtherChar);
			this.gpbSeparator.Controls.Add(this.rdbSeparatorOther);
			this.gpbSeparator.Controls.Add(this.rdbTab);
			this.gpbSeparator.Controls.Add(this.rdbSemicolon);
			this.gpbSeparator.Location = new System.Drawing.Point(15, 47);
			this.gpbSeparator.Name = "gpbSeparator";
			this.gpbSeparator.Size = new System.Drawing.Size(129, 94);
			this.gpbSeparator.TabIndex = 5;
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
			// btnPreview
			// 
			this.btnPreview.Location = new System.Drawing.Point(311, 124);
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Size = new System.Drawing.Size(198, 25);
			this.btnPreview.TabIndex = 6;
			this.btnPreview.Text = "Load preview (first 500 rows)";
			this.btnPreview.UseVisualStyleBackColor = true;
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
			// 
			// gpbEncoding
			// 
			this.gpbEncoding.Controls.Add(this.rdbOEM);
			this.gpbEncoding.Controls.Add(this.rdbUnicode);
			this.gpbEncoding.Controls.Add(this.rdbAnsi);
			this.gpbEncoding.Location = new System.Drawing.Point(160, 47);
			this.gpbEncoding.Name = "gpbEncoding";
			this.gpbEncoding.Size = new System.Drawing.Size(138, 94);
			this.gpbEncoding.TabIndex = 7;
			this.gpbEncoding.TabStop = false;
			this.gpbEncoding.Text = "Encoding";
			// 
			// rdbOEM
			// 
			this.rdbOEM.AutoSize = true;
			this.rdbOEM.Location = new System.Drawing.Point(6, 63);
			this.rdbOEM.Name = "rdbOEM";
			this.rdbOEM.Size = new System.Drawing.Size(49, 17);
			this.rdbOEM.TabIndex = 2;
			this.rdbOEM.Text = "OEM";
			this.rdbOEM.UseVisualStyleBackColor = true;
			// 
			// rdbUnicode
			// 
			this.rdbUnicode.AutoSize = true;
			this.rdbUnicode.Location = new System.Drawing.Point(6, 42);
			this.rdbUnicode.Name = "rdbUnicode";
			this.rdbUnicode.Size = new System.Drawing.Size(65, 17);
			this.rdbUnicode.TabIndex = 1;
			this.rdbUnicode.Text = "Unicode";
			this.rdbUnicode.UseVisualStyleBackColor = true;
			// 
			// rdbAnsi
			// 
			this.rdbAnsi.AutoSize = true;
			this.rdbAnsi.Checked = true;
			this.rdbAnsi.Location = new System.Drawing.Point(6, 19);
			this.rdbAnsi.Name = "rdbAnsi";
			this.rdbAnsi.Size = new System.Drawing.Size(50, 17);
			this.rdbAnsi.TabIndex = 0;
			this.rdbAnsi.TabStop = true;
			this.rdbAnsi.Text = "ANSI";
			this.rdbAnsi.UseVisualStyleBackColor = true;
			// 
			// btnSave_DataSet
			// 
			this.btnSave_DataSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSave_DataSet.Location = new System.Drawing.Point(320, 400);
			this.btnSave_DataSet.Name = "btnSave_DataSet";
			this.btnSave_DataSet.Size = new System.Drawing.Size(189, 28);
			this.btnSave_DataSet.TabIndex = 8;
			this.btnSave_DataSet.Text = "Save to database - with DataSet";
			this.btnSave_DataSet.UseVisualStyleBackColor = true;
			this.btnSave_DataSet.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// lblProgress
			// 
			this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblProgress.AutoSize = true;
			this.lblProgress.Location = new System.Drawing.Point(12, 380);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(91, 13);
			this.lblProgress.TabIndex = 9;
			this.lblProgress.Text = "Imported: 0 row(s)";
			// 
			// btnSave_Direct
			// 
			this.btnSave_Direct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSave_Direct.Location = new System.Drawing.Point(320, 439);
			this.btnSave_Direct.Name = "btnSave_Direct";
			this.btnSave_Direct.Size = new System.Drawing.Size(189, 28);
			this.btnSave_Direct.TabIndex = 10;
			this.btnSave_Direct.Text = "Save to database - directly";
			this.btnSave_Direct.UseVisualStyleBackColor = true;
			this.btnSave_Direct.Click += new System.EventHandler(this.btnSaveDirect_Click);
			// 
			// lblOwner
			// 
			this.lblOwner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblOwner.AutoSize = true;
			this.lblOwner.Location = new System.Drawing.Point(12, 415);
			this.lblOwner.Name = "lblOwner";
			this.lblOwner.Size = new System.Drawing.Size(41, 13);
			this.lblOwner.TabIndex = 11;
			this.lblOwner.Text = "Owner:";
			// 
			// lblTableName
			// 
			this.lblTableName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblTableName.AutoSize = true;
			this.lblTableName.Location = new System.Drawing.Point(12, 444);
			this.lblTableName.Name = "lblTableName";
			this.lblTableName.Size = new System.Drawing.Size(66, 13);
			this.lblTableName.TabIndex = 12;
			this.lblTableName.Text = "Table name:";
			// 
			// txtOwner
			// 
			this.txtOwner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtOwner.Location = new System.Drawing.Point(88, 415);
			this.txtOwner.Name = "txtOwner";
			this.txtOwner.Size = new System.Drawing.Size(87, 20);
			this.txtOwner.TabIndex = 13;
			this.txtOwner.Text = "dbo";
			// 
			// txtTableName
			// 
			this.txtTableName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtTableName.Location = new System.Drawing.Point(88, 444);
			this.txtTableName.Name = "txtTableName";
			this.txtTableName.Size = new System.Drawing.Size(216, 20);
			this.txtTableName.TabIndex = 14;
			// 
			// frmImport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(519, 481);
			this.Controls.Add(this.txtTableName);
			this.Controls.Add(this.txtOwner);
			this.Controls.Add(this.lblTableName);
			this.Controls.Add(this.lblOwner);
			this.Controls.Add(this.btnSave_Direct);
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.btnSave_DataSet);
			this.Controls.Add(this.gpbEncoding);
			this.Controls.Add(this.btnPreview);
			this.Controls.Add(this.gpbSeparator);
			this.Controls.Add(this.chkFirstRowColumnNames);
			this.Controls.Add(this.dataGridView_preView);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtFileToImport);
			this.Controls.Add(this.label1);
			this.MinimumSize = new System.Drawing.Size(527, 515);
			this.Name = "frmImport";
			this.Text = "Import CSV";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView_preView)).EndInit();
			this.gpbSeparator.ResumeLayout(false);
			this.gpbSeparator.PerformLayout();
			this.gpbEncoding.ResumeLayout(false);
			this.gpbEncoding.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtFileToImport;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.DataGridView dataGridView_preView;
		private System.Windows.Forms.CheckBox chkFirstRowColumnNames;
		private System.Windows.Forms.GroupBox gpbSeparator;
		private System.Windows.Forms.RadioButton rdbSeparatorOther;
		private System.Windows.Forms.RadioButton rdbTab;
		private System.Windows.Forms.RadioButton rdbSemicolon;
		private System.Windows.Forms.TextBox txtSeparatorOtherChar;
		private System.Windows.Forms.Button btnPreview;
		private System.Windows.Forms.GroupBox gpbEncoding;
		private System.Windows.Forms.RadioButton rdbOEM;
		private System.Windows.Forms.RadioButton rdbUnicode;
		private System.Windows.Forms.RadioButton rdbAnsi;
		private System.Windows.Forms.Button btnSave_DataSet;
		private System.Windows.Forms.Label lblProgress;
		private System.Windows.Forms.Button btnSave_Direct;
		private System.Windows.Forms.Label lblOwner;
		private System.Windows.Forms.Label lblTableName;
		private System.Windows.Forms.TextBox txtOwner;
		private System.Windows.Forms.TextBox txtTableName;
	}
}