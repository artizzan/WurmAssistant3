using System;
using System.Windows.Threading;

namespace AldursLab.Deprec.Core.AppFramework.Wpf
{
    public class DispatcherTimerProxy : ITimer
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public event EventHandler Tick
        {
            add { timer.Tick += value; }
            remove { timer.Tick -= value; }
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public TimeSpan Interval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }
    }
}
