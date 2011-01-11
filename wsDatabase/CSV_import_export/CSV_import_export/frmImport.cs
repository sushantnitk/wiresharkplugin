using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CSV_import_export
{
	public partial class frmImport : Form
	{
		public frmImport()
		{
			InitializeComponent();
		}

		private string fileCSV;		//full file name
		private string dirCSV;		//directory of file to import
		private string fileNevCSV;	//name (with extension) of file to import - field

		public string FileNevCSV	//name (with extension) of file to import - property
		{
			get { return fileNevCSV; }
			set { fileNevCSV = value; }
		}

		private string strFormat;	//CSV separator character
		private string strEncoding; //Encoding of CSV file

		private long rowCount = 0;	//row number of source file
		


		// Browses file with OpenFileDialog control

		private void btnFileOpen_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialogCSV = new OpenFileDialog();

			openFileDialogCSV.InitialDirectory = Application.ExecutablePath.ToString();
			openFileDialogCSV.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
			openFileDialogCSV.FilterIndex = 1;
			openFileDialogCSV.RestoreDirectory = true;

			if (openFileDialogCSV.ShowDialog() == DialogResult.OK)
			{
				this.txtFileToImport.Text = openFileDialogCSV.FileName.ToString();
			}

		}



		// Delimiter character selection
		private void Format()
		{
			try
			{

				if (rdbSemicolon.Checked)
				{
					strFormat = "Delimited(;)";
				}
				else if (rdbTab.Checked)
				{
					strFormat = "TabDelimited";
				}
				else if (rdbSeparatorOther.Checked)
				{
					strFormat = "Delimited(" + txtSeparatorOtherChar.Text.Trim() + ")";
				}
				else
				{
					strFormat = "Delimited(;)";
				}


			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Format");
			}
			finally
			{
			}
		}


		// Encoding selection
		private void Encoding()
		{
			try
			{

				if (rdbAnsi.Checked)
				{
					strEncoding = "ANSI";
				}
				else if (rdbUnicode.Checked)
				{
					strEncoding = "Unicode";
				}
				else if (rdbOEM.Checked)
				{
					strEncoding = "OEM";
				}
				else
				{
					strEncoding = "ANSI";
				}


			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Encoding");
			}
			finally
			{
			}
		}



		/* Schema.ini File (Text File Driver)

		When the Text driver is used, the format of the text file is determined by using a
		schema information file. The schema information file, which is always named Schema.ini
		and always kept in the same directory as the text data source, provides the IISAM 
		with information about the general format of the file, the column name and data type
		information, and a number of other data characteristics*/

		private void writeSchema()
		{
			try
			{
				FileStream fsOutput = new FileStream(this.dirCSV + "\\schema.ini", FileMode.Create, FileAccess.Write);
				StreamWriter srOutput = new StreamWriter(fsOutput);
				string s1, s2, s3, s4, s5;

				s1 = "[" + this.FileNevCSV + "]";
				s2 = "ColNameHeader=" + chkFirstRowColumnNames.Checked.ToString();
				s3 = "Format=" + this.strFormat;
				s4 = "MaxScanRows=25";
				s5 = "CharacterSet=" + this.strEncoding;

				srOutput.WriteLine(s1.ToString() + "\r\n" + s2.ToString() + "\r\n" + s3.ToString() + "\r\n" + s4.ToString() + "\r\n" + s5.ToString());
				srOutput.Close();
				fsOutput.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "writeSchema");
			}
			finally
			{ }
		}

		/*
		 * Loads the csv file into a DataSet.
		 * 
		 * If the numberOfRows parameter is -1, it loads oll rows, otherwise it
		 * loads the first specified number of rows (for preview)
		 */

		public DataSet LoadCSV(int numberOfRows)
		{
			DataSet ds = new DataSet();
			try
			{
				// Creates and opens an ODBC connection
				string strConnString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + this.dirCSV.Trim() + ";Extensions=asc,csv,tab,txt;Persist Security Info=False";
				string sql_select;
				OdbcConnection conn;
				conn = new OdbcConnection(strConnString.Trim());
				conn.Open();

				//Creates the select command text
				if (numberOfRows == -1)
				{
					sql_select = "select * from [" + this.FileNevCSV.Trim() + "]";
				}
				else
				{
					sql_select = "select top " + numberOfRows + " * from [" + this.FileNevCSV.Trim() + "]";
				}

				//Creates the data adapter
				OdbcDataAdapter obj_oledb_da = new OdbcDataAdapter(sql_select, conn);
				
				//Fills dataset with the records from CSV file
				obj_oledb_da.Fill(ds, "csv");
				
				//closes the connection
				conn.Close();
			}
			catch (Exception e) //Error
			{
				MessageBox.Show(e.Message, "Error - LoadCSV",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			return ds;
		}


		// Checks if a file was given.
		private bool fileCheck()
		{
			if ((fileCSV == "") || (fileCSV == null) || (dirCSV == "") || (dirCSV == null) || (FileNevCSV == "") || (FileNevCSV == null))
			{
				MessageBox.Show("Select a CSV file to load first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			else
			{
				return true;
			}
		}




		private void btnPreview_Click(object sender, EventArgs e)
		{
			loadPreview();
		}


        /* Loads the preview of CSV file in the DataGridView control.
         */

		private void loadPreview()
		{
			try
			{
				// select format, encoding, an write the schema file
				Format();
				Encoding();
				writeSchema();

				// loads the first 500 rows from CSV file, and fills the
				// DataGridView control.
				this.dataGridView_preView.DataSource = LoadCSV(500);
				this.dataGridView_preView.DataMember = "csv";
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error - loadPreview", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			SaveToDatabase_withDataSet();
		}


		/*
		 * Loads the CSV file to a dataset, and imports data
		 * to the database with SqlBulkCopy.
		 */

		private void SaveToDatabase_withDataSet()
		{
			try
			{
				if (fileCheck())
				{
					// select format, encoding, and write the schema file
					Format();
					Encoding();
					writeSchema();

					// loads all rows from from csv file
					DataSet ds = LoadCSV(-1);

					// gets the number of rows
					this.rowCount = ds.Tables[0].Rows.Count;

					// Makes a DataTableReader, which reads data from data set.
					// It is nececery for bulk copy operation.
					DataTableReader dtr = ds.Tables[0].CreateDataReader();

					// Creates schema table. It gives column names for create table command.
					DataTable dt;
					dt = dtr.GetSchemaTable();

					// You can view that schema table if you want:
					//this.dataGridView_preView.DataSource = dt;

					// Creates a new and empty table in the sql database
					CreateTableInDatabase(dt, this.txtOwner.Text, this.txtTableName.Text, prop.sqlConnString);

					// Copies all rows to the database from the dataset.
					using (SqlBulkCopy bc = new SqlBulkCopy(prop.sqlConnString))
					{
						// Destination table with owner - this example doesn't
						// check the owner and table names!
						bc.DestinationTableName = "[" + this.txtOwner.Text + "].[" + this.txtTableName.Text + "]";

						// User notification with the SqlRowsCopied event
						bc.NotifyAfter = 100;
						bc.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
						
						// Starts the bulk copy.
						bc.WriteToServer(ds.Tables[0]);

						// Closes the SqlBulkCopy instance
						bc.Close();
					}

					// Writes the number of imported rows to the form
					this.lblProgress.Text = "Imported: " + this.rowCount.ToString() + "/" + this.rowCount.ToString() + " row(s)";
					this.lblProgress.Refresh();

					// Notifies user
					MessageBox.Show("ready");
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error - SaveToDatabase_withDataSet", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}



		/*
		 *  shows the progress of import operation
		 */

		private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
		{
			this.lblProgress.Text = "Imported: " + e.RowsCopied.ToString() + "/" + this.rowCount.ToString() + " row(s)";
			this.lblProgress.Refresh();
		}


		private void btnSaveDirect_Click(object sender, EventArgs e)
		{
			SaveToDatabaseDirectly();
		}

		/*
		 * Imports data to the database with SqlBulkCopy.
		 * This method doesn't use a temporary dataset, it loads
		 * data immediately from the ODBC connection
		 */

		private void SaveToDatabaseDirectly()
		{
			try
			{
				if (fileCheck())
				{
					// select format, encoding, and write the schema file
					Format();
					Encoding();
					writeSchema();

					// Creates and opens an ODBC connection
					string strConnString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + this.dirCSV.Trim() + ";Extensions=asc,csv,tab,txt;Persist Security Info=False";
					string sql_select;
					OdbcConnection conn;
					conn = new OdbcConnection(strConnString.Trim());
					conn.Open();

					//Counts the row number in csv file - with an sql query
					OdbcCommand commandRowCount = new OdbcCommand("SELECT COUNT(*) FROM [" + this.FileNevCSV.Trim() + "]", conn);
					this.rowCount = System.Convert.ToInt32(commandRowCount.ExecuteScalar());

					// Creates the ODBC command
					sql_select = "select * from [" + this.FileNevCSV.Trim() + "]";
					OdbcCommand commandSourceData = new OdbcCommand(sql_select, conn);

					// Makes on OdbcDataReader for reading data from CSV
					OdbcDataReader dataReader = commandSourceData.ExecuteReader();

					// Creates schema table. It gives column names for create table command.
					DataTable dt;
					dt = dataReader.GetSchemaTable();

					// You can view that schema table if you want:
					//this.dataGridView_preView.DataSource = dt;

					// Creates a new and empty table in the sql database
					CreateTableInDatabase(dt, this.txtOwner.Text, this.txtTableName.Text, prop.sqlConnString);

					// Copies all rows to the database from the data reader.
					using (SqlBulkCopy bc = new SqlBulkCopy("server=(local);database=Test_CSV_impex;Trusted_Connection=True"))
					{
						// Destination table with owner - this example doesn't
						// check the owner and table names!
						bc.DestinationTableName = "[" + this.txtOwner.Text + "].[" + this.txtTableName.Text + "]";

						// User notification with the SqlRowsCopied event
						bc.NotifyAfter = 100;
						bc.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

						// Starts the bulk copy.
						bc.WriteToServer(dataReader);

						// Closes the SqlBulkCopy instance
						bc.Close();
					}

					// Writes the number of imported rows to the form
					this.lblProgress.Text = "Imported: " + this.rowCount.ToString() + "/" + this.rowCount.ToString() + " row(s)";
					this.lblProgress.Refresh();
					
					// Notifies user
					MessageBox.Show("ready");
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error - SaveToDatabaseDirectly", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		/*
		 * Generates the create table command using the schema table, and
		 * runs it in the sql database.
		 */

		private bool CreateTableInDatabase(DataTable dtSchemaTable, string tableOwner, string tableName, string connectionString)
		{
			try
			{

				// Generates the create table command.
				// The first column of schema table contains the column names.
				// The data type is nvarcher(4000) in all columns.

				string ctStr = "CREATE TABLE [" + tableOwner + "].[" + tableName + "](\r\n";
				for (int i = 0; i < dtSchemaTable.Rows.Count; i++)
				{
					ctStr += "  [" + dtSchemaTable.Rows[i][0].ToString() + "] [nvarchar](4000) NULL";
					if (i < dtSchemaTable.Rows.Count)
					{
						ctStr += ",";
					}
					ctStr += "\r\n";
				}
				ctStr += ")";

				// You can check the sql statement if you want:
				//MessageBox.Show(ctStr);


				// Runs the sql command to make the destination table.
				
				SqlConnection conn = new SqlConnection(connectionString);
				SqlCommand command = conn.CreateCommand();
				command.CommandText = ctStr;
				conn.Open();
				command.ExecuteNonQuery();
				conn.Close();

				return true;

			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "CreateTableInDatabase", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		/*
		 * Makes file and directory information, and suggests a table name
		 */

		private void tbFile_TextChanged(object sender, EventArgs e)
		{
			// full file name
			this.fileCSV = this.txtFileToImport.Text;

			// creates a System.IO.FileInfo object to retrive information from selected file.
			System.IO.FileInfo fi = new System.IO.FileInfo(this.fileCSV);
			// retrives directory
			this.dirCSV = fi.DirectoryName.ToString();
			// retrives file name with extension
			this.FileNevCSV = fi.Name.ToString();

			// database table name
			this.txtTableName.Text = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length).Replace(" ", "_");
		}

		private void txtSeparatorOtherChar_TextChanged(object sender, EventArgs e)
		{
			this.rdbSeparatorOther.Checked = true;
		}

		
	}
}