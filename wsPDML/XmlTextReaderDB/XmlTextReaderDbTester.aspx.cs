using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using XmlTextReaderDB.Component;

namespace XmlTextReaderDB.Web {
	/// <summary>
	/// Summary description for XmlTextReaderDbTester.
	/// </summary>
	public class XmlTextReaderDbTester : System.Web.UI.Page	{
		public XmlTextReaderDbTester()		{
			Page.Init += new System.EventHandler(Page_Init);
		}

        private void Page_Load(object sender, System.EventArgs e)	{
            string xmlPath = Server.MapPath("customers.xml");
	        SQLGenerator sqlGenerator = new SQLGenerator();
            SQLInfo sqlInfo = sqlGenerator.CreateSQLStatement(xmlPath);
            if (sqlInfo.Status == 0) {
                Response.Write("<b>SQL Output:</b><p />" + sqlInfo.SQL + "<p />");
                /*
                Execute the SQL statement
                SqlConnection dataConn = null;
                try {
                    string connStr = ConfigurationSettings.AppSettings["dsn"];
                    dataConn = new SqlConnection(connStr);
                    SqlCommand cmd = new SqlCommand(sqlInfo.SQL,dataConn);
                    dataConn.Open();
                    cmd.ExecuteNonQuery();
                    Response.Write("<b>Database updated!</b>");
                }
                catch (Exception exp) {
                    Response.Write("<b>Error: </b>" + exp.Message);
                }
                finally {
                    dataConn.Close();
                }
                */
            } else {
                Response.Write("<b>Error Occurred:</b><p />" + sqlInfo.StatusMessage);
            }            
        }

		private void Page_Init(object sender, EventArgs e)	{

			InitializeComponent();
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
            this.Load += new System.EventHandler(this.Page_Load);

        }
		#endregion
	}
}
