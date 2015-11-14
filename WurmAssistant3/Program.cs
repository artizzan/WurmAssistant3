using System;
using System.Windows.Forms;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant3.Core.Root;

namespace AldursLab.WurmAssistant3
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainView = new MainForm(args);
            Application.Run(mainView);
        }
    }
}
