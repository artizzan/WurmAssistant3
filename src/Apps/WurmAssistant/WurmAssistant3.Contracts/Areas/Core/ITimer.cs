using System;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface ITimer
    {
        event EventHandler Tick;

        TimeSpan Interval { get; set; }

        void Start();

        void Stop();
    }
}
