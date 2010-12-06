using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace softerCell_U3_v0._01
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new softerCell());
            //Application.Run(new allMessageFlow());
            //Application.Run(new msMessageFlow());
            //Application.Run(new messageParameterList()); 

        }
    }
}
