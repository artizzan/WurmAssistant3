using System;

using AldurSoft.WurmAssistant3.Modules.Triggers;

using Core.AppFramework.Wpf.ViewModels;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.ViewModels.Modules.Triggers
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
