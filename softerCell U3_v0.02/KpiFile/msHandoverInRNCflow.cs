using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace softerCell_U3_v0._01
{
    class msHandoverInRNCflow
    {
        public static void mobile_handover_in_rnc_flow()
        {
            string strsql = null;
            streamMessagePool._ho_in_message ho_in_field = new streamMessagePool._ho_in_message();
            FieldInfo[] fields = ho_in_field.GetType().GetFields();
  
            strsql = kpiCommonClass.createListTable(fields);

            strsql = "create table " + streamMessagePool.dataTableName + "_ho_in" + " (" + strsql + " ) ";
            streamMessagePool.Acc.RunNoQuery(strsql);

            IEnumerable<string> list_n =
            from n in streamMessagePool.ml
            where n.message_info.IndexOf("id-RelocationResourceAllocation (3)") != -1 & n.message_gsm_a_imsi.Length > 1
            select n.message_frame;

            foreach (string i in list_n)//针对每1个类型的字段
            {
                int fr = Int32.Parse(i);

                //在此调用父窗口的进度条，采用托管的方式传递
                middleDelegate.DoSendPMessage(fr);
                Application.DoEvents();

                //建1条LAU流程消息
                streamMessagePool._ho_in_message ho_in = new streamMessagePool._ho_in_message();

                //找出现消息i的关联字段hs表
                HashSet<string> hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_fr(fr, false);

                //找出用户消息流程
                HashSet<string> hsEndMessage = new HashSet<string>();
                hsEndMessage.Add("id-RelocationComplete (13)");         //切换完成
                IEnumerable<streamMessagePool._stream_message> list_fr =
                    messageFlowSearch.convert_hsRelationString_listUserMessage(hs, fr, hsEndMessage);

                streamMessagePool._stream_message sm = list_fr.First();
                ho_in.id_RelocationResourceAllocation_r_message_frame = sm.message_frame;
                ho_in.id_RelocationResourceAllocation_r_message_time = sm.message_time;
                ho_in.id_RelocationResourceAllocation_r_imsi = sm.message_gsm_a_imsi;

                sm = list_fr.FirstOrDefault(n => n.message_frame != i
                    & n.message_info.IndexOf("id-RelocationResourceAllocation (3)") != -1);
                if (sm != null)
                {
                    ho_in.id_RelocationResourceAllocation_s_message_frame = sm.message_frame;
                    ho_in.id_RelocationResourceAllocation_s_message_time = sm.message_time;
                }

                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("id-RelocationDetect (12)") != -1);
                if (sm != null)
                {
                    ho_in.id_RelocationDetect_message_frame = sm.message_frame;
                    ho_in.id_RelocationDetect_message_time = sm.message_time;
                }

                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("id-RelocationComplete (13)") != -1);
                if (sm != null)
                {
                    ho_in.id_RelocationComplete_message_frame = sm.message_frame;
                    ho_in.id_RelocationComplete_message_time = sm.message_time;
                }

                strsql = kpiCommonClass.insertListTable(fields,ho_in);

                strsql = "insert into " + streamMessagePool.dataTableName + "_ho_in" + " values(" + strsql + ")";
                streamMessagePool.Acc.RunNoQuery(strsql);
            }
        }
    }
}
