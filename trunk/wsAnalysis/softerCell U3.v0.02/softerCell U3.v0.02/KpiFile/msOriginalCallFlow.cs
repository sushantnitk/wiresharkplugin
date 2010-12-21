using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace softerCell_U3_v0._01
{
    class msOriginalCallFlow
    {
        public static void mobile_original_call_flow()
        {
            string strsql = null;
            streamMessagePool._moc_message moc_field = new streamMessagePool._moc_message();
            FieldInfo[] fields = moc_field.GetType().GetFields();

            strsql = kpiCommonClass.createListTable(fields);

            strsql = "create table " + streamMessagePool.dataTableName + "_moc" + " (" + strsql + " ) ";
            streamMessagePool.Acc.RunNoQuery(strsql);

            IEnumerable<string> list_n =
            from n in streamMessagePool.ml
            where n.message_info.IndexOf("CM Service Request") != -1
            select n.message_frame;

            foreach (string i in list_n)//针对每1个类型的字段
            {
                int fr = Int32.Parse(i);

                //在此调用父窗口的进度条，采用托管的方式传递
                middleDelegate.DoSendPMessage(fr);
                Application.DoEvents();

                //建1条moc流程消息
                streamMessagePool._moc_message moc = new streamMessagePool._moc_message();

                //找出现消息i的关联字段hs表
                HashSet<string> hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_fr(fr, false);

                //找出用户消息流程
                HashSet<string> hsEndMessage = new HashSet<string>();
                hsEndMessage.Add("Release Complete (0x05)");//主叫呼出的SCCP连接释放
                hsEndMessage.Add("Connect Acknowledge (0x0f)");//通话连接确认
                IEnumerable<streamMessagePool._stream_message> list_fr =
                    messageFlowSearch.convert_hsRelationString_listUserMessage(hs, fr, hsEndMessage);

                streamMessagePool._stream_message sm = list_fr.First();
                moc.CM_Service_Request_message_frame = sm.message_frame;
                moc.CM_Service_Request_message_time = sm.message_time;
                moc.CM_Service_Request_message_rnc_id = sm.message_rnc_id;
                moc.CM_Service_Request_message_lac = sm.message_lac;
                moc.CM_Service_Request_message_gsm_a_tmsi = sm.message_gsm_a_tmsi;

                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Connection Confirm (0x02)") != -1);
                if (sm != null)
                {
                    moc.CC_message_frame = sm.message_frame;
                    moc.CC_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("id-CommonID (15)") != -1);
                if (sm != null)
                {
                    moc.id_CommonID_message_frame = sm.message_frame;
                    moc.id_CommonID_message_time = sm.message_time;
                    moc.id_CommonID_message_imsi = sm.message_gsm_a_imsi;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Authentication Request (0x12)") != -1);
                if (sm != null)
                {
                    moc.Authentication_Request_message_frame = sm.message_frame;
                    moc.Authentication_Request_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                    n.message_info.IndexOf("Authentication Response (0x14)") != -1);
                if (sm != null)
                {
                    moc.Authentication_Response_message_frame = sm.message_frame;
                    moc.Authentication_Response_message_time = sm.message_time;
                }

                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Identity Request (0x18)") != -1
                        & n.message_id_type.IndexOf("IMEI") != -1);
                if (sm != null)
                {
                    moc.Identity_Request_IMEI_message_frame = sm.message_frame;
                    moc.Identity_Request_IMEI_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Identity Response (0x19)") != -1
                    & n.message_id_type.IndexOf("IMEI") != -1);
                if (sm != null)
                {
                    moc.Identity_Response_IMEI_message_frame = sm.message_frame;
                    moc.Identity_Response_IMEI_message_time = sm.message_time;
                    moc.Identity_Response_IMEI_message_id_type = sm.message_id_type;
                    moc.Identity_Response_IMEI_message_id_num = sm.message_id_num;
                }
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("CM Service Accept (0x21)") != -1);
                if (sm != null)
                {
                    moc.CM_Service_Accept_message_frame = sm.message_frame;
                    moc.CM_Service_Accept_message_time = sm.message_time;
                }

                //1.cp
                sm = list_fr.FirstOrDefault(n =>
                 n.message_info.IndexOf("CP-DATA (0x01)") != -1);
                if (sm != null)
                {
                    moc.CP_DATA_message_frame= sm.message_frame;
                    moc.CP_DATA_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                 n.message_info.IndexOf("CP-ACK (0x04)") != -1);
                if (sm != null)
                {
                    moc.CP_ACK_message_frame = sm.message_frame;
                    moc.CP_ACK_message_time = sm.message_time;
                }

                //2.register
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("Register (0x3b)") != -1);
                if (sm != null)
                {
                    moc.Register_message_frame = sm.message_frame;
                    moc.Register_message_time = sm.message_time;
                }

                //3.setup
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("Setup (0x05)") != -1);
                if (sm != null)
                {
                    moc.Setup_message_frame = sm.message_frame;
                    moc.Setup_message_time = sm.message_time;
                    moc.Setup_called_number = sm.message_id_num;
                }
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("Call Proceeding (0x02)") != -1);
                if (sm != null)
                {
                    moc.Call_Proceeding_message_frame = sm.message_frame;
                    moc.Call_Proceeding_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("id-RAB-Assignment (0)") != -1);
                if (sm != null)
                {
                    moc.id_RAB_Assignment_r_message_frame = sm.message_frame;
                    moc.id_RAB_Assignment_r_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n => n.message_frame != moc.id_RAB_Assignment_r_message_frame
                  & n.message_info.IndexOf("id-RAB-Assignment (0)") != -1);
                if (sm != null)
                {
                    moc.id_RAB_Assignment_s_message_frame = sm.message_frame;
                    moc.id_RAB_Assignment_s_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("Alerting (0x01)") != -1);
                if (sm != null)
                {
                    moc.Alerting_message_frame = sm.message_frame;
                    moc.Alerting_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("Connect (0x07)") != -1);
                if (sm != null)
                {
                    moc.Connect_message_frame = sm.message_frame;
                    moc.Connect_message_time = sm.message_time;
                }
                sm = list_fr.FirstOrDefault(n =>
                n.message_info.IndexOf("Connect Acknowledge (0x0f)") != -1);
                if (sm != null)
                {
                    moc.Connect_Acknowledge_message_frame = sm.message_frame;
                    moc.Connect_Acknowledge_message_time = sm.message_time;
                }


                strsql = kpiCommonClass.insertListTable(fields,moc );

                strsql = "insert into " + streamMessagePool.dataTableName + "_moc" + " values(" + strsql + ")";
                streamMessagePool.Acc.RunNoQuery(strsql);
            }
        }
    }
}
