using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace softerCell_U3_v0._01
{
    class msLocationUpdateFlow
    {
        public static void mobile_location_update_flow()
        {
            string strsql = null;
            streamMessagePool._lau_message _lau = new streamMessagePool._lau_message();
            FieldInfo[] fields = _lau.GetType().GetFields();

            strsql = kpiCommonClass.createListTable(fields);

            strsql = "create table " + streamMessagePool.dataTableName + "_lau" + " (" + strsql + " ) ";
            streamMessagePool.Acc.RunNoQuery(strsql);

            IEnumerable<string> list_n =
            from n in streamMessagePool.ml
            where n.message_info.IndexOf("Location Updating Request (0x08)") != -1
            select n.message_frame;

            foreach (string i in list_n)//针对每1个类型的字段
            {
                int fr = Int32.Parse(i);

                //在此调用父窗口的进度条，采用托管的方式传递
                middleDelegate.DoSendPMessage(fr);
                Application.DoEvents();

                //建1条LAU流程消息
                streamMessagePool._lau_message lau = new streamMessagePool._lau_message();

                //找出现消息i的关联字段hs表
                HashSet<string> hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_fr(fr, false);

                //找出用户消息流程
                HashSet<string> hsEndMessage = new HashSet<string>();
                hsEndMessage.Add("Release Complete (0x05)");            //位置更新的SCCP连接释放
                IEnumerable<streamMessagePool._stream_message> list_fr =
                    messageFlowSearch.convert_hsRelationString_listUserMessage(hs, fr, hsEndMessage);

                streamMessagePool._stream_message sm = list_fr.First();
                lau.Location_Updating_Request_message_frame = sm.message_frame;
                lau.Location_Updating_Request_message_time = sm.message_time;
                lau.Location_Updating_Request_message_rnc_id = sm.message_rnc_id;
                lau.Location_Updating_Request_message_lac = sm.message_lac;
                lau.Location_Updating_Request_message_gsm_a_tmsi = sm.message_gsm_a_tmsi;

                sm = list_fr.FirstOrDefault(n =>
                 n.message_info.IndexOf("Connection Confirm (0x02)") != -1);
                if (sm != null)
                {
                    lau.CC_message_frame = sm.message_frame;
                    lau.CC_message_time = sm.message_time;
                }

                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Identity Request (0x18)") != -1
                  & n.message_id_type.IndexOf("IMSI") != -1);
                if (sm != null)
                {
                    lau.Identity_Request_IMSI_message_frame = sm.message_frame;
                    lau.Identity_Request_IMSI_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Identity Response (0x19)") != -1
                    & n.message_id_type.IndexOf("IMSI") != -1);
                if (sm != null)
                {
                    lau.Identity_Response_IMSI_message_frame = sm.message_frame;
                    lau.Identity_Response_IMSI_message_time = sm.message_time;
                    lau.Identity_Response_IMSI_message_id_type = sm.message_id_type;
                    lau.Identity_Response_IMSI_message_id_num = sm.message_id_num;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("id-CommonID (15)") != -1);
                if (sm != null)
                {
                    lau.id_CommonID_message_frame = sm.message_frame;
                    lau.id_CommonID_message_time = sm.message_time;
                    lau.id_CommonID_message_imsi = sm.message_gsm_a_imsi;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Authentication Request (0x12)") != -1);
                if (sm != null)
                {
                    lau.Authentication_Request_message_frame = sm.message_frame;
                    lau.Authentication_Request_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Authentication Response (0x14)") != -1);
                if (sm != null)
                {
                    lau.Authentication_Response_message_frame = sm.message_frame;
                    lau.Authentication_Response_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Identity Request (0x18)") != -1
                    & n.message_id_type.IndexOf("IMEI") != -1);
                if (sm != null)
                {
                    lau.Identity_Request_IMEI_message_frame = sm.message_frame;
                    lau.Identity_Request_IMEI_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Identity Response (0x19)") != -1
                    & n.message_id_type.IndexOf("IMEI") != -1);
                if (sm != null)
                {
                    lau.Identity_Response_IMEI_message_frame = sm.message_frame;
                    lau.Identity_Response_IMEI_message_time = sm.message_time;
                    lau.Identity_Response_IMEI_message_id_type = sm.message_id_type;
                    lau.Identity_Response_IMEI_message_id_num = sm.message_id_num;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Location Updating Accept (0x02)") != -1);
                if (sm != null)
                {
                    lau.Location_Updating_Accept_message_frame = sm.message_frame;
                    lau.Location_Updating_Accept_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                   n.message_info.IndexOf("TMSI Reallocation Complete (0x1b)") != -1);
                if (sm != null)
                {
                    lau.TMSI_Reallocation_Complete_message_frame = sm.message_frame;
                    lau.TMSI_Reallocation_Complete_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("id-Iu-Release (1)") != -1);
                if (sm != null)
                {
                    lau.id_Iu_Release_1_message_frame = sm.message_frame;
                    lau.id_Iu_Release_1_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n => n.message_frame != lau.id_Iu_Release_1_message_frame
                 & n.message_info.IndexOf("id-Iu-Release (1)") != -1);
                if (sm != null)
                {
                    lau.id_Iu_Release_2_message_frame = sm.message_frame;
                    lau.id_Iu_Release_2_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Released (0x04)") != -1);
                if (sm != null)
                {
                    lau.RLSD_message_frame = sm.message_frame;
                    lau.RLSD_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Release Complete (0x05)") != -1);
                if (sm != null)
                {
                    lau.RLC_message_frame = sm.message_frame;
                    lau.RLC_message_time = sm.message_time;
                }

                strsql = kpiCommonClass.insertListTable(fields, lau );

                strsql = "insert into " + streamMessagePool.dataTableName + "_lau" + " values(" + strsql + ")";
                streamMessagePool.Acc.RunNoQuery(strsql);
            }
        }
    }
}
