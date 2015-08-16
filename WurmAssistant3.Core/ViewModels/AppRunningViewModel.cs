using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using Caliburn.Micro;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.ViewModels
{
    public class AppRunningViewModel : Screen
    {
        readonly IEnvironment environment;
        LogOutputViewModel logOutputViewModel;

        public AppRunningViewModel([NotNull] IEnvironment environment)
        {
            if (environment == null) throw new ArgumentNullException("environment");
            this.environment = environment;
        }

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

        public void RequestRestart()
        {
            environment.RequestRestart();
        }
    }
}
