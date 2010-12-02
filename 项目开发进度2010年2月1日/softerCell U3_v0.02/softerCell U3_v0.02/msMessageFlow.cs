using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace softerCell_U3_v0._01
{
    public partial class msMessageFlow : Form
    {
        public msMessageFlow()
        {
            InitializeComponent();
        }

        private void msMessageFlow_Load(object sender, EventArgs e)
        {
            middleDelegate.sendAEvent += new middleDelegate.SendAMessage(this.searchFr_msMessageFlow);
            middleDelegate.sendStrEvent += new middleDelegate.SendStrMessage(this.searchStr_msMessageFlow);
            this.Location = new Point(this.MdiParent.Location.X + this.MdiParent.Width / 2 - SystemInformation.CaptionHeight,
                this.MdiParent.Location.Y);
            this.Width = this.MdiParent.Width / 2;
            this.Height = (this.MdiParent.Height - 2 * (SystemInformation.MenuHeight + SystemInformation.CaptionHeight)) / 2;
        }

        //输入文本时，通过textbox----搜索-----用户消息流程
        private void searchStr_msMessageFlow(string searchstr)
        {
            HashSet<string> hs = new HashSet<string>();

            if (searchstr.IndexOf("0x") == -1)
                hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_imsi(streamMessagePool.ml, searchstr,true );//此处修改
            else
                hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_lr(streamMessagePool.ml, searchstr,true );//此处修改

            treeView1.Nodes.Clear();

            TreeNode txtRoot = new TreeNode("User_Message_" + searchstr);

            treeView1.Nodes.Add(txtRoot);

            foreach (streamMessagePool._stream_message sm in streamMessagePool.ml)
            {
                if (hs.Contains(sm.message_sccp_slr) || hs.Contains(sm.message_sccp_dlr) || hs.Contains(sm.message_gsm_a_imsi))
                {
                    Application.DoEvents();
                    string strsql = null;
                    FieldInfo[] fields = sm.GetType().GetFields();
                    foreach (var field in fields)
                    {
                        var sqlfieldvalue = field.GetValue(sm);
                        if (field.Name.IndexOf("message_number") == -1
                            && field.Name.IndexOf("message_begin_line") == -1
                            && field.Name.IndexOf("message_end_lineNumber") == -1
                                && field.Name.IndexOf("message_time") == -1)

                            strsql = strsql + " " + sqlfieldvalue + " ";
                    }
                    TreeNode rootMessage = new TreeNode(strsql);
                    txtRoot.Nodes.Add(rootMessage);
                }
            }
            treeView1.ExpandAll();
        }

        //点击时，通过frame----搜索-----用户消息流程
        private void searchFr_msMessageFlow(int fr)
        {
            //---------此处修改测试关联结果,需要强力搜索时改成  true
            HashSet<string> hs = messageFlowSearch.iu_cs_sinalling_relation_delegate_fr(fr, true);
            treeView1.Nodes.Clear();
            TreeNode txtRoot = new TreeNode("User_Message_" + fr.ToString());
            treeView1.Nodes.Add(txtRoot);
            foreach (streamMessagePool._stream_message sm in streamMessagePool.ml)
            {
                if (hs.Contains(sm.message_sccp_slr + sm.message_source)
                    || hs.Contains(sm.message_sccp_dlr + sm.message_destination)
                    || hs.Contains(sm.message_gsm_a_imsi)
                    || hs.Contains(sm.CIC + sm.message_source + sm.message_destination)
                    //|| hs.Contains(sm.CallingPartyNumber)
                    //|| hs.Contains(sm.message_id_num)
                    //|| hs.Contains(sm.CalledPartyNumber)
                    )
                {
                    Application.DoEvents();
                    string strsql = null;
                    FieldInfo[] fields = sm.GetType().GetFields();
                    foreach (var field in fields)
                    {
                        var sqlfieldvalue = field.GetValue(sm);
                        if (field.Name.IndexOf("message_number") == -1
                            && field.Name.IndexOf("message_begin_line") == -1
                            && field.Name.IndexOf("message_end_lineNumber") == -1
                                && field.Name.IndexOf("message_time") == -1)

                            strsql = strsql + " " + sqlfieldvalue + " ";
                    }
                    TreeNode rootMessage = new TreeNode(strsql);

                    string[] ar = strsql.Split(Convert.ToChar(" "));
                    if (fr == Int32.Parse(ar[4].ToString())) rootMessage.ForeColor = Color.Blue;//--------------------

                    txtRoot.Nodes.Add(rootMessage);
                }
            }
            treeView1.ExpandAll();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            saveTreeDisplay.saveTree(treeView1);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            foreach (TreeNode tn in treeView1.Nodes)
            {
                foreach (TreeNode tn1 in tn.Nodes)
                {
                    if (tn1.IsSelected == true) 
                    {
                        //MessageBox.Show(tn.Text.ToString());
                        //MessageBox.Show(tn1.Text.ToString());
                        string[] ar = tn1.Text.Split(Convert.ToChar(" "));
                        if (ar.Length > 4)
                        {
                            int fr = Int32.Parse(ar[4].ToString());//----------------
                            middleDelegate.DoSendBMessage(fr);
                        }
                    }

                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
