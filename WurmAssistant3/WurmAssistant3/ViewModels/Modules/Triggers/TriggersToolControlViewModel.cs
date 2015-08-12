using System;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Modules.Triggers;

namespace AldursLab.WurmAssistant3.ViewModels.Modules.Triggers
{
    public class TriggersToolControlViewModel : ModuleToolControlViewModel
    {
        private readonly ITriggersModule triggersModule;

        public TriggersToolControlViewModel([NotNull] ITriggersModule triggersModule)
        {
            if (triggersModule == null) throw new ArgumentNullException("triggersModule");
            this.triggersModule = triggersModule;
        }
    }
}
