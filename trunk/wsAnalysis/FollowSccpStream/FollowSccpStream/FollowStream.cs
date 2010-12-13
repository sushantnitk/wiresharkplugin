using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FollowSccpStream
{
    class FollowStream
    {
        private DataClasses1DataContext mydb = new DataClasses1DataContext(common.connString);
        //没有出现sccp对之前的单tmsi,slr,dlr回调字典
        private Dictionary<int?, string> dCallback = new Dictionary<int?, string>();
        //没有出现sccp对之前的imsi回调字典的补充
        private Dictionary<int?, string> _dCallback = new Dictionary<int?, string>();
        //删除没有出现sccp对之前的回调字典记录，避免重复，即一个记录只用一次
        private HashSet<int?> hRemovecall = new HashSet<int?>();
        //出现sccp对之后的只包含一项slr或者dlr的稀疏矩阵字典
        private Dictionary<string, string> dConn = new Dictionary<string, string>();
        //opc+dpc+slr+dlr索引字典，索引到包标签
        public Dictionary<int?, string> dFlow = new Dictionary<int?, string>();

        FlowStatistics fs = new FlowStatistics();

        public FollowStream(IEnumerable<LA_update> totalMessge)
        {
            //FollowSccpStream(totalMessge);
            common.messagelist = mydb.LA_update.ToDictionary(e => e.PacketNum);
            FollowSccpStream(common.messagelist);

        }
        //通过回调的方式获取 opc+dpc+slr+dlr字典值的关键字的集合
        //private void FollowSccpStream(IEnumerable<LA_update> totalMessge)
        private void FollowSccpStream(Dictionary<int?, LA_update> totalMessge)
        {
            //foreach (LA_update i in totalMessge)
            foreach (var dic in totalMessge)
            {
                var i = dic.Value;
                //common.messagelist.Add(i);
                //Console.WriteLine(i.PacketNum);
                //寻呼消息
                if (i.sccp_slr == null && i.sccp_dlr == null)
                {
                    //寻呼消息，包标签都需要进入队列
                    if (i.tmsi != null)
                    {
                        dCallback.Add(i.PacketNum, i.tmsi);
                        dFlow.Add(i.PacketNum, i.tmsi);
                    }
                    if (i.imsi != null)
                    {
                        _dCallback.Add(i.PacketNum, i.imsi);
                        if (!dFlow.ContainsKey(i.PacketNum))
                            dFlow.Add(i.PacketNum, i.imsi);
                    }
                }

                //寻呼响应
                if (i.sccp_slr != null && i.sccp_dlr == null)
                {
                    //查询稀疏矩阵
                    if (dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr))
                        //后续消息，Flow正常增加包标签
                        dFlow.Add(i.PacketNum, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_slr]);
                    else
                        //位置更新消息需要进入队列,开始记录包标签
                        dCallback.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr);

                    Dictionary<int?, string> d = new Dictionary<int?, string>();
                    foreach (var call in dCallback)
                        d.Add(call.Key, call.Value);
                    Dictionary<int?, string> _d = new Dictionary<int?, string>();
                    foreach (var call in _dCallback)
                        _d.Add(call.Key, call.Value);

                    //回调寻呼消息包标签 ，相当于把TMSI和IMSI替换成opc+dpc+slr
                    if (i.tmsi != null)
                        foreach (var call in d)
                            if (call.Value == i.tmsi)
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_slr;
                    if (i.imsi != null)
                        foreach (var call in _d)
                            if (call.Value == i.imsi)
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_slr;
                }

                //CC消息
                if (i.sccp_slr != null && i.sccp_dlr != null)
                {
                    //Flow正常增加包标签
                    dFlow.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);

                    //正常增加稀疏矩阵
                    if (!dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr))
                    {
                        dConn.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                        //回调位置更新包标签，已经替换了TMSI和IMSI的寻呼消息包标签
                        foreach (var call in dCallback)
                            if (call.Value == i.m3ua_opc + i.m3ua_dpc + i.sccp_slr)
                            {
                                if (!dFlow.ContainsKey(call.Key))
                                    dFlow.Add(call.Key, dConn[call.Value]);
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
                                    dFlow.Add(call.Key, dConn[call.Value]);
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
                                    dFlow.Add(call.Key, dConn[call.Value]);
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
                                    dFlow.Add(call.Key, dConn[call.Value]);
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
                        //后续消息，Flow正常增加包标签，此处增加包标签
                        dFlow.Add(i.PacketNum, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr]);
                    else
                        //开始记录包标签
                        dCallback.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr);

                    Dictionary<int?, string> d = new Dictionary<int?, string>();
                    foreach (var call in dCallback)
                        d.Add(call.Key, call.Value);
                    Dictionary<int?, string> _d = new Dictionary<int?, string>();
                    foreach (var call in _dCallback)
                        _d.Add(call.Key, call.Value);

                    //回调寻呼消息包标签
                    if (i.tmsi != null)
                        foreach (var call in d)
                            if (call.Value == i.tmsi)
                                //开始记录包标签
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr;
                    if (i.imsi != null)
                        foreach (var call in _d)
                            if (call.Value == i.imsi)
                                //开始记录包标签
                                dCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr;
                }

                //删除稀疏矩阵的主键
                if (i.ip_version_MsgType == "SCCP.Released")
                {
                    dConn.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr);
                    dConn.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr);
                    dConn.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr);
                    dConn.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr);
                    //此处做统计,通过多线程
                    FlowStatistics(i.PacketNum);
                    Console.WriteLine(i.PacketNum);
                }

                //删除回调记录的主键
                foreach (var k in hRemovecall)
                {
                    dCallback.Remove(k);
                    if (_dCallback.ContainsKey(k))
                        _dCallback.Remove(k);
                }
                //Thread.Sleep(1);
            }
            fs.Save();
            Console.WriteLine(dFlow.Count());
            Console.WriteLine(dConn.Count());
            Console.ReadKey();
        }

        private void FlowStatistics(int? packetnum)
        {
            var value = dFlow[packetnum];
            //Console.WriteLine(value);
            var connLookup = dFlow.ToLookup(e => e.Value);
            Task.Factory.StartNew(() => fs.FlowStatics(connLookup[value].Select(e => e.Key).ToList()));
            // if(Task.Factory.
            //Task.Factory.StartNew(() => DoComputation3());
            // fs.FlowStatics(a);
            //Console.WriteLine("packetnum");
            /* 
            var outer = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Outer task beginning.");

                var child = Task.Factory.StartNew(() =>
                {
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Detached task completed.");
                });

            });
            outer.Wait();
            Console.WriteLine("Outer task completed.");
            Output:
                Outer task beginning.
                Outer task completed.
                Detached task completed.
             */
        }
    }
}
