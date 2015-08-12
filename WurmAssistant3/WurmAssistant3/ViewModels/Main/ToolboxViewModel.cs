﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.ViewModels.Modules;

namespace AldursLab.WurmAssistant3.ViewModels.Main
{
    public class ToolboxViewModel
    {
        private readonly ObservableCollection<ModuleToolControlViewModel> tools = new ObservableCollection<ModuleToolControlViewModel>();

        public ToolboxViewModel([NotNull] ModuleToolControlViewModel[] moduleToolControlViewModels)
        {
            if (moduleToolControlViewModels == null) throw new ArgumentNullException("moduleToolControlViewModels");
            if (moduleToolControlViewModels.Any(model => model == null))
            {
                throw new ArgumentException("moduleToolControlViewModels array contains nulls");
            }

            foreach (var moduleViewModel in moduleToolControlViewModels)
            {
                tools.Add(moduleViewModel);
            }
        }

        public ObservableCollection<ModuleToolControlViewModel> Tools
        {
            get { return tools; }
        }
    }
}
