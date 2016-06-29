using System;
using System.Windows.Threading;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    class DispatcherTimerProxy : ITimer
    {
        readonly DispatcherTimer timer;

        public DispatcherTimerProxy()
        {
            timer = new DispatcherTimer();
        }

        public event EventHandler Tick
        {
            add { timer.Tick += value; }
            remove { timer.Tick -= value; }
        }

        public TimeSpan Interval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
