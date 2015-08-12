using System;
using System.Collections.Generic;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Systems
{
    public class ScheduleEngine : IScheduleEngine, IDisposable
    {
        private readonly Dictionary<Action<ExecutionInfo>, CallbackContainer> callbacks =
            new Dictionary<Action<ExecutionInfo>, CallbackContainer>();
        private readonly ITimer timer;
        private bool stopped = false;

        public ScheduleEngine([NotNull] ITimerFactory timerFactory)
        {
            if (timerFactory == null)
                throw new ArgumentNullException("timerFactory");
            this.timer = timerFactory.Create();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += TimerOnTick;
            timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            foreach (var callbackContainer in callbacks.Values)
            {
                var dateNow = Time.Clock.LocalNow;
                if (callbackContainer.LastInvoke < dateNow - callbackContainer.MaximumFrequency)
                {
                    var timeSinceLastInvoke = dateNow - callbackContainer.LastInvoke;
                    callbackContainer.LastInvoke = dateNow;
                    callbackContainer.Action(new ExecutionInfo() { TimeSinceLastCallback = timeSinceLastInvoke });
                }
            }
        }

        public void RegisterForUpdates(
            TimeSpan maximumFrequency, 
            Action<ExecutionInfo> callback)
        {
            callbacks[callback] = new CallbackContainer()
            {
                Action = callback,
                LastInvoke = Time.Clock.LocalNow,
                MaximumFrequency = maximumFrequency
            };
        }

        class CallbackContainer
        {
            public Action<ExecutionInfo> Action { get; set; }
            public DateTime LastInvoke { get; set; }
            public TimeSpan MaximumFrequency { get; set; }
        }

        public void Dispose()
        {
            if (stopped)
            {
                return;
            }

            timer.Stop();
            timer.Tick -= TimerOnTick;
            stopped = true;
        }
    }
}
