using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace softerCell_U3_v0._01
{
    class saveTreeDisplay
    {
        public static void saveTree(TreeView tv)
        {
            string path = streamMessagePool.appPath + "\\saveTree\\" + tv.Nodes[0].Text;
            File.Delete(path );
            TreeNodeCollection tc = tv.Nodes;
            selectNode(tv,path,tc);
            MessageBox.Show("OK");
        }

        private static void selectNode(TreeView tv, string path, TreeNodeCollection tc)  //运用递归过程遍历treeView的所有的节点
        {
            foreach (TreeNode TNode in tc)
            {
                tv.SelectedNode = TNode;//treeView选中事件
                File.AppendAllText(path , TNode.Text + Environment.NewLine);
                selectNode(tv,path,TNode.Nodes);
            }
        }
    }
}
