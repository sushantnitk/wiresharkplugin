using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FollowSccpStream
{
    class ColumnFormat
    {
        //        <Query Kind="Statements">
        //  <IncludePredicateBuilder>true</IncludePredicateBuilder>
        //</Query>

        public static void FormatColumn()
        {
            bool flag = false;
            bool haveWrite = false;
            string sLine = null;
            //string srLine = null;
            HashSet<string> hs = new HashSet<string>();
            HashSet<string> hsClone = new HashSet<string>();
            StreamReader objReader = new StreamReader(@"C:\Documents and Settings\Administrator\Application Data\Wireshark\preferences");
            while (!objReader.EndOfStream)
            {
                sLine = objReader.ReadLine();
                if (sLine.IndexOf("column.format:") != -1)
                    flag = true;
                if (flag == true)
                    if (sLine.IndexOf("######## User Interface: Font ########") != -1)
                        flag = false;

                if (!flag)
                {
                    hs.Add(sLine);
                }
                else
                {
                    if (!haveWrite)
                    {
                        /*
                        StreamReader sr = new StreamReader(@"G:\wiresharkplugin\wsDatabase\column.format");
                        while (!sr.EndOfStream)
                        {
                            srLine = sr.ReadLine();
                            hs.Add(srLine);
                        }
                         * 
                         * column.format: 
	                            "No.", "%m",
	                            "Time", "%Yt",
	                            "Source", "%s",
	                            "Destination", "%d",
	                            "Destination Local Reference", "%Cus:sccp.dlr",
	                            "Source Local Reference", "%Cus:sccp.slr",
	                            "Protocol", "%p",
	                            "DTAP Mobility Management Message Type", "%Cus:gsm_a.dtap_msg_mm_type",
	                            "Message Type", "%Cus:sccp.message_type",
	                            "OPC", "%Cus:m3ua.protocol_data_opc",
	                            "DPC", "%Cus:m3ua.protocol_data_dpc",
	                            "TMSI/P-TMSI", "%Cus:gsm_a.tmsi",
	                            "IMSI", "%Cus:gsm_a.imsi",
	                            "Info", "%i"
                         * 
                         * "Time", "%At",
                         */
                        hsClone.Add("column.format: ");
                        hsClone.Add("	'No.', '%m',");
                        hsClone.Add("	'Time', '%At',");
                        //hsClone.Add("	'Source', '%s',");
                        //hsClone.Add("	'Destination', '%d',");
                        hsClone.Add("	'IMSI', '%Cus:gsm_a.imsi',");
                        hsClone.Add("	'TMSI/P-TMSI', '%Cus:gsm_a.tmsi',");
                        hsClone.Add("	'OPC', '%Cus:m3ua.protocol_data_opc',");
                        hsClone.Add("	'DPC', '%Cus:m3ua.protocol_data_dpc',");
                        hsClone.Add("	'Source Local Reference', '%Cus:sccp.slr',");
                        hsClone.Add("	'Destination Local Reference', '%Cus:sccp.dlr',");
                        //hs.Add("	'Source Local Reference', '%Cus:sccp.slr',");
                        //hs.Add("	'Protocol', '%p',");
                        hsClone.Add("	'Info', '%i'");
                        foreach (string s in hsClone)
                            hs.Add(s.Replace("'","\""));
                        haveWrite = true;
                    }
                }
            }
            objReader.Close();

            StreamWriter objWriter = new StreamWriter(@"C:\Documents and Settings\Administrator\Application Data\Wireshark\preferences");
            foreach (string s in hs)
                objWriter.WriteLine(s);
            objWriter.Flush();
            objWriter.Close();
        }
    }
}
