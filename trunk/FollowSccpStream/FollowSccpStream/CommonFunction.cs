using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Data.Linq;

namespace FollowSccpStream
{
    class CommonFunction
    {
        //private static string connString = "Data Source=192.168.1.9;Initial Catalog=sz_23B_20100920;Persist Security Info=True;User ID=weihp;Password=admin123456";
        public static string ConnString = "Data Source=.\\sqlexpress;Initial Catalog=sz04a_mc_all;Integrated Security=True";
        //public static DataClasses1DataContext MyDatabase= new DataClasses1DataContext(ConnString);
        //public static HashSet<AMessageDetail> messagelist = new HashSet<AMessageDetail>();
        public static HashSet<AMessageDetail> MessageList = new HashSet<AMessageDetail>();
        public static string ExecuteCmd(string cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            //调用程序名 
            {
                startInfo.Arguments = "/C " + cmd;
                //调用命令 CMD 
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
            }

            Process p = Process.Start(startInfo);
            string strOutput = p.StandardOutput.ReadToEnd();
            string strError = p.StandardError.ReadToEnd();

            p.WaitForExit();

            if ((strOutput.Length != 0))
            {
                return strOutput;
            }
            else if ((strError.Length != 0))
            {
                return strError;
            }

            return "";
        }
       


    }
}
