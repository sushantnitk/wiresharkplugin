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
    class common
    {
        //private static string connString = "Data Source=192.168.1.9;Initial Catalog=sz_23B_20100920;Persist Security Info=True;User ID=weihp;Password=admin123456";
        public static string connString = "Data Source=.\\sqlexpress;Initial Catalog=sz04a_mc_all;Integrated Security=True";
        public static DataClasses1DataContext mydb = new DataClasses1DataContext(connString);
        //public static HashSet<LA_update> messagelist = new HashSet<LA_update>();
        public static Dictionary<int?, LA_update> messagelist = new Dictionary<int?, LA_update>();
        static void InitTable()
        {
            var typeName = "System.Data.Linq.SqlClient.SqlBuilder";
            var type = typeof(DataContext).Assembly.GetType(typeName);
            var bf = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
            var metaTable = mydb.Mapping.GetTable(typeof(LA_update1));
            var sql = type.InvokeMember("GetCreateTableCommand", bf, null, null, new[] { metaTable });
            string delSql = @"if exists (select 1 from  sysobjects where  id = object_id('dbo.LA_update1') and   type = 'U')
                drop table dbo.LA_update1";
            mydb.ExecuteCommand(delSql.ToString());
            mydb.ExecuteCommand(sql.ToString());
            GC.Collect();
            GC.Collect();

            var totalMessge = mydb.LA_update.Where(e => e.FileNum == 0);
           // FollowSccpStream(totalMessge);
            GC.Collect();
            GC.Collect();
           // SendOrders(DefineFlow(totalMessge), "LA_update1");
            GC.Collect();
            GC.Collect();
        }
        static void SendOrders(IEnumerable<LA_update1> mFlow, string tablename)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (SqlConnection con = new SqlConnection(common.connString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    var newOrders = mFlow;
                    SqlBulkCopy bc = new SqlBulkCopy(con,
                      SqlBulkCopyOptions.CheckConstraints |
                      SqlBulkCopyOptions.FireTriggers |
                      SqlBulkCopyOptions.KeepNulls, tran);
                    bc.BatchSize = 10;
                    bc.DestinationTableName = tablename;
                    bc.WriteToServer(newOrders.AsDataReader());
                    tran.Commit();
                }
                con.Close();
            }
            sw.Stop();
            Console.WriteLine(tablename + "---" + sw.Elapsed.TotalSeconds.ToString() + "---");
            GC.Collect();
            GC.Collect();
            Console.ReadKey();
        }

    }
}
