using System;

namespace AldursLab.WurmAssistant3.Core.Root.Contracts
{
    public interface IUpdateLoop
    {
        /// <summary>
        /// Interval for this event should be 500 ms.
        /// Events should always be triggered on UI thread.
        /// </summary>
        event EventHandler<EventArgs> Updated;
    }
}
