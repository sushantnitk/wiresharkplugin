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
        private Dictionary<int?, string> dListCallback = new Dictionary<int?, string>();
        //没有出现sccp对之前的imsi回调字典的补充
        private Dictionary<int?, string> dListCallbackClone = new Dictionary<int?, string>();
        //删除没有出现sccp对之前的回调字典记录，避免重复，即一个记录只用一次
        private HashSet<int?> hRemoveCall = new HashSet<int?>();
        //出现sccp对之后的只包含一项slr或者dlr的稀疏矩阵字典
        private Dictionary<string, string> dListConnetion = new Dictionary<string, string>();
        //opc+dpc+slr+dlr索引字典，索引到包标签
        public Dictionary<int?, string> dListFlow = new Dictionary<int?, string>();
        //实例化一个字典，以便计算哪些消息还没有做关联
        private HashSet<AMessageDetail> hListMessage = new HashSet<AMessageDetail>();
        //实例化一个统计方法，在消息流遍历的过程中进行统计
        public FlowStatistics FlowStatistics;
        public FollowStream()
        {
            FlowStatistics = new FlowStatistics();
        }
        public FollowStream(List<string> message)
        {
            FlowStatistics = new FlowStatistics(message);
            //FollowSccpStream(totalMessge);.OrderBy(e=>e.PacketNum)

            //FollowSccpStream(totalMessge);
        }
        //通过回调的方式获取 opc+dpc+slr+dlr字典值的关键字的集合
        // public void FollowSccpStream(IEnumerable<AMessageDetail> totalMessge)
        // public void FollowSccpStream(Dictionary<int?, AMessageDetail> totalMessge)
        public void FollowSccpStream(AMessageDetail i)
        {
            hListMessage.Add(i);
            RecordPaging(i);
            RecordPagingResponse(i);
            RecordConnetionConfirm(i);
            RecordContinueMessage(i);
            RecordSccpRelease(i);
            ReleaseCallback(i);
            //foreach (AMessageDetail i in totalMessge)
            //var total = totalMessge.OrderBy(e => e.Key);
            //foreach (var dic in total)
            //{
            //var i = dic.Value;
            //common.messagelist.Add(i);
            //Console.WriteLine(i.PacketNum);
        }
        private void RecordPaging(AMessageDetail i)
        {
            //寻呼消息
            if (i.sccp_slr == null && i.sccp_dlr == null)
            {
                //寻呼消息，包标签都需要进入队列
                if (i.tmsi != null)
                {
                    dListCallback.Add(i.PacketNum, i.tmsi);
                    dListFlow.Add(i.PacketNum, i.tmsi);
                }
                if (i.imsi != null)
                {
                    dListCallbackClone.Add(i.PacketNum, i.imsi);
                    if (!dListFlow.ContainsKey(i.PacketNum))
                        dListFlow.Add(i.PacketNum, i.imsi);
                }
            }
        }

        public void RecordPagingResponse(AMessageDetail i)
        {

            //寻呼响应
            if (i.sccp_slr != null && i.sccp_dlr == null)
            {
                //查询稀疏矩阵
                if (dListConnetion.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr))
                    //后续消息，Flow正常增加包标签
                    dListFlow.Add(i.PacketNum, dListConnetion[i.m3ua_opc + i.m3ua_dpc + i.sccp_slr]);
                else
                    //位置更新消息需要进入队列,开始记录包标签
                    dListCallback.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr);

                Dictionary<int?, string> d = new Dictionary<int?, string>();
                foreach (var call in dListCallback)
                    d.Add(call.Key, call.Value);
                Dictionary<int?, string> _d = new Dictionary<int?, string>();
                foreach (var call in dListCallbackClone)
                    _d.Add(call.Key, call.Value);

                //回调寻呼消息包标签 ，相当于把TMSI和IMSI替换成opc+dpc+slr
                if (i.tmsi != null)
                    foreach (var call in d)
                        if (call.Value == i.tmsi)
                            dListCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_slr;
                if (i.imsi != null)
                    foreach (var call in _d)
                        if (call.Value == i.imsi)
                            dListCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_slr;
            }
        }

        public void RecordConnetionConfirm(AMessageDetail i)
        {
            //CC消息
            if (i.sccp_slr != null && i.sccp_dlr != null)
            {
                //Flow正常增加包标签
                dListFlow.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                //正常增加稀疏矩阵
                if (!dListConnetion.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr))
                {
                    dListConnetion.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                    //回调位置更新包标签，已经替换了TMSI和IMSI的寻呼消息包标签
                    foreach (var call in dListCallback)
                        if (call.Value == i.m3ua_opc + i.m3ua_dpc + i.sccp_slr)
                        {
                            if (!dListFlow.ContainsKey(call.Key))
                                dListFlow.Add(call.Key, dListConnetion[call.Value]);
                            else
                                dListFlow[call.Key] = dListConnetion[call.Value];
                            hRemoveCall.Add(call.Key);
                        }
                }
                if (!dListConnetion.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr))
                {
                    dListConnetion.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                    //回调位置更新包标签
                    foreach (var call in dListCallback)
                        if (call.Value == i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr)
                        {
                            if (!dListFlow.ContainsKey(call.Key))
                                dListFlow.Add(call.Key, dListConnetion[call.Value]);
                            else
                                dListFlow[call.Key] = dListConnetion[call.Value];
                            hRemoveCall.Add(call.Key);
                        }
                }
                if (!dListConnetion.ContainsKey(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr))
                {
                    dListConnetion.Add(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                    //回调位置更新包标签
                    foreach (var call in dListCallback)
                        if (call.Value == i.m3ua_dpc + i.m3ua_opc + i.sccp_slr)
                        {
                            if (!dListFlow.ContainsKey(call.Key))
                                dListFlow.Add(call.Key, dListConnetion[call.Value]);
                            else
                                dListFlow[call.Key] = dListConnetion[call.Value];
                            hRemoveCall.Add(call.Key);
                        }
                }
                if (!dListConnetion.ContainsKey(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr))
                {
                    dListConnetion.Add(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                    //回调位置更新包标签
                    foreach (var call in dListCallback)
                        if (call.Value == i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr)
                        {
                            if (!dListFlow.ContainsKey(call.Key))
                                dListFlow.Add(call.Key, dListConnetion[call.Value]);
                            else
                                dListFlow[call.Key] = dListConnetion[call.Value];
                            hRemoveCall.Add(call.Key);
                        }
                }
            }
        }
        public void RecordContinueMessage(AMessageDetail i)
        {
            //后续消息
            if (i.sccp_slr == null && i.sccp_dlr != null)
            {
                //查询稀疏矩阵
                if (dListConnetion.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr))
                    //后续消息，Flow正常增加包标签，此处增加包标签
                    dListFlow.Add(i.PacketNum, dListConnetion[i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr]);
                else
                    //开始记录包标签
                    dListCallback.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr);

                Dictionary<int?, string> d = new Dictionary<int?, string>();
                foreach (var call in dListCallback)
                    d.Add(call.Key, call.Value);
                Dictionary<int?, string> _d = new Dictionary<int?, string>();
                foreach (var call in dListCallbackClone)
                    _d.Add(call.Key, call.Value);

                //回调寻呼消息包标签
                if (i.tmsi != null)
                    foreach (var call in d)
                        if (call.Value == i.tmsi)
                            //开始记录包标签
                            dListCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr;
                if (i.imsi != null)
                    foreach (var call in _d)
                        if (call.Value == i.imsi)
                            //开始记录包标签
                            dListCallback[call.Key] = i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr;
            }
        }
        public void RecordSccpRelease(AMessageDetail i)
        {
            //删除稀疏矩阵的主键
            if (i.message_type == "RLC" || i.message_type == "RLSD" || i.message_type.IndexOf("Release")!=-1)
            {
                dListConnetion.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr);
                dListConnetion.Remove(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr);
                dListConnetion.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr);
                dListConnetion.Remove(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr);
                //此处做统计,通过多线程
                CountFlow(i.PacketNum);
                Console.WriteLine(i.PacketNum);;
            }
        }

        public void ReleaseCallback(AMessageDetail i)
        {

            //删除回调记录的主键
            foreach (var k in hRemoveCall)
            {
                dListCallback.Remove(k);
                if (dListCallbackClone.ContainsKey(k))
                    dListCallbackClone.Remove(k);
            }

        }
        private void FlowConsoleWrite(int? packetnum)
        {
            if (packetnum % 5000 == 0)
            {
                GC.Collect();
                GC.Collect();
            }
            var value = dListFlow[packetnum];
            var connLookup = dListFlow.ToLookup(e => e.Value);
            Task.Factory.StartNew(() => FlowStatistics.WriteFlowConsole(connLookup[value].Select(e => e.Key).ToList(), value));
        }

        private void ReleaseBuffer(int? packetnum)
        {
            if (packetnum % 10000 < 10)
                GC.Collect();
        }

        private void CountFlow(int? PacketNumber)
        {
            //定时清理内存
            ReleaseBuffer(PacketNumber);
            //从dFlow和mList获取消息列表
            var value = dListFlow[PacketNumber];
            var connLookup = dListFlow.ToLookup(e => e.Value);
            var packetNumList = connLookup[value].Select(e => e.Key).ToList();
            var asccp = hListMessage.Where(e => packetNumList.Contains(e.PacketNum)).ToDictionary(e => e.PacketNum);
            //Console.WriteLine(mList.Count);
            //从mList删除已经计算过的消息列表
            foreach (var p in asccp)
                hListMessage.Remove(p.Value);
            //Console.WriteLine(mList.Count);
            //开始进行统计
            //Task.Factory.StartNew(() => FlowStatistics.CountFlow(asccp));
            Task.Factory.StartNew(() => FlowStatistics.CountPcapFlow(asccp));
        }
    }
}
