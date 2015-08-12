using System;

namespace AldursLab.Deprec.Core
{
    /// <summary>
    /// Basic timer abstraction.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Occurs when timer ticks.
        /// </summary>
        event EventHandler Tick;
        /// <summary>
        /// Starts this timer.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops this timer.
        /// </summary>
        void Stop();
        /// <summary>
        /// Gets or sets the timer interval.
        /// </summary>
        /// <value>
        /// The timer interval.
        /// </value>
        TimeSpan Interval { get; set; }
    }

    public class TimerMock : ITimer
    {
        private bool enabled = false;

        public void InvokeTick()
        {
            if (enabled)
            {
                OnTick();
            }
        }

        public event EventHandler Tick;

        protected virtual void OnTick()
        {
            EventHandler handler = Tick;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void Start()
        {
            enabled = true;
        }

        public void Stop()
        {
            enabled = false;
        }

        public TimeSpan Interval { get; set; }
    }
}
