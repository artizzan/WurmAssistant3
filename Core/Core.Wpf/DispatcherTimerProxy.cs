using System;
using System.Windows.Threading;

namespace AldurSoft.Core.Wpf
{
    /// <summary>
    /// ITimer implementation using DispatcherTimer
    /// </summary>
    public class DispatcherTimerProxy : IDispatcherTimer
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// Occurs when timer ticks.
        /// </summary>
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
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }
    }
}
