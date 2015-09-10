using System;
using System.Collections.Generic;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.ModuleManager.ViewModels
{
    public class ModuleManager { }

    public class ModulesViewModel : Screen
    {
        readonly ModuleManager moduleManager;
        IEnumerable<ModuleControl> moduleControlViewModels;

        public ModulesViewModel([NotNull] ModuleManager moduleManager)
        {
            if (moduleManager == null) throw new ArgumentNullException("moduleManager");
            this.moduleManager = moduleManager;

            var list = new List<ModuleControl>();
            //foreach (var m in moduleManager.Modules)
            //{
            //    var module = m;
            //    list.Add(new ModuleControl()
            //    {
            //        OpenAction = () => module.ShowGui(),
            //        Name = module.Name
            //    });
            //}
            ModuleControlViewModels = list;
        }

        public IEnumerable<ModuleControl> ModuleControlViewModels
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
