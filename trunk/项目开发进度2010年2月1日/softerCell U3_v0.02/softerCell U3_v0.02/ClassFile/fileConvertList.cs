using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace softerCell_U3_v0._01
{
    class fileConvertList
    {
        private static int message_number = -1;
        public const int line_total_number = 2000000;//读入总行数，分次读入
        private static  int pb;
        private static int fr;
        private static Regex regex = new Regex(@"( )+");
        private static Stack<streamMessagePool._stream_message> s = new Stack<streamMessagePool._stream_message>();

        public static void open_iu_cs_file()
        {
            int file_line_number = 0;
            string file_line_detail = null;

            string line_detail = null;//消息行号
            int line_number = 0;//消息行号
            int read_message = 0;//是否读入消息
            int read_sctp = 0;//读入的消息是否有组包

            StreamReader rd = new StreamReader(streamMessagePool.signallingFilePath);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            Queue<string> q = new Queue<string>();

            while (rd.Peek() >= 0)
            {
                GC.Collect();

                Application.DoEvents();

                //MemoryStream ms = new MemoryStream();
                //StreamWriter sw = new StreamWriter(ms);

                while (file_line_number < line_total_number + line_number && file_line_number >= line_number)//在此分次读取
                {
                    file_line_detail = rd.ReadLine();
                    //sw.WriteLine(file_line_detail);
                    q.Enqueue(file_line_detail);
                    file_line_number++;
                }

                //sw.Flush();
                //ms.Position = 0;
                //StreamReader reader = new StreamReader(ms);
                //reader.BaseStream.Seek(0, SeekOrigin.Begin);
                //while (reader.Peek() >= 0)

                pb = 0;

                while (q.Count > 0)
                {
                    //line_detail = reader.ReadLine();
                    line_detail = q.Dequeue();
                    line_number++;
                    read_message++;
                    pb++;

                    //在此调用父窗口的进度条，采用托管的方式传递
                    middleDelegate.DoSendPMessage(pb);
                    Application.DoEvents();

                    if (line_detail != null)
                    {
                        if (line_detail.Length > 10)
                        {
                            if (line_detail.Substring(0, 2).IndexOf("Fr") != -1)
                            {
                                read_message = 0;//开关参数
                                read_sctp = 0;
                            }
                        }

                        if (read_message == 0)//如果出现消息行，那么存入基本消息到内存
                        {
                            message_number++;
                            streamMessagePool._stream_message sm = new streamMessagePool._stream_message();
                            sm.message_begin_lineNumber = line_number;//消息开始位置
                            line_detail = regex.Replace(line_detail, " ");
                            string[] ar = line_detail.Split(Convert.ToChar(" "));
                            sm.message_frame = message_number.ToString();//----------------------------------------------->
                            sm.message_number = ar[1].Trim();
                            fr = message_number;
                            if (fr != 0)
                            {
                                //streamMessagePool.ml.ElementAt(fr - 1).message_end_lineNumber = line_number;

                                streamMessagePool._stream_message _sm = s.Pop();
                                _sm.message_end_lineNumber = line_number;
                                s.Push(_sm);

                            }
                            //streamMessagePool.ml.Add(sm);
                            s.Push(sm);
                        }

                        if (line_detail.IndexOf("Stream Control Transmission Protocol") != -1)
                        {
                            read_sctp++;
                            if (read_sctp != 1)//如果出现消息组包，那么直接复制前1条消息的内容
                            {
                                message_number++;
                                fr = message_number;
                                streamMessagePool._stream_message sm = new streamMessagePool._stream_message();

                                //streamMessagePool._stream_message _sm = streamMessagePool.ml.ElementAt(fr - 1);
                                streamMessagePool._stream_message _sm = s.Peek();

                                sm.message_begin_lineNumber = _sm.message_begin_lineNumber;
                                sm.message_frame = message_number.ToString();
                                sm.message_number = _sm.message_number;
                                sm.message_time = _sm.message_time;
                                sm.message_ttime = _sm.message_ttime;
                                sm.message_source = _sm.message_source;
                                sm.message_destination = _sm.message_destination;
                                sm.message_protocol = _sm.message_protocol;
                                sm.message_info = _sm.message_info;
                                sm.message_end_lineNumber = _sm.message_end_lineNumber;
                                if (fr != 1)
                                {
                                    _sm.message_end_lineNumber = line_number;
                                }

                                //streamMessagePool.ml.Add(sm);
                                s.Push(sm);
                            }
                        }

                        if (line_detail.Length > 2)
                        {
                            //ThreadPool.QueueUserWorkItem(new WaitCallback(infoName),line_detail);
                            protocolName(line_detail);
                            infoName(line_detail);
                        }
                    }
                }

                //reader.Dispose();
                //sw.Dispose();
                //ms.Dispose();
                //streamMessagePool.ml.ElementAt(streamMessagePool.ml.Count() - 1).message_end_lineNumber = line_number;

                streamMessagePool._stream_message _smLast = s.Pop();
                _smLast.message_end_lineNumber = line_number;
                s.Push(_smLast);

            }
            rd.Close();
            rd.Dispose();
            streamMessagePool.ml.AddRange(s.Reverse());
        }

        private static void protocolName(string line_detail)
        {
            if (line_detail.Substring(0, 1) != " ")
            {
                streamMessagePool._stream_message sm = s.Pop();

                line_detail = regex.Replace(line_detail, "").Trim();

                sm.message_protocol = "M3UA";

                if (line_detail.IndexOf("GSMA-I") != -1)
                    sm.message_protocol = "GSM";
                if (line_detail.IndexOf("GSMSMS") != -1)
                    sm.message_protocol = "SMS";
                if (line_detail.IndexOf("UserPart") != -1)
                    sm.message_protocol = "ISUP";
                if (line_detail.IndexOf("RadioAccess") != -1)
                    sm.message_protocol = "RANAP";
                if (line_detail.IndexOf("ConnectionControl") != -1)
                    sm.message_protocol = "SCCP";

                s.Push(sm);
            }
        }

        private static void infoName(string line_detail)
        {

            if (line_detail.IndexOf(":") != -1)
            {

                string[] ar = line_detail.Split(Convert.ToChar(":"));
                if (ar.Length < 5)
                {
                    streamMessagePool._stream_message sm = s.Pop();

                    ar[0] = regex.Replace(ar[0], "");

                    if (ar[0].IndexOf("OPC") != -1)
                        sm.message_source = ar[1].Trim();
                    if (ar[0].IndexOf("DPC") != -1)
                        sm.message_destination = ar[1].Trim();

                    //此处更正时间-->测试OK
                    if (ar[0].IndexOf("ArrivalTime") != -1)
                    {
                        line_detail = line_detail.Replace("Arrival Time:", "");
                        DateTime dt = DateTime.Parse(line_detail.Trim().Substring(0,20));
                        sm.message_ttime = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    if (ar[0].IndexOf("Timesincereferenceorfirstframe") != -1)
                        sm.message_time = ar[1].Replace(" seconds]", "").Trim();

                    //此处更新info
                    if (ar[0].IndexOf("procedureCode") != -1)
                        sm.message_info = ar[1].Trim();
                    if (ar[0].IndexOf("MessageType") != -1 || ar[0].IndexOf("Messagetype") != -1)
                        sm.message_info = ar[1].Trim();
                    //此处更新isup
                    if (ar[0].IndexOf("CalledPartyNumber") != -1)
                        sm.CalledPartyNumber = ar[1].Trim();
                    if (ar[0].IndexOf("CallingPartyNumber") != -1)
                        sm.CallingPartyNumber = ar[1].Trim();
                    if (ar[0].IndexOf("CIC") != -1)
                        sm.CIC = ar[1].Trim();


                    if (ar[0].IndexOf("SourceLocalReference") != -1)
                        sm.message_sccp_slr = ar[1].Trim();

                    if (ar[0].IndexOf("DestinationLocalReference") != -1)
                        sm.message_sccp_dlr = ar[1].Trim();

                    if (ar[0].IndexOf("TMSI/P-TMSI") != -1)
                        sm.message_gsm_a_tmsi = ar[1].Trim();

                    if (ar[0].IndexOf("IMSIdigits") != -1)
                        sm.message_gsm_a_imsi = ar[1].Trim();


                    if (ar[0].IndexOf("rNC-ID") != -1)                                   //<----------增加字段处
                        sm.message_rnc_id = ar[1].Trim();

                    if (ar[1].IndexOf("  Type of identity") != -1) { sm.message_id_type = ar[2].Trim(); }
                    if (ar[0].IndexOf("MobileIdentityType") != -1)
                        sm.message_id_type = ar[1].Trim();

                    if (ar[0].IndexOf("BCDDigits") != -1)
                        sm.message_id_num = ar[1].Trim();

                    if (ar[0].IndexOf("lAC") != -1)
                        sm.message_lac = ar[1].Trim();

                    if (ar[0].IndexOf("radioNetwork") != -1)
                        sm.message_radioNetwork = ar[1].Trim();

                    if (ar[0].IndexOf("Cause") != -1)
                        sm.message_Cause = ar[1].Trim();

                    s.Push(sm);
                }
            }
        }

        public static object ExecuteCmd(string cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            //调用程序名 
            {
                startInfo.Arguments = "/C " + cmd;
                //调用命令 CMD 
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
            }

            Process p = Process.Start(startInfo);
            string strOutput = p.StandardOutput.ReadToEnd();
            string strError = p.StandardError.ReadToEnd();

            p.WaitForExit();


            if ((strOutput.Length != 0))
            {
                return strOutput;
            }
            else if ((strError.Length != 0))
            {
                return strError;
            }

            //return cmd;

            return "OK";
        }
    }
}

