using System;
using System.Windows.Forms;

namespace ThetisCore.Task
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
//          Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TaskMain.Init();

            Application.Run(/*new FrmMain()*/);
        }
    }
}
