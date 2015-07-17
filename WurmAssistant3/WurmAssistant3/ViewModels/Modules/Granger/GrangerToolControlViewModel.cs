using System;

using AldurSoft.WurmAssistant3.Modules.Granger;

using Core.AppFramework.Wpf.ViewModels;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.ViewModels.Modules.Granger
{
    public class GrangerToolControlViewModel : ModuleToolControlViewModel
    {
        private readonly IGrangerModule grangerModule;

        public GrangerToolControlViewModel([NotNull] IGrangerModule grangerModule)
        {
            if (grangerModule == null) throw new ArgumentNullException("grangerModule");
            this.grangerModule = grangerModule;
        }
    }
}
