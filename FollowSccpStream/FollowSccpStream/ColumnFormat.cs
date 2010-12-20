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

        public void FormatColumn()
        {
            bool flag = false;
            bool haveWrite = false;
            string sLine = null;
            string srLine = null;
            HashSet<string> hs = new HashSet<string>();
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
                        StreamReader sr = new StreamReader(@"G:\wiresharkplugin\wsDatabase\column.format");
                        while (!sr.EndOfStream)
                        {
                            srLine = sr.ReadLine();
                            hs.Add(srLine);
                        }
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
