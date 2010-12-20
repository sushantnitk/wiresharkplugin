using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FollowSccpStream
{
    class FlowStatistics
    {
        System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", false);
        //DataClasses1DataContext mydb = new DataClasses1DataContext(common.connString);
        Dictionary<string, int> myDic = new Dictionary<string, int>();
        Dictionary<string, int>[] newDic = new Dictionary<string, int>[6];
        //List<string> startmessage = new List<string>();
        Dictionary<string, Dictionary<string, int>> startmessage = new Dictionary<string, Dictionary<string, int>>();
        Dictionary<string, Dictionary<string, int>> _startmessage = new Dictionary<string, Dictionary<string, int>>();
        List<string> message = new List<string>();
        bool messageexit = false;
        //ILookup<int?, LA_update> messagelist;
        //Tuple<string, Dictionary<string, int>> statics;

        public FlowStatistics(List<string> message)
        {
            this.message = message;
            initMessageDic();
            initFlowCollection();
            mydicClone();
        }

        private void initWrite()
        {
            //获取消息列表字典
            foreach (string m in message)
            {
                myDic.Add(m, 0);
                Console.Write(m + "--------------------");
                Console.WriteLine(0);

                sw.Write(m + "--------------------");
                sw.WriteLine(0);

            }
            sw.Flush();
        }

        private void initFlowCollection()
        {
            //初始化统计表字典
            //for (int i = 0; i < startmessage.Count(); i++)
            //    newDic[i] = new Dictionary<string, int>();

            startmessage.Add("DTAP MM.CM Service Request", newDic[0]);
            startmessage.Add("BSSMAP.Paging", newDic[0]);
            startmessage.Add("BSSMAP.Handover Request", newDic[0]);
            startmessage.Add("BSSMAP.Handover Performed", newDic[0]);
            startmessage.Add("BSSMAP.Handover Required", newDic[0]);
            startmessage.Add("DTAP MM.Location Updating Request", newDic[0]);
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
        private void initMessageDic()
        {
            //获取消息列表字典
            foreach (string m in message)
            {
                myDic.Add(m, 0);
                Console.Write(m + "--------------------");
                Console.WriteLine(0);

                sw.Write(m + "--------------------");
                sw.WriteLine(0);

            }
            sw.Flush();
        }

        //把myDic的做n次Clone
        private void mydicClone()
        {
            //给初始化字典赋值
            int i = 0;
            foreach (var s in startmessage)
            {
                newDic[i] = new Dictionary<string, int>();
                foreach (KeyValuePair<string, int> pair in myDic)
                    newDic[i].Add(pair.Key, 0);
                i++;
            }

            startmessageClone();

            int index = 0;
            foreach (var m in _startmessage)
            {
                startmessage[m.Key] = newDic[index];
                index++;
            }
        }

        //把startmessage的内容做1次Clone
        private void startmessageClone()
        {
            foreach (var m in startmessage)
                _startmessage.Add(m.Key, myDic);
        }

        public void Save()
        {
            foreach (var m in startmessage)
            {
                Console.Write("**************"+m.Key + "**************");
                sw.Write("**************"+m.Key + "**************");
                Console.WriteLine("**************");
                sw.WriteLine("**************");
                foreach (var n in m.Value)
                {
                    Console.Write(n.Key + "--------------------");
                    Console.WriteLine(n.Value);
                    sw.Write(n.Key + "--------------------");
                    sw.WriteLine(n.Value);
                }
            }
            sw.Flush();
            sw.Close();
        }

        public void FlowStatics(Dictionary<int?,LA_update> asccp)
        {          
           // asccp = asccp.OrderBy(e => e.Key);
            foreach (var start in _startmessage)
            {
                messageexit = false;
                foreach (var a in asccp)
                {
                    //var messageb = mydb.LA_update.Where(e => e.PacketNum == b).Select(e => e.ip_version_MsgType).FirstOrDefault();
                    //var messageb = common.messagelist[b].ip_version_MsgType;
                    //var messageb = common.messagelist.Where(e => e.PacketNum == b).Select(e => e.ip_version_MsgType).FirstOrDefault();
                    var messageb = a.Value.ip_version_MsgType;
                    if (messageb == start.Key)
                    {
                        messageexit = true;
                    }
                    if (messageexit == true)
                        foreach (KeyValuePair<string, int> kvp in myDic)
                        {
                            if (messageb == kvp.Key)
                            {
                                var c = startmessage[start.Key];
                                c[kvp.Key] = c[kvp.Key] + 1;
                                //newDic[kvp.Key] = newDic[kvp.Key] + 1;
                            }
                        }
                }
            }
        }
        public void FlowConsoleWrite(List<int?> a, string opcdpcsccp)
        {
            var messagefirst = a.OrderBy(e => e.Value);
            foreach (var b in messagefirst)
            {
                //var messageb = mydb.LA_update.Where(e => e.PacketNum == b).FirstOrDefault();
                //var messageb = common.messagelist[b];
                var messageb = common.messagelist.Where(e => e.PacketNum == b).FirstOrDefault();
                sw.Write(opcdpcsccp); sw.Write(",");
                sw.Write(messageb.PacketNum); sw.Write(",");
                sw.Write(messageb.PacketTime); sw.Write(",");
                sw.Write(messageb.ip_version_MsgType + "\n");
            }
            sw.Flush();

        }
    }
}
