using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace softerCell_U3_v0._01
{
    class msIuReleaseRequestFlow
    {
        public static void mobile_iu_release_request_flow()
        {
            string strsql = null;
            streamMessagePool._iu_release_request_message iu_rr_field = new streamMessagePool._iu_release_request_message();
            FieldInfo[] fields = iu_rr_field.GetType().GetFields();

            strsql = kpiCommonClass.createListTable(fields);

            strsql = "create table " + streamMessagePool.dataTableName + "_iu_rr" + " (" + strsql + " ) ";
            streamMessagePool.Acc.RunNoQuery(strsql);


            IEnumerable<string> list_n =
            from n in streamMessagePool.ml
            where n.message_info.IndexOf("id-Iu-ReleaseRequest (11)") != -1
            select n.message_frame;

            foreach (string i in list_n)//针对每1个类型的字段
            {
                int fr = Int32.Parse(i);

                //在此调用父窗口的进度条，采用托管的方式传递
                middleDelegate.DoSendPMessage(fr);
                Application.DoEvents();

                streamMessagePool._iu_release_request_message iu_rr = new streamMessagePool._iu_release_request_message();
                streamMessagePool._stream_message sm = streamMessagePool.ml.ElementAt(fr);
                iu_rr.iu_release_request_message_frame = sm.message_frame;
                iu_rr.iu_release_request_message_time = sm.message_time;
                iu_rr.iu_release_request_message_radioNetwork = sm.message_radioNetwork;
                iu_rr.iu_release_request_message_Cause = sm.message_Cause;

                //找出现消息i的关联字段hs表
                //考虑用这个方法
                //IEnumerable<string> strings = ...;// C# 3 and .NET 3.5:string joined = string.Join(",", strings.ToArray());

                HashSet<string> hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_fr(fr, true);

                IEnumerable<streamMessagePool._stream_message> list_fr =
                from n in streamMessagePool.ml
                where hs.Contains(n.message_sccp_slr + n.message_source)
                    | hs.Contains(n.message_sccp_dlr + n.message_destination)
                    | hs.Contains(n.message_gsm_a_imsi)
                    | hs.Contains(n.CIC + n.message_source + n.message_destination)
                select n;

                var query1 =
                        from n in list_fr
                        where n.message_gsm_a_imsi != null
                        select n.message_gsm_a_imsi;
                if (query1.Any())
                    iu_rr.iu_release_request_imsi = query1.FirstOrDefault(n => n.Length > 1);

                var query2 =
                        from n in list_fr
                        where n.message_rnc_id != null
                        select n.message_rnc_id;
                if (query2.Any())
                    iu_rr.iu_release_request_rnc_id = query2.FirstOrDefault(n => n.Length > 1);

                var query3 =
                     from n in list_fr
                     where n.message_id_num != null & n.message_id_type != null
                     select n
                         into m
                         where m.message_id_type.IndexOf("IMEI") != -1
                         select m.message_id_num;
                if (query3.Any())
                    iu_rr.iu_release_request_imei = query3.FirstOrDefault(n => n.Length > 1);


                strsql = kpiCommonClass.insertListTable(fields,iu_rr );

                strsql = "insert into " + streamMessagePool.dataTableName + "_iu_rr" + " values(" + strsql + ")";
                streamMessagePool.Acc.RunNoQuery(strsql);
            }
        }
    }
}
