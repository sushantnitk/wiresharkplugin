using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace CSV_import_export
{
	public partial class frmExport : Form
	{
		public frmExport()
		{
			InitializeComponent();
		}

		private void frmExport_Load(object sender, EventArgs e)
		{
			loadTables();
		}


        /*
         * Loads list of user tables from the SQL database, and fills
         * a ListBox control with tatble names.
         */

		private void loadTables()
		{
            // Conncets to database, and selects the table names.
            SqlConnection cn = new SqlConnection(prop.sqlConnString);
			SqlDataAdapter da = new SqlDataAdapter("select name from dbo.sysobjects where xtype = 'U' and name <> 'dtproperties' order by name", cn);
			DataTable dt = new DataTable();
			
            // Fills the list to an DataTable.
			da.Fill(dt);

            // Clears the ListBox
			this.lbxTables.Items.Clear();

            // Fills the table names to the ListBox.
            // Notifies user if there is no user table in the database yet.
			if (dt.Rows.Count == 0)
			{
				MessageBox.Show("There is no user table in the specified database. Import a CSV file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				this.lbxTables.Items.Add("< no user table in database >");
				this.btnExportToCSV.Enabled = false;
			}
			else
			{
				this.btnExportToCSV.Enabled = true;

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					this.lbxTables.Items.Add(dt.Rows[i][0].ToString());
				}
				this.lbxTables.SelectedIndex = 0;

			}

		}

        /* Refresh the list of user tables.
         */

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			loadTables();
		}


        /*
         * Returns the specified separator character
         */

		private string separator
		{
			get
			{
				if (rdbSemicolon.Checked)
				{
					return ";";
				}
				else if (rdbTab.Checked)
				{
					return "\t";
				}
				else if (rdbSeparatorOther.Checked)
				{
					if (txtSeparatorOtherChar.Text.Length == 1)
					{
						return txtSeparatorOtherChar.Text;
					}
					else
					{
						MessageBox.Show("Invalid separator character.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						rdbSemicolon.Checked = true;
						return ";";
					}
				}
				else
				{
					return ";";
				}
			}
		}


        
		private void txtSeparatorOtherChar_TextChanged(object sender, EventArgs e)
		{
			this.rdbSeparatorOther.Checked = true;
		}


        /*
         * Returnes the appropriate Encoding object.
         */

		private Encoding encodingCSV
		{
			get
			{
				if (rdbUnicode.Checked)
				{
					return Encoding.Unicode;
				}
				else if (rdbASCII.Checked)
				{
					return Encoding.ASCII;
				}
				else if (rdbUTF7.Checked)
				{
					return Encoding.UTF7;
				}
				else if (rdbUTF8.Checked)
				{
					return Encoding.UTF8;
				}
				else
				{
					return Encoding.Unicode;
				}

				// You can add other options, for ex.:
				//return Encoding.GetEncoding("iso-8859-2");
				//return Encoding.Default;
			}
		}


		private void btnExportToCSV_Click(object sender, EventArgs e)
		{
			exportToCSV();
		}

        /*
         * Asks a filename and location from the user to export the data, and
         * runs the export operation.
         */

		private void exportToCSV()
		{

            //Asks the filenam with a SaveFileDialog control.

			SaveFileDialog saveFileDialogCSV = new SaveFileDialog();
			saveFileDialogCSV.InitialDirectory = Application.ExecutablePath.ToString();

			saveFileDialogCSV.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
			saveFileDialogCSV.FilterIndex = 1;
			saveFileDialogCSV.RestoreDirectory = true;

			if (saveFileDialogCSV.ShowDialog() == DialogResult.OK)
			{
                // Runs the export operation if the given filenam is valid.
				exportToCSVfile(saveFileDialogCSV.FileName.ToString());
			}
		}



        /*
         * Exports data to the CSV file.
         */

		private void exportToCSVfile(string fileOut)
		{
            // Connects to the database, and makes the select command.
            SqlConnection conn = new SqlConnection(prop.sqlConnString);
			string sqlQuery = "select * from " + this.lbxTables.SelectedItem.ToString();
			SqlCommand command = new SqlCommand(sqlQuery, conn);
			conn.Open();
			
            // Creates a SqlDataReader instance to read data from the table.
            SqlDataReader dr = command.ExecuteReader();

            // Retrives the schema of the table.
			DataTable dtSchema = dr.GetSchemaTable();

            // Creates the CSV file as a stream, using the given encoding.
			StreamWriter sw = new StreamWriter(fileOut, false, this.encodingCSV);
			
            string strRow; // represents a full row

            // Writes the column headers if the user previously asked that.
			if (this.chkFirstRowColumnNames.Checked)
			{
				sw.WriteLine(columnNames(dtSchema, this.separator));
			}

            // Reads the rows one by one from the SqlDataReader
            // transfers them to a string with the given separator character and
            // writes it to the file.
			while (dr.Read())
			{
				strRow = "";
				for (int i = 0; i < dr.FieldCount; i++)
				{
					strRow += dr.GetString(i);
					if (i < dr.FieldCount - 1)
					{
						strRow += this.separator;
					}
				}
				sw.WriteLine(strRow);
			}


            // Closes the text stream and the database connenction.
			sw.Close();
			conn.Close();

            // Notifies the user.
			MessageBox.Show("ready");
		}


        /*
         * Retrieves the header row from the schema table.
         */

		private string columnNames(DataTable dtSchemaTable, string delimiter)
		{
			string strOut = "";
			if (delimiter.ToLower() == "tab")
			{
				delimiter = "\t";
			}

			for (int i = 0; i < dtSchemaTable.Rows.Count; i++)
			{
				strOut += dtSchemaTable.Rows[i][0].ToString();
				if (i < dtSchemaTable.Rows.Count - 1)
				{
					strOut += delimiter;
				}

			}
			return strOut;
		}




	}

}