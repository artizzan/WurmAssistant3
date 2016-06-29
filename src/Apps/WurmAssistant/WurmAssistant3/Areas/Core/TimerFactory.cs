namespace AldursLab.WurmAssistant3.Areas.Core
{
    [KernelBind(BindingHint.Singleton)]
    class TimerFactory : ITimerFactory
    {
        public ITimer CreateUiThreadTimer()
        {
            return new DispatcherTimerProxy();
        }
    }
}
