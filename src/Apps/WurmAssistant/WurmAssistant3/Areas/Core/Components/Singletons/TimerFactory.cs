using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Parts;

namespace AldursLab.WurmAssistant3.Areas.Core.Components.Singletons
{
    class TimerFactory : ITimerFactory
    {
        public ITimer CreateUiThreadTimer()
        {
            return new DispatcherTimerProxy();
        }
    }
}
