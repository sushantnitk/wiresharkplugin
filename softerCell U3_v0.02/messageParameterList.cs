using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace softerCell_U3_v0._01
{
    public partial class messageParameterList : Form
    {
        public messageParameterList()
        {
            InitializeComponent();
        }

        private void messageParameterList_Load(object sender, EventArgs e)
        {
            middleDelegate.sendBEvent += new middleDelegate.SendBMessage(this.search_messageParameterList);
            this.Location = new Point(this.MdiParent.Location.X + this.MdiParent.Width / 2- SystemInformation.CaptionHeight,
                this.MdiParent.Location.Y +this.MdiParent.Height / 2 -
                2* (SystemInformation.MenuHeight + SystemInformation.CaptionHeight) / 2);
            this.Width = this.MdiParent.Width / 2;
            this.Height  = (this.MdiParent.Height - 2*(SystemInformation.MenuHeight + SystemInformation.CaptionHeight))/2;

            toolStripStatusLabel1.Text = string.Empty;
        }
        private void search_messageParameterList(int fr)
        {
            int message_s_begin_lineNumber = streamMessagePool.ml.ElementAt(fr).message_begin_lineNumber;
            int message_s_end_lineNumber = streamMessagePool.ml.ElementAt(fr).message_end_lineNumber;
            treeView1.Nodes.Clear();
            TreeNode txtRoot = new TreeNode("Message_Detail_" + fr.ToString());
            string line_detail = null;
            int line_number = 0;
            System.IO.StreamReader reader = new StreamReader(streamMessagePool.signallingFilePath);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            while (reader.Peek() >= 0)
            {
                line_detail = reader.ReadLine();
                line_number++;
                if (line_number >= message_s_begin_lineNumber)
                {
                    TreeNode rootMessage = new TreeNode(line_detail);
                    txtRoot.Nodes.Add(rootMessage);
                }
                if (line_number >= message_s_end_lineNumber)
                {
                    treeView1.Nodes.Add(txtRoot);
                    treeView1.ExpandAll();
                    reader.Close();
                    return;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            saveTreeDisplay.saveTree(treeView1);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Clipboard.Clear();
            toolStripStatusLabel1.Text = e.Node.Text;
            Clipboard.SetDataObject(e.Node .Text );//拷贝选中文本
            IDataObject iData = Clipboard.GetDataObject();
            //textBox2.Text = (String)iData.GetData(DataFormats.Text); //放到指定的文本控件
        }
    }
}
