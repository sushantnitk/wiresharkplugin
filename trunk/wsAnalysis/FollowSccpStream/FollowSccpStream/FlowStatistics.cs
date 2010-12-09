using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FollowSccpStream
{
    class FlowStatistics
    {
        public static void FlowStatics()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", false);
            DataClasses1DataContext mydb = new DataClasses1DataContext(common.connString);
            var message = mydb.LA_update1.Select(e => e.ip_version_MsgType).Distinct();
            Dictionary<string, int> myDic = new Dictionary<string, int>();
            foreach (string m in message)
            {
                myDic.Add(m, 0);
                Console.Write(m + "--------------------");
                Console.WriteLine(0);

                sw.Write(m + "--------------------");
                sw.WriteLine(0);

            }
            sw.Flush();

            List<string> startmessage = new List<string>();
            startmessage.Add("DTAP MM.Location Updating Request");
            startmessage.Add("DTAP MM.CM Service Request");
            startmessage.Add("DTAP RR.Paging Response");
            startmessage.Add("BSSMAP.Handover Request");


            foreach (var start in startmessage)
            {
                Dictionary<string, int> newDic = new Dictionary<string, int>();
                foreach (KeyValuePair<string, int> pair in myDic)
                    newDic.Add(pair.Key, 0);

                var a = from p in mydb.LA_update1
                        where p.ip_version_MsgType == start
                        select p.opcdpcsccp;

                foreach (var b in a)
                {
                    foreach (KeyValuePair<string, int> kvp in myDic)
                    {
                        var c = mydb.LA_update1.Where(e => e.opcdpcsccp == b).Where(e => e.ip_version_MsgType == kvp.Key).FirstOrDefault();
                        if (c != null)
                            newDic[kvp.Key] = newDic[kvp.Key] + 1;
                    }
                }

                foreach (var m in newDic.OrderByDescending(e => e.Value))
                {
                    Console.Write(m.Key+"--------------------");
                    Console.WriteLine(m.Value);

                    sw.Write(m.Key + "--------------------");
                    sw.WriteLine(m.Value);
                }

                sw.Flush();
                
            }
            sw.Close();
        }
    }
}
