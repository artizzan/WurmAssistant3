using System;
using System.Collections.Generic;
using AldursLab.WurmAssistant3.Core.Infrastructure.Modules;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.ViewModels.ModuleManagement
{
    public class ModuleManagerViewModel : Screen
    {
        readonly ModuleManager moduleManager;
        IEnumerable<ModuleControlViewModel> moduleControlViewModels;

        public ModuleManagerViewModel([NotNull] ModuleManager moduleManager)
        {
            if (moduleManager == null) throw new ArgumentNullException("moduleManager");
            this.moduleManager = moduleManager;

            var list = new List<ModuleControlViewModel>();
            foreach (var m in moduleManager.Modules)
            {
                var module = m;
                list.Add(new ModuleControlViewModel()
                {
                    OpenAction = () => module.ShowGui(),
                    Name = module.Name
                });
            }
            ModuleControlViewModels = list;
        }

        public IEnumerable<ModuleControlViewModel> ModuleControlViewModels
        {
            get { return moduleControlViewModels; }
            private set
            {
                if (Equals(value, moduleControlViewModels)) return;
                moduleControlViewModels = value;
                NotifyOfPropertyChange(() => ModuleControlViewModels);
            }
        }
    }
}
