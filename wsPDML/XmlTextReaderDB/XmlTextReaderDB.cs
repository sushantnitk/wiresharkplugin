using System;
using System.Xml;
using System.Text;
using System.Collections;

namespace XmlTextReaderDB.Component {
    /// <summary>
    /// Copyright 2001.  Dan Wahlin - http://www.XMLforASP.NET
    /// </summary>
    public class SQLGenerator {

        public SQLInfo CreateSQLStatement(string xmlPath) {
            XmlTextReader reader = null;
            Hashtable fieldNamesValues = new Hashtable();
            StringBuilder sqlStatements = new StringBuilder();
            bool error = false;
            //Create Return Object
            SQLInfo sqlInfo = new SQLInfo();
            try {
                reader = new XmlTextReader(xmlPath);
                //Read through the XML stream and find proper tokens
                while (reader.Read()) {
                    if (!error) { //Stop parsing if problem is encountered
                        if (reader.NodeType == XmlNodeType.Element) {
                            //Get the name of the XML token
                            switch (reader.Name.ToLower()) {
                                case "customer":
                                    if (reader.HasAttributes) {
                                        string customerID = reader.GetAttribute("id");
                                        if (customerID != String.Empty) {
                                            fieldNamesValues.Add("CustomerID","'" + customerID + "'");

                                        } else {
                                            sqlInfo.Status = 1;
                                            sqlInfo.StatusMessage = "ID attribute empty on customer element";
                                            sqlInfo.SQL = null;
                                            error = true;
                                        }
                                    } else {
                                        sqlInfo.Status = 1;
                                        sqlInfo.StatusMessage = "No attributes on customer element";
                                        sqlInfo.SQL = null;
                                        error = true;
                                    }
                                    break;
                                case "companyname":
                                    string companyName = reader.ReadString();
                                    if (companyName != String.Empty) {
                                        fieldNamesValues.Add("CompanyName","'" + companyName + "'");
                                    } else {
                                        sqlInfo.Status = 1;
                                        sqlInfo.StatusMessage = "CompanyName element is empty.";
                                        sqlInfo.SQL = null;
                                        error = true;
                                    }
                                    break;
                                case "contactname":
                                    if (reader.HasAttributes) {
                                        fieldNamesValues.Add("ContactName","'" + reader.GetAttribute("name") + "'");
                                        fieldNamesValues.Add("ContactTitle","'" + reader.GetAttribute("title") + "'");
                                    }
                                    break;
                                case "address":
                                    if (reader.HasAttributes) {
                                        fieldNamesValues.Add("Address","'" + reader.GetAttribute("street") + "'");
                                        fieldNamesValues.Add("City","'" + reader.GetAttribute("city") + "'");
                                        fieldNamesValues.Add("Region","'" + reader.GetAttribute("state") + "'");
                                        fieldNamesValues.Add("PostalCode","'" + reader.GetAttribute("zip") + "'");
                                        fieldNamesValues.Add("Country","'" + reader.GetAttribute("country") + "'");
                                    }
                                    break;  
                                case "busphone":
                                    if (reader.HasAttributes) {
                                        fieldNamesValues.Add("Phone","'" + reader.GetAttribute("busLine") + "'");
                                    }
                                    break;
                                case "busfax":
                                    if (reader.HasAttributes) {
                                        fieldNamesValues.Add("Fax","'" + reader.GetAttribute("busLine") + "'");
                                    }
                                    break;
                            } //switch
                        } //XmlNodeType check
                    } else {
                        break;
                    }
                    if (reader.NodeType == XmlNodeType.EndElement) {
                        if (reader.Name.ToLower() == "customer") {
                            string[] FVArray = AddSeparator(fieldNamesValues,',');
                            string fields = FVArray[0];
                            string fieldVals = FVArray[1];
                            sqlStatements.Append("INSERT INTO Customers (" + fields + ") VALUES (" + fieldVals + ");");
                            //Clear out ArrayLists to handle multiple XML records
                            fieldNamesValues.Clear();
                        }
                    }
                } //End While
                if (!error) {
                    sqlInfo.Status = 0;
                    sqlInfo.StatusMessage = String.Empty;
                    sqlInfo.SQL = sqlStatements.ToString();
                }

                return sqlInfo;
    
            } catch (Exception exp) {
                sqlInfo.Status = 1;
                sqlInfo.StatusMessage = exp.Message + "\n\n" + exp.StackTrace;
                sqlInfo.SQL = null;
                return sqlInfo;
            }
            finally {
                if (reader != null) reader.Close();
            }
        }

        private string[] AddSeparator(Hashtable fv,char sep) {
            int len = fv.Count;
            int i = 0;
            StringBuilder sbFields = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            IDictionaryEnumerator fvEnum = fv.GetEnumerator();
            while (fvEnum.MoveNext()) {
                sbFields.Append(fvEnum.Key);
                if (i < len -1) sbFields.Append(sep);
                sbValues.Append(fvEnum.Value);
                if (i < len -1) sbValues.Append(sep);
                i++;
            }
            string[] output = {sbFields.ToString(),sbValues.ToString()};
            return output;
        }

    }


    public struct SQLInfo {
        int _status;
        string _statusMessage;
        string _sql;

        public int Status {
            get {
                return _status;
            }
            set {
                _status = value;
            }
        }
        public string StatusMessage {
            get {
                return _statusMessage;
            }
            set {
                _statusMessage = value;
            }
        }
        public string SQL {
            get {
                return _sql;
            }
            set {
                _sql = value;
            }
        }
    }
    
}
