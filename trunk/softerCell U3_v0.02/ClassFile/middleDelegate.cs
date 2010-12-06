using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace softerCell_U3_v0._01
{

    public static class middleDelegate
    {
        //消息帧号
        public delegate void SendAMessage(int i);
        public static event SendAMessage sendAEvent;
        public static void DoSendAMessage(int i)
        {
            sendAEvent(i);
        }
        //消息帧号
        public delegate void SendBMessage(int i);
        public static event SendBMessage sendBEvent;
        public static void DoSendBMessage(int i)
        {
            sendBEvent(i);
        }
        //进度条
        public delegate void SendPMessage(int i);
        public static event SendPMessage sendPEvent;
        public static void DoSendPMessage(int i)
        {
            sendPEvent(i);
        }
        //消息关联因子
        public delegate void SendStrMessage(string str);
        public static event SendStrMessage sendStrEvent;
        public static void DoSendStrMessage(string str)
        {
            sendStrEvent(str);
        }
        //装ListBox
        public delegate void SendListMessage();
        public static event SendListMessage sendListEvent;
        public static void DoSendListMessage()
        {
            sendListEvent();
        }
    }
}
