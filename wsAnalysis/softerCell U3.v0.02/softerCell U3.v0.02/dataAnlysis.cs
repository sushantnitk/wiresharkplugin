using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace softerCell_U3_v0._01
{
    public partial class dataAnlysis : Form
    {
        public dataAnlysis()
        {
            InitializeComponent();
        }

        private void dataAnlysis_Load(object sender, EventArgs e)
        {

            treeView1.Nodes.Clear();
            //提取数据库中各表的字段
            DataTable dt =streamMessagePool . Acc.Tables;
            TreeNode txtRoot = new TreeNode("Dump_Detail_Table");
            treeView1.Nodes.Add(txtRoot);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
               string tb = dt.Rows[i]["TABLE_NAME"].ToString();
                TreeNode rootMessage = new TreeNode(tb);
                txtRoot.Nodes.Add(rootMessage);
            }
            //提取消息字段和数量
           dt = streamMessagePool.Acc.RunQuery("select message_info,count(*) c from softerCell_have_read"+
                " group by message_info order by 2 desc ");
            TreeNode infoRoot = new TreeNode("Info_Field_Table");
            treeView1.Nodes.Add(infoRoot);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tb = dt.Rows[i][0].ToString() + "," + dt.Rows[i][1].ToString();
                TreeNode rootMessage = new TreeNode(tb);
                infoRoot.Nodes.Add(rootMessage);
            }

            TreeNode dataRoot = new TreeNode("Data_Table_Field");
            treeView1.Nodes.Add(dataRoot);

            treeView1.ExpandAll();

            treeView1.SelectedNode = txtRoot;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Level == 0)
                {
                    if (e.Node.Text == "Data_Table_Field")
                    {

                        DataTable dt0 = streamMessagePool.Acc.GetColumns();
                        dataGridView1.DataSource = dt0.DefaultView;
                    }
                }
                if (e.Node.Level == 1)
                {

                    if (e.Node.Parent.Text == "Info_Field_Table")
                    {
                        DataTable dt = streamMessagePool.Acc.RunQuery("select * from dbo.softerCell_have_read where message_info like '"
                            + e.Node.Text.Substring(0, e.Node.Text.IndexOf(",")) + "%'");
                        {
                            dataGridView1.AutoGenerateColumns = true;
                            dataGridView1.DataSource = dt.DefaultView;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
                            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
                            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
                            dataGridView1.GridColor = Color.Blue;
                        }
                        richTextBox1.Text = dt.Rows.Count.ToString();
                    }

                    if (e.Node.Parent.Text == "Dump_Detail_Table")
                    {
                        DataTable dt = streamMessagePool.Acc.RunQuery("select * from " + e.Node.Text);
                        {
                            dataGridView1.AutoGenerateColumns = true;
                            dataGridView1.DataSource = dt.DefaultView;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
                            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
                            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
                            dataGridView1.GridColor = Color.Blue;
                        }
                        richTextBox1.Text = dt.Rows.Count.ToString();
                    }
                }

            }
            catch {    return;}
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //自动搜索指定的列名称的单元内容，这样改动datagridview的数据结构也不会影响关联计算
            int row = dataGridView1.CurrentCell.RowIndex;
            int fr=0;
            for (int i=0; i < dataGridView1.Columns.Count; i++)
            {
                if (dataGridView1.Columns[i].HeaderText.IndexOf("_frame") != -1)
                {
                    Int32.TryParse(dataGridView1.Rows[row].Cells[i].Value.ToString(), out  fr);
                    break;
                }
            }
            middleDelegate.DoSendAMessage(fr);
            middleDelegate.DoSendBMessage(fr);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = streamMessagePool.Acc.RunQuery(richTextBox1.Text);
                {
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt.DefaultView;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
                    dataGridView1.BorderStyle = BorderStyle.Fixed3D;
                    dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
                    dataGridView1.GridColor = Color.Blue;
                }
            }
            catch { return; }
        }

        private void kpiToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                kpiToExcel.ExportForDataGridview(dataGridView1, sender.ToString(), true);
            }
            catch { return; }
        }
    }
}
