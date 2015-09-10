using System;
using System.Windows.Forms;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant3.Core.Areas.Root.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.Root.Views;

namespace AldursLab.WurmAssistant3
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainView(new MainViewModel()));
            }
            catch (LockFailedException)
            {
                // if another app is running, we are silently quitting
                Application.Exit();
            }
        }
    }
}
