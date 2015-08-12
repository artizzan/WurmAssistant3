using AldursLab.Deprec.Core.AppFramework.Wpf.Attributes;
using AldursLab.Deprec.Core.AppFramework.Wpf.ViewModels;

namespace AldursLab.WurmAssistant3.ViewModels.Main
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
