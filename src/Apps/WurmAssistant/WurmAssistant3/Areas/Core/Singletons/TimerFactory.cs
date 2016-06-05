using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Parts;

namespace AldursLab.WurmAssistant3.Areas.Core.Singletons
{
    class TimerFactory : ITimerFactory
    {
        public ITimer CreateUiThreadTimer()
        {
            return new DispatcherTimerProxy();
        }
    }
}
