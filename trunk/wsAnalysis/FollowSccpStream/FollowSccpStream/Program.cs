using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FollowSccpStream
{
    class Program
    {
        //private static string connString = "Data Source=192.168.1.9;Initial Catalog=sz_23B_20100920;Persist Security Info=True;User ID=weihp;Password=admin123456";
        private static string connString = "Data Source=.\\sqlexpress;Initial Catalog=mc_sz04a;Integrated Security=True";
        private static DataClasses1DataContext mydb = new DataClasses1DataContext(connString);
        static void Main(string[] args)
        {
            FollowSccpStream();
        }

        static void FollowSccpStream()
        {
            Dictionary<string, string> dConn = new Dictionary<string, string>();
            Dictionary<int?, string> dFlow = new Dictionary<int?, string>();
            var totalMessge = mydb.LA_update;//.Take(10000);

            for (int m = 0; m < 3; m++)
            {
                foreach (LA_update i in totalMessge)
                {
                    if (i.sccp_slr != null && i.sccp_dlr != null)
                    {
                        if (!dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr) || !dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr))
                        {
                            dConn.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                            dConn.Add(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                            dConn.Add(i.m3ua_dpc + i.m3ua_opc + i.sccp_slr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                            dConn.Add(i.m3ua_dpc + i.m3ua_opc + i.sccp_dlr, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                            dFlow.Add(i.PacketNum, i.m3ua_opc + i.m3ua_dpc + i.sccp_slr + i.sccp_dlr);
                        }
                    }
                    if (i.tmsi != null && i.sccp_slr == null && i.sccp_dlr == null)
                        if (dConn.ContainsKey(i.tmsi))
                            if (!dFlow.ContainsKey(i.PacketNum))
                                dFlow.Add(i.PacketNum, dConn[i.tmsi]);
                    if (i.sccp_slr != null)
                    {
                        if (dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_slr))
                            if (!dFlow.ContainsKey(i.PacketNum))
                            {
                                dFlow.Add(i.PacketNum, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_slr]);
                                if (i.tmsi != null)
                                    if (!dConn.ContainsKey(i.tmsi))
                                        dConn.Add(i.tmsi, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_slr]);
                            }
                    }
                    if (i.sccp_dlr != null)
                    {
                        if (dConn.ContainsKey(i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr))
                            if (!dFlow.ContainsKey(i.PacketNum))
                            {
                                dFlow.Add(i.PacketNum, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr]);
                                if (i.tmsi != null)
                                    if (!dConn.ContainsKey(i.tmsi))
                                        dConn.Add(i.tmsi, dConn[i.m3ua_opc + i.m3ua_dpc + i.sccp_dlr]);
                            }
                    }
                }
            }
            Console.WriteLine(dFlow.Count());
            Console.WriteLine(dConn.Count());
            var connLookup = dFlow.ToLookup(e => e.Value);
            //Dictionary<string, string> _dConn = new Dictionary<string, string>();
           
            foreach (var i in connLookup)
            {
                var nLst = i.Select(e => e.Key);
                var nFlow = totalMessge.Where(e => nLst.Contains(e.PacketNum));
                var mflow=string.Empty;
                foreach (var m in nFlow)
                {
                    if (m.ip_version_MsgType == "BSSMAP.Paging")
                    {
                        mflow += m.FileNum.Value.ToString() + m.PacketNum.Value.ToString() + m.PacketTime.Value.ToString() + m.ip_version_MsgType + m.imsi;
                    }
                    if (m.ip_version_MsgType == "DTAP RR.Paging Response")
                    {
                        mflow += m.FileNum.Value.ToString() + m.PacketNum.Value.ToString() + m.PacketTime.Value.ToString() + m.ip_version_MsgType + m.imsi;
                    }

                    //Console.WriteLine(m.ip_version_MsgType);

                }

                Console.WriteLine(mflow);
                //Console.WriteLine("\n");

                /*
                 * 
                var sccp = nFlow.Where(e => e.ip_version_MsgType == "sccp.Release Complete").FirstOrDefault();
                var nKey = i.Select(e => e.Value).FirstOrDefault();
                if (sccp != null)
                {
                    //此处开始做分析统计？yes?
                    foreach (var n in dConn)
                        if (n.Value == nKey)
                            _dConn.Add(n.Key, "");
                    foreach (var n in _dConn)
                        dConn.Remove(n.Key);
                    foreach (int n in nLst)
                        dFlow.Remove(n);
                }
                 * 
                 */

            }
            Console.WriteLine(dFlow.Count());
            Console.WriteLine(dConn.Count());
            Console.ReadKey();
        }
    }
}
