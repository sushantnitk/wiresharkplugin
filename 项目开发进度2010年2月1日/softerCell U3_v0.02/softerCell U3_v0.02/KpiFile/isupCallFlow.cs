using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace softerCell_U3_v0._01
{
    class isupCallFlow
    {
        public static void isup_iam_acm_flow()
        {
            string strsql = null;
            streamMessagePool._isup_iam_message isup_iam_field = new streamMessagePool._isup_iam_message();
            FieldInfo[] fields = isup_iam_field.GetType().GetFields();

            strsql = kpiCommonClass.createListTable(fields);

            strsql = "create table " + streamMessagePool.dataTableName + "_isup_iam" + " (" + strsql + " ) ";
            streamMessagePool.Acc.RunNoQuery(strsql);

            IEnumerable<string> list_n =
            from n in streamMessagePool.ml
            where n.message_info.IndexOf("Initial address (1)") != -1 
            select n.message_frame;

            foreach (string i in list_n)//针对每1个类型的字段
            {
                int fr = Int32.Parse(i);

                //在此调用父窗口的进度条，采用托管的方式传递
                middleDelegate.DoSendPMessage(fr);
                Application.DoEvents();

                //建1条LAU流程消息
                streamMessagePool._isup_iam_message iam = new streamMessagePool._isup_iam_message();

                //找出现消息i的关联字段hs表
                HashSet<string> hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_fr(fr, false);

                //找出用户消息流程
                HashSet<string> hsEndMessage = new HashSet<string>();
                hsEndMessage.Add("Release complete (16)");         //切换完成
                IEnumerable<streamMessagePool._stream_message> list_fr =
                    messageFlowSearch.convert_hsRelationString_listUserMessage(hs, fr, hsEndMessage);

                streamMessagePool._stream_message sm = list_fr.First();
                iam.Initial_address_message_frame = sm.message_frame;
                iam.Initial_address_message_time = sm.message_time;

                sm = list_fr.FirstOrDefault(n => n.message_frame != i
                    & n.message_info.IndexOf("Address complete (6)") != -1);
                if (sm != null)
                {
                    iam.Address_complete_message_frame = sm.message_frame;
                    iam.Address_complete_message_time = sm.message_time;
                }

                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Release (12)") != -1);
                if (sm != null)
                {
                    iam.Release_message_frame = sm.message_frame;
                    iam.Release_message_time= sm.message_time;
                }

                sm = list_fr.FirstOrDefault(n => n.message_info.IndexOf("Release complete (16)") != -1);
                if (sm != null)
                {
                    iam.Release_complete_message_frame = sm.message_frame;
                    iam.Release_complete_message_time= sm.message_time;
                }


                strsql = kpiCommonClass.insertListTable(fields,iam);

                strsql = "insert into " + streamMessagePool.dataTableName + "_isup_iam" + " values(" + strsql + ")";
                streamMessagePool.Acc.RunNoQuery(strsql);
            }
        }
    }
}
