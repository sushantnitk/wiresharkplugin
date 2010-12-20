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
        //private DataClasses1DataContext mydb = new DataClasses1DataContext(common.connString);
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
        //实例化一个字典，以便计算哪些消息还没有做关联
        private HashSet<LA_update> mList = new HashSet<LA_update>();
        //实例化一个统计方法，在消息流遍历的过程中进行统计
        public FlowStatistics flowstat;
        public FollowStream(List<string> message)
        {
            flowstat = new FlowStatistics(message);
            //FollowSccpStream(totalMessge);.OrderBy(e=>e.PacketNum)

            //FollowSccpStream(totalMessge);
        }
        //通过回调的方式获取 opc+dpc+slr+dlr字典值的关键字的集合
        // public void FollowSccpStream(IEnumerable<LA_update> totalMessge)
        // public void FollowSccpStream(Dictionary<int?, LA_update> totalMessge)
        public void FollowSccpStream(LA_update i)
        {
            mList.Add(i);
            paging(i);
            pagingresponse(i);
            connetionconfirm(i);
            continuemessage(i);
            sccprelease(i);
            callbackrelease(i);
            //foreach (LA_update i in totalMessge)
            //var total = totalMessge.OrderBy(e => e.Key);
            //foreach (var dic in total)
            //{
            //var i = dic.Value;
            //common.messagelist.Add(i);
            //Console.WriteLine(i.PacketNum);
        }
        private void paging(LA_update i)
        {
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
        }

        public void pagingresponse(LA_update i)
        {

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
        }

        public void connetionconfirm(LA_update i)
        {
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
        }
        public void continuemessage(LA_update i)
        {
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
        }
        public void sccprelease(LA_update i)
        {
            //删除稀疏矩阵的主键
            if (i.ip_version_MsgType.IndexOf("SCCP.Release") != -1)
            {
                dConn.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr);
                dConn.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr);
                dConn.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr);
                dConn.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr);
                //此处做统计,通过多线程
                FlowStatistics(i.PacketNum);
                Console.WriteLine(i.PacketNum);
                //Task.Factory.StartNew(()=>fs.FlowConsoleWrite
                //FlowConsoleWrite(i.PacketNum);
                //var value = dFlow[i.PacketNum];
                //var connLookup = dFlow.ToLookup(e => e.Value);
                //Task.Factory.StartNew(() => fs.FlowConsoleWrite(connLookup[value].Select(e => e.Key).ToList(),value));
                // fs.FlowConsoleWrite(connLookup[value].Select(e => e.Key).ToList(), value);
            }
        }

        public void callbackrelease(LA_update i)
        {

            //删除回调记录的主键
            foreach (var k in hRemovecall)
            {
                dCallback.Remove(k);
                if (_dCallback.ContainsKey(k))
                    _dCallback.Remove(k);
            }
            //Thread.Sleep(1);
            //}
            //fs.Save();
            //结束以后则需要把所有的消息都过一遍
            //Console.WriteLine(dFlow.Count());
            //Console.WriteLine(dConn.Count());
            //Console.ReadKey();
        }
        private void FlowConsoleWrite(int? packetnum)
        {
            if (packetnum % 5000 == 0)
            {
                GC.Collect();
                GC.Collect();
            }
            var value = dFlow[packetnum];
            var connLookup = dFlow.ToLookup(e => e.Value);
            Task.Factory.StartNew(() => flowstat.FlowConsoleWrite(connLookup[value].Select(e => e.Key).ToList(), value));
        }

        private void Dispose(int? packetnum)
        {
            if (packetnum % 10000 == 0)
                GC.Collect();
        }

        private void FlowStatistics(int? packetnum)
        {
            //定时清理内存
            Dispose(packetnum);
            //从dFlow和mList获取消息列表
            var value = dFlow[packetnum];
            var connLookup = dFlow.ToLookup(e => e.Value);
            var packetNumList = connLookup[value].Select(e => e.Key).ToList();
            var asccp = mList.Where(e => packetNumList.Contains(e.PacketNum)).ToDictionary(e => e.PacketNum);
            //Console.WriteLine(mList.Count);
            //从mList删除已经计算过的消息列表
            foreach (var p in asccp)
                mList.Remove(p.Value);
            //Console.WriteLine(mList.Count);
            //开始进行统计
            Task.Factory.StartNew(() => flowstat.FlowStatics(asccp));
        }
    }
}
