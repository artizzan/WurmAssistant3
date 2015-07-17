using System;

using AldurSoft.WurmAssistant3.Modules.Timers;

using Core.AppFramework.Wpf.ViewModels;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.ViewModels.Modules.Timers
{
    public class TimersToolControlViewModel : ModuleToolControlViewModel
    {
        private readonly ITimersModule timersModule;

        public TimersToolControlViewModel([NotNull] ITimersModule timersModule)
        {
            if (timersModule == null) throw new ArgumentNullException("timersModule");
            this.timersModule = timersModule;
        }
    }
}
