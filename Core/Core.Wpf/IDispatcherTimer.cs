namespace AldurSoft.Core.Wpf
{
    /// <summary>
    /// Ticks must be synchronous, queueing but not stacking, dispatched to UI thread.
    /// </summary>
    public interface IDispatcherTimer : ITimer
    {
    }
}
