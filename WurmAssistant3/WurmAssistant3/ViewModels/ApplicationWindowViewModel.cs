using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.WurmAssistant3.ViewModels.Main;

using Caliburn.Micro;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.ViewModels
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
