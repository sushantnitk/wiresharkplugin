using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CSV_import_export
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmImport f = new frmImport();
			f.MdiParent = this;
			f.Show();
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmExport f = new frmExport();
			f.MdiParent = this;
			f.Show();
		}
	}
}