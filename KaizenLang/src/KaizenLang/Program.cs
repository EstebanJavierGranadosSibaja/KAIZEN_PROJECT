
using System;
using System.Windows.Forms;
using KaizenLang.UI;

namespace ParadigmasLang
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
