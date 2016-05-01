using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.Essentials
{
    /// <summary>
    /// Provides current time with possibility to change the provider.
    /// </summary>
    public class Time
    {
        private static Time _localTimeProvider;
        private static readonly object SyncLocker = new object();

        public static Time Get
        {
            get
            {
                if (_localTimeProvider != null)
                    return _localTimeProvider;
                else
                {
                    lock (SyncLocker)
                    {
                        if (_localTimeProvider == null)
                        {
                            _localTimeProvider = new Time();
                        }
                    }
                    return _localTimeProvider;
                }
            }
            private set
            {
                _localTimeProvider = value;
            }
        }

        /// <summary>
        /// Provider can be changed only, when no requests have been made to Time.Get
        /// </summary>
        /// <param name="timeProvider"></param>
        public static void SetProvider(Time timeProvider)
        {
            if (_localTimeProvider != null)
            {
                throw new InvalidOperationException("time provider already set");
            }
            Time.Get = timeProvider;
        }

        public virtual DateTime LocalNow { get { return DateTime.Now; } }
        public virtual DateTimeOffset LocalNowOffset { get { return DateTimeOffset.Now; } }
    }

    public class StubbableTime : Time
    {
        private bool OverrideActive { get; set; }
        private DateTime? NowOverride { get; set; }
        private DateTimeOffset? NowOffsetOverride { get; set; }

        private readonly object syncLocker = new object();

        [CanBeNull]
        public StubScope CurrentScope { get; private set; }


        public override DateTime LocalNow
        {
            get
            {
                if (NowOverride != null)
                {
                    return NowOverride.Value;
                }
                else
                {
                    return base.LocalNow;
                }
            }
        }

        public override DateTimeOffset LocalNowOffset
        {
            get
            {
                if (NowOffsetOverride != null)
                {
                    return NowOffsetOverride.Value;
                }
                else
                {
                    return base.LocalNowOffset;
                }
            }
        }

        public StubScope CreateStubbedScope()
        {
            lock (syncLocker)
            {
                if (CurrentScope != null)
                {
                    throw new InvalidOperationException("Another scope is already active, dispose of it first.");
                }
                else
                {
                    CurrentScope = new StubScope(this);
                }

                return CurrentScope;
            }
        }

        public class StubScope : IDisposable
        {
            private readonly StubbableTime clock;

            public StubScope(StubbableTime clock)
            {
                this.clock = clock;
            }

            public void Dispose()
            {
                Cleanup();
                GC.SuppressFinalize(this);
            }

            public void OverrideNow(DateTime overrideValue)
            {
                clock.NowOverride = overrideValue;
            }

            public void OverrideNowOffset(DateTimeOffset overrideValue)
            {
                clock.NowOffsetOverride = overrideValue;
            }

            public void AdvanceTime(double seconds)
            {
                OverrideNow(clock.LocalNow + TimeSpan.FromSeconds(seconds));
                OverrideNowOffset(clock.LocalNowOffset + TimeSpan.FromSeconds(seconds));
            }

            public void SetAllLocalTimes(DateTime dateTime)
            {
                OverrideNow(dateTime);
                OverrideNowOffset(dateTime);
            }

            ~StubScope()
            {
                Cleanup();
            }

            void Cleanup()
            {
                clock.NowOverride = null;
                clock.OverrideActive = false;
                clock.CurrentScope = null;
            }
        }
    }
}
