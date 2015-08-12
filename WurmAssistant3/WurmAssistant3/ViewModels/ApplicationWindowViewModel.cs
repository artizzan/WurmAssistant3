using System;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.ViewModels.Main;
using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.ViewModels
{
    public class ApplicationWindowViewModel : Screen
    {
        private readonly MainViewModel mainViewModel;

        public ApplicationWindowViewModel([NotNull] MainViewModel mainViewModel)
        {
            if (mainViewModel == null) throw new ArgumentNullException("mainViewModel");
            this.mainViewModel = mainViewModel;
        }

        public MainViewModel MainContent
        {
            get { return mainViewModel; }
        }
    }
}
