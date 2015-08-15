using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.ViewModels
{
    public class AppRunningViewModel : Screen
    {
        LogOutputViewModel logOutputViewModel;

        [Inject]
        public LogOutputViewModel LogOutputViewModel
        {
            get { return logOutputViewModel; }
            set
            {
                if (Equals(value, logOutputViewModel)) return;
                logOutputViewModel = value;
                NotifyOfPropertyChange(() => LogOutputViewModel);
            }
        }
    }
}
