using AldursLab.Deprec.Core;
using AldursLab.Deprec.Core.AppFramework.Wpf;

namespace AldursLab.WurmAssistant3.Systems
{
    public class TimerFactory : ITimerFactory
    {
        public ITimer Create()
        {
            return new DispatcherTimerProxy();
        }
    }
}
