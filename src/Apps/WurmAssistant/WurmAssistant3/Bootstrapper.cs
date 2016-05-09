using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AldursLab.WurmAssistant3.Areas.Core.Views;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            
            System.Windows.Forms.Application.EnableVisualStyles();

            var mainView = new MainForm(e.Args);
            mainView.Closed += (o, args) => Application.Shutdown();
            mainView.Show();
        }
    }
}
