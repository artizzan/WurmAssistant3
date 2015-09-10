using System;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using AldursLab.WurmAssistant3.Core.Areas.Logging.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.ModuleManager.ViewModels;
using Caliburn.Micro;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Root.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        LogViewModel logViewModel;
        ModulesViewModel modulesViewModel;
        MenuViewModel menuViewModel;

        [UsedImplicitly]
        public LogViewModel LogViewModel
        {
            get { return logViewModel; }
            set
            {
                if (Equals(value, logViewModel)) return;
                logViewModel = value;
                NotifyOfPropertyChange(() => LogViewModel);
            }
        }

        [UsedImplicitly]
        public ModulesViewModel ModulesViewModel
        {
            get { return modulesViewModel; }
            set
            {
                if (Equals(value, modulesViewModel)) return;
                modulesViewModel = value;
                NotifyOfPropertyChange(() => ModulesViewModel);
            }
        }

        [UsedImplicitly]
        public MenuViewModel MenuViewModel
        {
            get { return menuViewModel; }
            set
            {
                if (Equals(value, menuViewModel)) return;
                menuViewModel = value;
                NotifyOfPropertyChange(() => MenuViewModel);
            }
        }
    }
}
