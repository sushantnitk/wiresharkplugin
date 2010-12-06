using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace softerCell_U3_v0._01
{
    class streamMessagePool
    {
        public static List<_stream_message> ml = new List<_stream_message>();
        public static IDBAccess Acc = DBAccessFactory.Create(DBType.SQL);

        public static string appPath = string.Empty;
        public static string dataTableName = "softerCell";

        public static string signallingFilePath;


        public class _kpi_stat
        {
            public int id;
            public string message_info;
            public int message_sum;
            public double loss_sum;
            public double loss_rate;
            public double message_time;
            public double step_delay;
            public double delay_rate;
        }
        public class _user_link
        {
            //public int message_user_number;//用户编号
            //public Dictionary<string, int> user_relation_identity = new Dictionary<string, int>();
            ////public string message_sccp_slr;
            ////public string message_sccp_dlr;
            ////public string message_gsm_a_imsi;//用户的IMSI
            ////public string message_id_num;//识别号码
            //public string SetupNumber;
            //public string CalledPartyNumber;
            //public string CallingPartyNumber;
            //public string CIC;
            //public HashSet<string> message_number_list = new HashSet<string>();

        }
        public class _stream_message
        {

            //public int message_user_number;//用户编号
            public string message_ttime;//采集时间
            public string message_frame;//因为有组包，原始编号修正后的消息编号

            public string message_number;//原始文件消息编号
            public string message_time;//时延统计
            public string message_source;
            public string message_destination;
            public string message_protocol;//用的最高协议栈
            public string message_info;//消息内容


            public int message_begin_lineNumber;
            public int message_end_lineNumber;

            public string message_sccp_slr;
            public string message_sccp_dlr;

            public string message_gsm_a_imsi;//用户的IMSI
            public string message_gsm_a_tmsi;
            public string message_rnc_id;//rnc编号
            public string message_lac;//lac编号
            public string message_id_type;//识别类型
            public string message_id_num;//识别号码

            public string message_radioNetwork;//<-------------------------------------增加字段处
            public string message_Cause;

            public string CalledPartyNumber;
            public string CallingPartyNumber;
            public string CIC;

        }

        public class _lau_message
        {
            //public int message_user_number;
            public string Location_Updating_Request_message_frame;
            public string Location_Updating_Request_message_time;
            public string Location_Updating_Request_message_rnc_id;
            public string Location_Updating_Request_message_lac;
            //public string Location_Updating_Request_message_gsm_a_imsi;
            public string Location_Updating_Request_message_gsm_a_tmsi;
            public string CC_message_frame;
            public string CC_message_time;
            public string Identity_Request_IMSI_message_frame;
            public string Identity_Request_IMSI_message_time;
            public string Identity_Response_IMSI_message_frame;
            public string Identity_Response_IMSI_message_time;
            public string Identity_Response_IMSI_message_id_type;
            public string Identity_Response_IMSI_message_id_num;
            public string id_CommonID_message_frame;
            public string id_CommonID_message_time;
            public string id_CommonID_message_imsi;
            public string Authentication_Request_message_frame;
            public string Authentication_Request_message_time;
            public string Authentication_Response_message_frame;
            public string Authentication_Response_message_time;
            public string Identity_Request_IMEI_message_frame;
            public string Identity_Request_IMEI_message_time;
            public string Identity_Response_IMEI_message_frame;
            public string Identity_Response_IMEI_message_time;
            public string Identity_Response_IMEI_message_id_type;
            public string Identity_Response_IMEI_message_id_num;
            public string Location_Updating_Accept_message_frame;
            public string Location_Updating_Accept_message_time;
            public string TMSI_Reallocation_Complete_message_frame;
            public string TMSI_Reallocation_Complete_message_time;
            public string id_Iu_Release_1_message_frame;
            public string id_Iu_Release_1_message_time;
            public string id_Iu_Release_2_message_frame;
            public string id_Iu_Release_2_message_time;
            public string RLSD_message_frame;
            public string RLSD_message_time;
            public string RLC_message_frame;
            public string RLC_message_time;

        }
        public class _moc_message
        {
            //CM Service Request  CM_Service_Request
            //(Int. ITU) CC
            //id-CommonID
            //CM Service Accept   CM_Service_Accept
            //Setup
            //Call Proceeding     Call_Proceeding
            //id-RAB-Assignment   id_RAB_Assignment
            //id-RAB-Assignment   id_RAB_Assignment
            //Alerting
            //Connect 
            //Connect Acknowledge  Connect_Acknowledge

            //GSM  CM Service Request (0x24) 
            //SCCP  Connection Confirm (0x02)
            //RANAP  id-CommonID (15)    0x04
            //GSM  CM Service Accept (0x21)  
            //SMS  CP-DATA (0x01)    0x9f3b02
            //GSM  CP-ACK (0x04)    0x045567 
            // GSM  CP-DATA (0x01)    0x04556
            // GSM  CP-ACK (0x04)    0x9f3b02
            // RANAP  id-Iu-Release (1)    0x
            // RANAP  id-Iu-Release (1)    0x
            // SCCP  Released (0x04)  0x9f3b0
            // SCCP  Release Complete (0x05) 

//GSM  CM Service Request (0x24)  0x
//SCCP  Connection Confirm (0x02)  0
//RANAP  id-CommonID (15)    0x03082
//GSM  Identity Request (0x18)    0x
//GSM  Identity Response (0x19)    0
//GSM  CM Service Accept (0x21)    0
// GSM  Register (0x3b)    0xa8c701 



            //public int message_user_number;
            public string CM_Service_Request_message_frame;
            public string CM_Service_Request_message_time;
            public string CM_Service_Request_message_rnc_id;
            public string CM_Service_Request_message_lac;
            //public string CM_Service_Request_message_gsm_a_imsi;
            public string CM_Service_Request_message_gsm_a_tmsi;
            public string CC_message_frame;
            public string CC_message_time;
            public string id_CommonID_message_frame;
            public string id_CommonID_message_time;
            public string id_CommonID_message_imsi;
            public string Authentication_Request_message_frame;
            public string Authentication_Request_message_time;
            public string Authentication_Response_message_frame;
            public string Authentication_Response_message_time;
            public string Identity_Request_IMEI_message_frame;
            public string Identity_Request_IMEI_message_time;
            public string Identity_Response_IMEI_message_frame;
            public string Identity_Response_IMEI_message_time;
            public string Identity_Response_IMEI_message_id_type;
            public string Identity_Response_IMEI_message_id_num;
            public string CM_Service_Accept_message_frame;
            public string CM_Service_Accept_message_time;
            public string CP_DATA_message_frame;
            public string CP_DATA_message_time;
            public string CP_ACK_message_frame;
            public string CP_ACK_message_time;
            public string Register_message_frame;
            public string Register_message_time;
            public string Setup_message_frame;
            public string Setup_message_time;
            public string Setup_called_number;
            public string Call_Proceeding_message_frame;
            public string Call_Proceeding_message_time;
            public string id_RAB_Assignment_r_message_frame;
            public string id_RAB_Assignment_r_message_time;
            public string id_RAB_Assignment_s_message_frame;
            public string id_RAB_Assignment_s_message_time;
            public string Alerting_message_frame;
            public string Alerting_message_time;
            public string Connect_message_frame;
            public string Connect_message_time;
            public string Connect_Acknowledge_message_frame;
            public string Connect_Acknowledge_message_time;
        }
        public class _mtc_message
        {
            //id-Paging        id_Paging
            //Paging Response   Paging_Response  
            //(Int. ITU) CC                   
            //id-CommonID          
            //Identity Request     
            //Identity Response    
            //Setup                
            //Call Confirmed    Call_Confirmed 
            //id-RAB-Assignment    
            //id-RAB-Assignment    
            //Alerting 
            //Connect 
            //Connect Acknowledge

            //RANAP  id-Paging (14)      460007
            //GSM  Paging Response (0x27)  0x0
            //SCCP  Connection Confirm (0x02) 
            //RANAP  id-CommonID (15)    0x0f0
            //GSM  Authentication Request (0x1
            //GSM  Authentication Response (0x
            //M3UA  CP-DATA (0x01)    0x0f0302
            //GSM  CP-ACK (0x04)    0x817a02  
            //SMS  CP-DATA (0x01)    0x817a02 
            //GSM  CP-ACK (0x04)    0x0f0302  
            //RANAP  id-Iu-Release (1)    0x0f
            //RANAP  id-Iu-Release (1)    0x81
            //SCCP  Released (0x04)  0x817a02 
            //SCCP  Release Complete (0x05)  0

            //public int message_user_number;
            public string id_Paging_message_frame;
            public string id_Paging_message_time;
            public string id_Paging_message_lac;
            public string id_Paging_message_ranap_imsi;
            public string Paging_Response_message_frame;
            public string Paging_Response_message_time;
            public string Paging_Response_message_rnc_id;
            public string Paging_Response_message_lac;
            //public string Paging_Response_message_gsm_a_imsi;
            public string Paging_Response_message_gsm_a_tmsi;
            public string CC_message_frame;
            public string CC_message_time;
            public string id_CommonID_message_frame;
            public string id_CommonID_message_time;
            public string id_CommonID_message_imsi;
            public string Authentication_Request_message_frame;
            public string Authentication_Request_message_time;
            public string Authentication_Response_message_frame;
            public string Authentication_Response_message_time;
            public string CP_DATA_message_frame;
            public string CP_DATA_message_time;
            public string CP_ACK_message_frame;
            public string CP_ACK_message_time;
            public string Identity_Request_IMEI_message_frame;
            public string Identity_Request_IMEI_message_time;
            public string Identity_Response_IMEI_message_frame;
            public string Identity_Response_IMEI_message_time;
            public string Identity_Response_IMEI_message_id_type;
            public string Identity_Response_IMEI_message_id_num;
            public string Setup_message_frame;
            public string Setup_message_time;
            public string Setup_calling_number;
            public string Call_Confirmed_message_frame;
            public string Call_Confirmed_message_time;
            public string id_RAB_Assignment_r_message_frame;
            public string id_RAB_Assignment_r_message_time;
            public string id_RAB_Assignment_s_message_frame;
            public string id_RAB_Assignment_s_message_time;
            public string Alerting_message_frame;
            public string Alerting_message_time;
            public string Connect_message_frame;
            public string Connect_message_time;
            public string Connect_Acknowledge_message_frame;
            public string Connect_Acknowledge_message_time;
        }
        public class _ho_in_message
        {
            //(Int. ITU) CR
            //(Int. ITU) CC
            //(Int. ITU) DT1
            //id-RelocationResourceAllocation   id_RelocationResourceAllocation
            //id-RelocationResourceAllocation
            //id-RelocationDetect               id_RelocationDetect
            //id-RelocationComplete             id_RelocationComplete
            //public int message_user_number;
            //public string CR_message_frame;
            //public string CR_message_time;
            //public string CC_message_frame;
            //public string CC_message_time;
            //public string DT1_message_frame;
            //public string DT1_message_time;
            public string id_RelocationResourceAllocation_r_message_frame;
            public string id_RelocationResourceAllocation_r_message_time;
            public string id_RelocationResourceAllocation_r_imsi;
            public string id_RelocationResourceAllocation_s_message_frame;
            public string id_RelocationResourceAllocation_s_message_time;
            public string id_RelocationDetect_message_frame;
            public string id_RelocationDetect_message_time;
            public string id_RelocationComplete_message_frame;
            public string id_RelocationComplete_message_time;
        }
        public class _ho_out_message
        {
            //Data Form 1 (0x06)            Data_Form_1
            //id-RelocationPreparation (2)  id_RelocationPreparation_r
            //id-RelocationPreparation (2)  id_RelocationPreparation_s
            //id-Iu-Release (1)             id_Iu_Release_r
            //id-Iu-Release (1)             id_Iu_Release_s
            //Released (0x04)               released_r 
            //Release Complete (0x05)       released_s
            //public int message_user_number;
            //public string Data_Form_1_message_frame;
            //public string Data_Form_1_message_time;
            public string id_RelocationPreparation_r_message_frame;
            public string id_RelocationPreparation_r_message_time;
            public string id_RelocationPreparation_s_message_frame;
            public string id_RelocationPreparation_s_message_time;
            public string id_Iu_Release_r_message_frame;
            public string id_Iu_Release_r_message_time;
            public string id_Iu_Release_r_radioNetwork;//<-------------------------------------增加字段处
            public string id_Iu_Release_s_message_frame;
            public string id_Iu_Release_s_message_time;
            public string released_r_message_frame;
            public string released_r_message_time;
            public string released_s_message_frame;
            public string released_s_message_time;
        }
        public class _iu_release_request_message
        {
            //public int message_user_number;
            public string iu_release_request_message_frame;
            public string iu_release_request_message_time;//<-------------------------------------增加字段处
            public string iu_release_request_message_radioNetwork;
            public string iu_release_request_message_Cause;
            public string iu_release_request_imsi;
            public string iu_release_request_imei;
            public string iu_release_request_rnc_id;
        }
        public class _isup_iam_message
        {
            //Initial address (1) 
            //Address complete (6)
            //Release (12)  
            //Release complete (16) 
            //public int message_user_number;
            public string Initial_address_message_frame;
            public string Initial_address_message_time;
            public string Address_complete_message_frame;
            public string Address_complete_message_time;
            public string Release_message_frame;
            public string Release_message_time;
            public string Release_complete_message_frame;
            public string Release_complete_message_time;
        }
    }
}
