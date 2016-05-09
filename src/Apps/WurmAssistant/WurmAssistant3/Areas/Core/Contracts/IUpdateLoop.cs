using System;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    public interface IUpdateLoop
    {
        /// <summary>
        /// Triggered regularly in 500 ms intervals, always on UI thread.
        /// </summary>
        event EventHandler<EventArgs> Updated;
    }
}
