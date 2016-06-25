using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Core.Parts
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
