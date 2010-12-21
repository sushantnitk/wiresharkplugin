using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FollowSccpStream
{
    class AMessageDetail
    {
        public int PacketNum;
        public DateTime PacketTime;
        public string imsi;
        public string tmsi;
        public string m3ua_opc;
        public string m3ua_dpc;
        public string sccp_slr;
        public string sccp_dlr;
        public string message_type;
        public AMessageDetail()
        {
        }
    }
}
