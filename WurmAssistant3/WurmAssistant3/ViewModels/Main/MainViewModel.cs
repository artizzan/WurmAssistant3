using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.WurmAssistant3.Modules.Calendar;

using Caliburn.Micro;

using Core.AppFramework.Wpf.Attributes;
using Core.AppFramework.Wpf.ViewModels;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.ViewModels.Main
{
    [GlobalViewModel]
    public class MainViewModel : ViewModelBase
    {
        private readonly ToolboxViewModel toolboxViewModel;

        public MainViewModel(ToolboxViewModel toolboxViewModel)
        {
            this.toolboxViewModel = toolboxViewModel;
        }

        public ToolboxViewModel ToolboxViewModel
        {
            get { return toolboxViewModel; }
        }
    }
}
