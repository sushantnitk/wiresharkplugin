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
    class Program
    {
        public static DataClasses1DataContext mydb = new DataClasses1DataContext(common.connString);
        //没有出现sccp对之前的单tmsi,slr,dlr回调字典
        private static Dictionary<int?, string> dCallback = new Dictionary<int?, string>();
        //没有出现sccp对之前的imsi回调字典的补充
        private static Dictionary<int?, string> _dCallback = new Dictionary<int?, string>();
        //删除没有出现sccp对之前的回调字典记录，避免重复，即一个记录只用一次
        private static HashSet<int?> hRemovecall = new HashSet<int?>();
        //出现sccp对之后的只包含一项slr或者dlr的稀疏矩阵字典
        private static Dictionary<string, string> dConn = new Dictionary<string, string>();
        //opc+dpc+slr+dlr索引字典，索引到包标签
        private static Dictionary<int?, string> dFlow = new Dictionary<int?, string>();

        static void Main(string[] args)
        {
            Console.Write("是否初始化数据库？");
            string inittable = Console.ReadLine();
            if (inittable == "y")
            {
                Console.WriteLine(inittable);
                InitTable();
            }
            FlowStatistics.FlowStatics();


        }
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
            FollowSccpStream(totalMessge);
            GC.Collect();
            GC.Collect();
            SendOrders(DefineFlow(totalMessge), "LA_update1");
            GC.Collect();
            GC.Collect();
        }

        //根据消息寻呼做记录
        //通过回调的方式获取 opc+dpc+slr+dlr字典值的关键字的集合
        static void FollowSccpStream(IEnumerable<LA_update> totalMessge)
        {
            /*            for (int m = 0; m < 2; m++)
                       { */

            //int tNum=0;//回调参数
            foreach (LA_update i in totalMessge)
            {
                //寻呼消息
                if (i.sccp_slr == null && i.sccp_dlr == null)
                {
                    //寻呼消息，包标签都需要进入队列
                    if (i.tmsi != null)
                    {
                        dCallback.Add(i.PacketNum, i.tmsi);
                        dFlow.Add(i.PacketNum, i.tmsi);
                        //_dCallback.Add(i.imsi, i.PacketNum);
                    }
                    if (i.imsi != null)
                    {
                        _dCallback.Add(i.PacketNum, i.imsi);
                        if (!dFlow.ContainsKey(i.PacketNum))
                            dFlow.Add(i.PacketNum, i.imsi);
                    }
                    //dConn.Add(i.tmsi, i.PacketNum);  //开始记录包标签
                    //if (i.imsi != null)
                    //   dCallback.Add(i.PacketNum, i.imsi);
                    //dConn.Add(i.imsi, i.PacketNum);  //开始记录包标签
                }

                //寻呼响应
                if (i.sccp_slr != null && i.sccp_dlr == null)
                {
                    //查询稀疏矩阵
                    if (dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr))
                    {
                        //后续消息，Flow正常增加包标签
                        dFlow.Add(i.PacketNum, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_slr]);
                    }
                    //位置更新消息需要进入队列
                    else
                    {
                        dCallback.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr);//开始记录包标签
                        //dConn.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr
                    }

                    Dictionary<int?, string> d = new Dictionary<int?, string>();
                    foreach (var call in dCallback)
                        d.Add(call.Key,call.Value);
                    Dictionary<int?, string> _d = new Dictionary<int?, string>();
                    foreach (var call in _dCallback)
                        _d.Add(call.Key, call.Value);

                    //回调寻呼消息包标签 ，相当于把IMSI替换成opc+dpc+slr
                    if (i.tmsi != null)
                        foreach (var call in d)
                            if (call.Value == i.tmsi)
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_slr;//开始记录包标签
                    if (i.imsi != null)
                        foreach (var call in _d)
                            if (call.Value == i.imsi)
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_slr;//开始记录包标签
                }

                //CC消息
                if (i.sccp_slr != null && i.sccp_dlr != null)
                {
                    //Flow正常增加包标签
                    //if (!dFlow.ContainsKey(i.PacketNum))
                    dFlow.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);

                    //正常增加稀疏矩阵
                    if (!dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr))
                    {
                        dConn.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                        //回调位置更新包标签
                        foreach (var call in dCallback)
                            if (call.Value == i.m3ua_opc + i.m3ua_dpc + i.sccp_slr)
                            {
                                if (!dFlow.ContainsKey(call.Key))
                                    dFlow.Add(call.Key, dConn[call.Value]);  //Flow补充增加TMSI标签号
                                else
                                    dFlow[call.Key] = dConn[call.Value];
                                hRemovecall.Add(call.Key);
                            }
                    }


                    if (!dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr))
                    {
                        dConn.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                        //回调位置更新包标签
                        foreach (var call in dCallback)
                            if (call.Value == i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr)
                            {
                                if (!dFlow.ContainsKey(call.Key))
                                    dFlow.Add(call.Key, dConn[call.Value]);  //Flow补充增加TMSI标签号
                                else
                                    dFlow[call.Key] = dConn[call.Value];
                                hRemovecall.Add(call.Key);
                            }
                    }
                    if (!dConn.ContainsKey(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr))
                    {
                        dConn.Add(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                        //回调位置更新包标签
                        foreach (var call in dCallback)
                            if (call.Value == i.m3ua_dpc + i.m3ua_opc + i.sccp_slr)
                            {
                                if (!dFlow.ContainsKey(call.Key))
                                    dFlow.Add(call.Key, dConn[call.Value]);  //Flow补充增加TMSI标签号
                                else
                                    dFlow[call.Key] = dConn[call.Value];
                                hRemovecall.Add(call.Key);
                            }
                    }
                    if (!dConn.ContainsKey(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr))
                    {
                        dConn.Add(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                        //回调位置更新包标签
                        foreach (var call in dCallback)
                            if (call.Value == i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr)
                            {
                                if (!dFlow.ContainsKey(call.Key))
                                    dFlow.Add(call.Key, dConn[call.Value]);  //Flow补充增加TMSI标签号
                                else
                                    dFlow[call.Key] = dConn[call.Value];
                                hRemovecall.Add(call.Key);
                            }
                    }
                }

                //后续消息
                if (i.sccp_slr == null && i.sccp_dlr != null)
                {
                    //查询稀疏矩阵
                    if (dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr))
                    {

                        //后续消息，Flow正常增加包标签
                        dFlow.Add(i.PacketNum, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr]);//此处增加包标签
                    }
                    else
                    {
                        dCallback.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr);//开始记录包标签
                    }

                    Dictionary<int?, string> d = new Dictionary<int?, string>();
                    foreach (var call in dCallback)
                        d.Add(call.Key, call.Value);
                    Dictionary<int?, string> _d = new Dictionary<int?, string>();
                    foreach (var call in _dCallback)
                        _d.Add(call.Key, call.Value);

                    //回调寻呼消息包标签
                    if (i.tmsi != null)
                    {
                        foreach (var call in d)
                            if (call.Value == i.tmsi)
                                //if (!dFlow.ContainsKey(call.Key))
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr;//开始记录包标签
                        //{
                        //    dFlow.Add(call.Key, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_slr]);  //Flow补充增加TMSI标签号
                        //    hRemovecall.Add(call.Key);
                        //}
                    }
                    if (i.imsi != null)
                        foreach (var call in _d)
                            if (call.Value == i.imsi)
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr;//开始记录包标签
                }

                //删除稀疏矩阵的主键
                if (i.ip_version_MsgType == "SCCP.Released")
                {
                    dConn.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr);
                    dConn.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr);
                    dConn.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr);
                    dConn.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr);
                }

                //删除回调记录的主键
                foreach (var k in hRemovecall)
                {
                    dCallback.Remove(k);
                    if (_dCallback.ContainsKey(k))
                        _dCallback.Remove(k);
                }
            }
            /* } */
            Console.WriteLine(dFlow.Count());
            Console.WriteLine(dConn.Count());
        }
        static IEnumerable<LA_update1> DefineFlow(IEnumerable<LA_update> totalMessge)
        {
            var connLookup = dFlow.ToLookup(e => e.Value);
            //Dictionary<string, string> _dConn = new Dictionary<string, string>();

            foreach (var i in connLookup)
            {
                //Console.WriteLine(i.Key);
                //Console.WriteLine(i.Select(e=>e.Key).Aggregate((total, next)=>total+next));
                var nLst = i.Select(e => e.Key);
                //foreach (var n in nLst)
                //    Console.WriteLine(n);
                var nFlow = totalMessge.Where(e => nLst.Contains(e.PacketNum));
                nFlow = nFlow.OrderBy(e => e.PacketTime);
                //var sccp = nFlow.Where(e => e.ip_version_MsgType == "SCCP.Release Complete").FirstOrDefault();
                //if (sccp != null)
                //{ 
                foreach (var m in nFlow)
                {
                    LA_update1 mFlow = new LA_update1();
                    //Console.WriteLine(m.ip_version_MsgType);
                    mFlow.BeginFrameNum = m.BeginFrameNum;
                    mFlow.DumpFor = m.DumpFor;
                    mFlow.FileNum = m.FileNum;
                    mFlow.imsi = m.imsi;
                    mFlow.ip_version = m.ip_version;
                    mFlow.ip_version_MsgType = m.ip_version_MsgType;
                    mFlow.m3ua_dpc = m.m3ua_dpc;
                    mFlow.m3ua_opc = m.m3ua_opc;
                    mFlow.PacketNum = m.PacketNum;
                    mFlow.PacketTime = m.PacketTime;
                    mFlow.PacketTime_ms_ = m.PacketTime_ms_;
                    mFlow.sccp_dlr = m.sccp_dlr;
                    mFlow.sccp_slr = m.sccp_slr;
                    mFlow.tmsi = m.tmsi;
                    mFlow.opcdpcsccp = i.Key;
                    yield return mFlow;
                }
            }
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
                    //bc.BulkCopyTimeout=0;
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
 