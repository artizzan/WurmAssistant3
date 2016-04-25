using System;
using System.Threading;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.JobRunning
{
    class TaskHandle
    {
        readonly Action action;
        DateTimeOffset lastInvoke = DateTimeOffset.MinValue;
        int triggered;
        DateTimeOffset lastError = DateTimeOffset.MinValue;

        static readonly TimeSpan ErrorMinimumRetryDelay = TimeSpan.FromMilliseconds(500);

        public TaskHandle([NotNull] Action action, [NotNull] string description, TimeSpan? minimumDelay = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (description == null) throw new ArgumentNullException(nameof(description));
            this.action = action;
            MinimumDelay = minimumDelay.HasValue ? MinimumDelay : TimeSpan.Zero;
            Description = description;
        }

        internal void TryExecute()
        {
            var now = Time.Get.LocalNowOffset;
            if (triggered == 1 && lastInvoke <= now - MinimumDelay && lastError <= now - ErrorMinimumRetryDelay)
            {
                Interlocked.Exchange(ref triggered, 0);
                try
                {
                    action();
                    lastInvoke = now;
                }
                catch (Exception)
                {
                    // if action failed, retry on next run
                    Interlocked.Exchange(ref triggered, 1);
                    lastError = Time.Get.LocalNowOffset;
                    throw;
                }
            }
        }

        public TimeSpan MinimumDelay { get; private set; }
        public string Description { get; private set; }

        public void Trigger()
        {
            Interlocked.Exchange(ref triggered, 1);
        }

        public void SetErrorAndRetrigger()
        {
            lastError = Time.Get.LocalNowOffset;
            Trigger();
        }
    }
}