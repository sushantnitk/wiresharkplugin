using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FollowSccpStream
{
    class FlowStatistics
    {
        System.IO.StreamWriter StatisticsWriter= new System.IO.StreamWriter(@"f:\log.txt", false);
        System.IO.StreamWriter FlowListWriter = new System.IO.StreamWriter(@"f:\log1.txt", false);
        //DataClasses1DataContext mydb = new DataClasses1DataContext(common.connString);
        Dictionary<string, int> dMessageList = new Dictionary<string, int>();
        Dictionary<string, int>[] dFlowMessageList = new Dictionary<string, int>[6];
        //List<string> startmessage = new List<string>();
        Dictionary<string, Dictionary<string, int>> dFlowStartMessage = new Dictionary<string, Dictionary<string, int>>();
        Dictionary<string, Dictionary<string, int>> dFlowStartMessageClone = new Dictionary<string, Dictionary<string, int>>();
        List<string> MessageList = new List<string>();
        bool MessageExit = false;
        //session编号
        private int StatIndex = 0;
        //ILookup<int?, LA_update> messagelist;
        //Tuple<string, Dictionary<string, int>> statics;

        public FlowStatistics()
        {
            InitMessageDic();
            InitFlowCollection();
            CloneMessageDic();
        }
        public FlowStatistics(List<string> message)
        {
            this.MessageList = message;
            InitMessageDic();
            InitFlowCollection();
            CloneMessageDic();
        }

        private void InitWrite()
        {
            //获取消息列表字典
            foreach (string m in MessageList)
            {
                dMessageList.Add(m, 0);
                Console.Write(m + "--------------------");
                Console.WriteLine(0);

                StatisticsWriter.Write(m + "--------------------");
                StatisticsWriter.WriteLine(0);

            }
            StatisticsWriter.Flush();
        }

        private void InitFlowCollection()
        {
            //初始化统计表字典
            //for (int i = 0; i < startmessage.Count(); i++)
            //    newDic[i] = new Dictionary<string, int>();

            dFlowStartMessage.Add("DTAP MM.CM Service Request", dFlowMessageList[0]);
            dFlowStartMessage.Add("BSSMAP.Paging", dFlowMessageList[0]);
            dFlowStartMessage.Add("BSSMAP.Handover Request", dFlowMessageList[0]);
            dFlowStartMessage.Add("BSSMAP.Handover Performed", dFlowMessageList[0]);
            dFlowStartMessage.Add("BSSMAP.Handover Required", dFlowMessageList[0]);
            dFlowStartMessage.Add("DTAP MM.Location Updating Request", dFlowMessageList[0]);
            //statics.Item1 = "DTAP MM.Location Updating Request";
            /*
            NGN指标定义方法
            gsm_a.cell_lac || gsm_a.cell_lac_target-------WHandoverRequireds
            Handover Performed---------------------WHandoverPerforms
            Handover Request-----------WHandoverRequests
            gsm_a.cell_lac || gsm_a.cell_lac_target----------WClearRequests
            Location Updating Request----------WLocationUpdates
            Paging------------WPagings
            gsm_a.cell_lac-----WTchAssignments
            */
        }

        //列出所有出现的消息->myDic
        private void InitMessageDic()
        {
            //获取消息列表字典
            foreach (string m in MessageList)
            {
                dMessageList.Add(m, 0);
                Console.Write(m + "--------------------");
                Console.WriteLine(0);

                StatisticsWriter.Write(m + "--------------------");
                StatisticsWriter.WriteLine(0);

            }
            StatisticsWriter.Flush();
        }

        //把myDic的做n次Clone
        private void CloneMessageDic()
        {
            //给初始化字典赋值
            int i = 0;
            foreach (var s in dFlowStartMessage)
            {
                dFlowMessageList[i] = new Dictionary<string, int>();
                foreach (KeyValuePair<string, int> pair in dMessageList)
                    dFlowMessageList[i].Add(pair.Key, 0);
                i++;
            }

            CloneStartMessage();

            int index = 0;
            foreach (var m in dFlowStartMessageClone)
            {
                dFlowStartMessage[m.Key] = dFlowMessageList[index];
                index++;
            }
        }

        //把startmessage的内容做1次Clone
        private void CloneStartMessage()
        {
            foreach (var m in dFlowStartMessage)
                dFlowStartMessageClone.Add(m.Key, dMessageList);
        }

        public void Save()
        {
            foreach (var m in dFlowStartMessage)
            {
                Console.Write("**************"+m.Key + "**************");
                StatisticsWriter.Write("**************"+m.Key + "**************");
                Console.WriteLine("**************");
                StatisticsWriter.WriteLine("**************");
                foreach (var n in m.Value)
                {
                    Console.Write(n.Key + "--------------------");
                    Console.WriteLine(n.Value);
                    StatisticsWriter.Write(n.Key + "--------------------");
                    StatisticsWriter.WriteLine(n.Value);
                }
            }
            StatisticsWriter.Flush();
            StatisticsWriter.Close();
            FlowListWriter.Close();
        }

        public void CountFlow(Dictionary<int?, LA_update> asccp)
        {
            foreach (var start in dFlowStartMessageClone)
            {
                //消息置位
                MessageExit = false;
                //消息顺序排序
                foreach (var a in asccp.OrderBy(e => e.Key))
                {
                    var messageb = a.Value.ip_version_MsgType;
                    if (messageb == start.Key)
                    {
                        MessageExit = true;
                    }
                    if (MessageExit == true)
                        if (dMessageList.ContainsKey(messageb))
                        {
                            var c = dFlowStartMessage[start.Key];
                            c[messageb] = c[messageb] + 1;
                        }
                    //flow写入数据库
                    if (MessageExit == true)
                    {
                        FlowListWriter.Write(StatIndex);
                        FlowListWriter.Write(",");
                        FlowListWriter.Write(a.Value.PacketNum);
                        FlowListWriter.Write(",");
                        FlowListWriter.Write(a.Value.PacketTime);
                        FlowListWriter.Write(",");
                        FlowListWriter.Write(a.Value.ip_version_MsgType);
                        FlowListWriter.Write("\n");
                        FlowListWriter.Flush();
                    }
                }
            }
            StatIndex++;
        }

        public void CountPcapFlow(Dictionary<int?, LA_update> asccp)
        {
            foreach (var a in asccp.OrderBy(e => e.Key))
            {
                FlowListWriter.Write(StatIndex);
                FlowListWriter.Write(",");
                FlowListWriter.Write(a.Value.PacketNum);
                FlowListWriter.Write(",");
                FlowListWriter.Write(a.Value.PacketTime);
                FlowListWriter.Write(",");
                FlowListWriter.Write(a.Value.ip_version_MsgType);
                FlowListWriter.Write("\n");
                FlowListWriter.Flush();
            }
            StatIndex++;
        }

        public void WriteFlowConsole(List<int?> a, string opcdpcsccp)
        {
            var messagefirst = a.OrderBy(e => e.Value);
            foreach (var b in messagefirst)
            {
                //var messageb = mydb.LA_update.Where(e => e.PacketNum == b).FirstOrDefault();
                //var messageb = common.messagelist[b];
                var messageb = CommonFunction.MessageList.Where(e => e.PacketNum == b).FirstOrDefault();
                StatisticsWriter.Write(opcdpcsccp); StatisticsWriter.Write(",");
                StatisticsWriter.Write(messageb.PacketNum); StatisticsWriter.Write(",");
                StatisticsWriter.Write(messageb.PacketTime); StatisticsWriter.Write(",");
                StatisticsWriter.Write(messageb.ip_version_MsgType + "\n");
            }
            StatisticsWriter.Flush();

        }
    }
}
