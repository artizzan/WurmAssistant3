using System;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Modules.Granger;

namespace AldursLab.WurmAssistant3.ViewModels.Modules.Granger
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
