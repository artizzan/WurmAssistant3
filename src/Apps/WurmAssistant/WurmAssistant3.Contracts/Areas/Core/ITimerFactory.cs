namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface ITimerFactory
    {
        ITimer CreateUiThreadTimer();
    }
}
