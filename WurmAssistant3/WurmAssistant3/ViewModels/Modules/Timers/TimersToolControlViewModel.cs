using System;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Modules.Timers;

namespace AldursLab.WurmAssistant3.ViewModels.Modules.Timers
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
