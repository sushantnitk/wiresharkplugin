using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace softerCell_U3_v0._01
{
    public partial class allMessageFlow : Form
    {
        public allMessageFlow()
        {
            InitializeComponent();
        }

        private msMessageFlow f1 = new msMessageFlow();
        private messageParameterList f2 = new messageParameterList();

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            toolStripComboBox1.Items.Clear();
            toolStripComboBox1.Items.Add("已经关联的数据");
            toolStripComboBox1.Items.Add("已经解码的数据");
        }

        private  string sfp()
        {
            string sfp = string.Empty;
            string licPath = streamMessagePool.appPath + "\\log\\OpenFileRecord";
            System.IO.StreamReader reader = new StreamReader(licPath);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            while (reader.Peek() >= 0)
            {
                sfp = reader.ReadLine();
            }
            reader.Close();
            return sfp;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                streamMessagePool.signallingFilePath = sfp();
                toolStripStatusLabel1.Text = streamMessagePool.signallingFilePath;
                Application.DoEvents();
                if (toolStripComboBox1.Text == "已经解码的数据")
                {
                    toolStripStatusLabel2.Text = "softerCell_have_read";
                    open_iu_cs_db("softerCell_have_read");
                }

                if (toolStripComboBox1.Text == "已经关联的数据")
                {
                    toolStripStatusLabel2.Text = "softerCell_user";
                    open_iu_cs_db("softerCell_user");
                }
            }
            catch { return; }
        }

        private void open_iu_cs_db(string db)
        {
            //GC.Collect();
            DataTable dt = streamMessagePool.Acc.RunQuery("select  * from " + db);
            streamMessagePool.ml.Clear();
            DataTableToList(dt);
            //loadMessageToListBox();
            middleDelegate.DoSendListMessage();
        }
        private void loadMessageToListBox()
        {
            GC.Collect();
            listBox1.Items.Clear();
            int rc = streamMessagePool.ml.Count;
            int i = 0;
            foreach (streamMessagePool._stream_message sm in streamMessagePool.ml)
            {
                i++;
                toolStripLabel1.Text = (int)(100 * i / rc) + "%";
                toolStripLabel1.Visible = true;
                Application.DoEvents();

                listBox1.Items.Add(sm.message_frame + "   " +
                    sm.message_ttime + "   " +
                    sm.message_source + "   " +
                    sm.message_destination + "   " +
                    sm.message_protocol + "   " +
                    sm.message_info + "   " +
                    sm.message_lac + "   " +
                    sm.message_rnc_id + "   " +
                    sm.message_gsm_a_imsi + "   " +
                    sm.message_sccp_slr + "   " +
                    sm.message_sccp_dlr + "   " +
                    sm.message_id_type + "   " +
                    sm.message_id_num + "   " +
                    sm.message_rnc_id + "   " +
                    sm.message_Cause + "   " +
                    sm.message_radioNetwork);
            }
        }
        private void DataTableToList(DataTable dt)
        {
            //GC.Collect();
            Stack<streamMessagePool._stream_message> s = new Stack<streamMessagePool._stream_message>();
            int rc = dt.Rows.Count;
            for (int i = 0; i < rc; i++)
            {
                toolStripLabel1.Text = (int)(100 * i / rc) + "%";
                toolStripLabel1.Visible = true;
                Application.DoEvents();
                streamMessagePool._stream_message sm = new streamMessagePool._stream_message();
                sm.message_number = dt.Rows[i]["message_number"].ToString();
                sm.message_ttime = dt.Rows[i]["message_ttime"].ToString();
                sm.message_time = dt.Rows[i]["message_time"].ToString();
                sm.message_source = dt.Rows[i]["message_source"].ToString();
                sm.message_destination = dt.Rows[i]["message_destination"].ToString();
                sm.message_protocol = dt.Rows[i]["message_protocol"].ToString();
                sm.message_info = dt.Rows[i]["message_info"].ToString();
                sm.message_frame = dt.Rows[i]["message_frame"].ToString();
                sm.message_begin_lineNumber = Int32.Parse(dt.Rows[i]["message_begin_lineNumber"].ToString());
                sm.message_end_lineNumber = Int32.Parse(dt.Rows[i]["message_end_lineNumber"].ToString());
                sm.message_sccp_slr = dt.Rows[i]["message_sccp_slr"].ToString();
                sm.message_sccp_dlr = dt.Rows[i]["message_sccp_dlr"].ToString();
                sm.message_gsm_a_imsi = dt.Rows[i]["message_gsm_a_imsi"].ToString();
                sm.message_gsm_a_tmsi = dt.Rows[i]["message_gsm_a_tmsi"].ToString();
                sm.message_rnc_id = dt.Rows[i]["message_rnc_id"].ToString();
                sm.message_lac = dt.Rows[i]["message_lac"].ToString();
                sm.message_id_type = dt.Rows[i]["message_id_type"].ToString();
                sm.message_id_num = dt.Rows[i]["message_id_num"].ToString();
                sm.message_radioNetwork = dt.Rows[i]["message_radioNetwork"].ToString();
                sm.message_Cause = dt.Rows[i]["message_Cause"].ToString();
                sm.CalledPartyNumber= dt.Rows[i]["CalledPartyNumber"].ToString();
                sm.CallingPartyNumber= dt.Rows[i]["CallingPartyNumber"].ToString();
                sm.CIC = dt.Rows[i]["CIC"].ToString();
                s.Push(sm);
            }
            streamMessagePool.ml.AddRange(s.Reverse());
        }
        private void allMessageFlow_Load(object sender, EventArgs e)
        {
            f1.MdiParent = this.MdiParent;
            f1.Show();
            f2.MdiParent = this.MdiParent;
            f2.Show();

            this.Location = new Point(this.MdiParent.Location.X, this.MdiParent.Location.Y);
            this.Width = this.MdiParent.Width / 2 - SystemInformation.CaptionHeight;
            this.Height = this.MdiParent.Height - 2 * (SystemInformation.MenuHeight + SystemInformation.CaptionHeight);

            toolStripLabel1.Text = string.Empty;
            toolStripStatusLabel1.Text = string.Empty;
            toolStripStatusLabel2.Text = string.Empty;
            toolStripStatusLabel3.Text = string.Empty;

            streamMessagePool.appPath = Application.StartupPath.ToString();
            streamMessagePool.Acc.Init(null, "CN-Iu", null, null);
            streamMessagePool.Acc.Open();

            //loadMessageToListBox();
            middleDelegate.sendListEvent += new middleDelegate.SendListMessage(this.loadMessageToListBox);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            f1.MdiParent = this.MdiParent;
            f1.Show();
            f2.MdiParent = this.MdiParent;
            f2.Show();

            int fr = listBox1.SelectedIndex;
            middleDelegate.DoSendAMessage(fr);
            middleDelegate.DoSendBMessage(fr);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            middleDelegate.DoSendStrMessage(toolStripTextBox1.Text);
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

    }
}
