using System;

namespace AldurSoft.Core
{
    /// <summary>
    /// Controls the time. Who said it's impossible?
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Gets the current local date and time.
        /// </summary>
        /// <value>
        /// Current local date and time.
        /// </value>
        DateTime LocalNow { get; }

        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        /// <value>
        /// Current UTC date and time.
        /// </value>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current local date and time.
        /// </summary>
        /// <value>
        /// Current local date and time.
        /// </value>
        DateTimeOffset LocalNowOffset { get; }
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        /// <value>
        /// Current UTC date and time.
        /// </value>
        DateTimeOffset UtcNowOffset { get; }
    }

    public class Clock : IClock
    {
        public DateTime LocalNow { get { return DateTime.Now; } }
        public DateTime UtcNow { get { return DateTime.UtcNow; } }
        public DateTimeOffset LocalNowOffset { get { return DateTimeOffset.Now; } }
        public DateTimeOffset UtcNowOffset { get { return DateTimeOffset.UtcNow; } }
    }

    public class MockableClock : IClock
    {
        private MockedScope scope = null;
        private object locker = new object();

        public DateTime LocalNow
        {
            get
            {
                lock (locker)
                {
                    if (scope == null)
                    {
                        return DateTime.Now;
                    }
                    else
                    {
                        return scope.LocalNow;
                    }
                }
            }
        }

        public DateTime UtcNow
        {
            get
            {
                lock (locker)
                {
                    if (scope == null)
                    {
                        return DateTime.UtcNow;
                    }
                    else
                    {
                        return scope.UtcNow;
                    }
                }
            }
        }

        public DateTimeOffset LocalNowOffset
        {
            get
            {
                lock (locker)
                {
                    if (scope == null)
                    {
                        return DateTimeOffset.Now;
                    }
                    else
                    {
                        return scope.LocalNowOffset;
                    }
                }
            }
        }

        public DateTimeOffset UtcNowOffset
        {
            get
            {
                lock (locker)
                {
                    if (scope == null)
                    {
                        return DateTimeOffset.UtcNow;
                    }
                    else
                    {
                        return scope.UtcNowOffset;
                    }
                }
            }
        }

        public MockedScope CreateScope()
        {
            lock (locker)
            {
                scope = new MockedScope(this, locker);
                return scope;
            }
        }

        public void ClearScope()
        {
            lock (locker)
            {
                scope.SetDisposed();
                scope = null;
            }
        }

        private void SetScope(MockedScope mockedScope)
        {
            lock (locker)
            {
                scope = mockedScope;
            }
        }

        public class MockedScope : IDisposable
        {
            private readonly MockableClock clock;
            private readonly object synclocker;
            private bool isDisposed = false;
            private DateTime _localNow;
            private DateTime _utcNow;
            private DateTimeOffset _localNowOffset;
            private DateTimeOffset _utcNowOffset;

            public MockedScope(MockableClock clock, object synclocker)
            {
                if (clock == null) throw new ArgumentNullException("clock");
                if (synclocker == null) throw new ArgumentNullException("synclocker");
                this.clock = clock;
                this.synclocker = synclocker;
                clock.SetScope(this);
            }

            public void SetAllLocalTimes(DateTime dateTime)
            {
                LocalNow = dateTime;
                LocalNowOffset = dateTime;
            }

            public DateTime LocalNow
            {
                internal get
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        return _localNow;
                    }
                }
                set
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        _localNow = value;
                    }
                }
            }

            public DateTime UtcNow
            {
                internal get
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        return _utcNow;
                    }
                }
                set
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        _utcNow = value;
                    }
                }
            }

            public DateTimeOffset LocalNowOffset
            {
                internal get
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        return _localNowOffset;
                    }
                }
                set
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        _localNowOffset = value;
                    }
                }
            }

            public DateTimeOffset UtcNowOffset
            {
                internal get
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        return _utcNowOffset;
                    }
                }
                set
                {
                    lock (synclocker)
                    {
                        VerifyNotDisposed();
                        _utcNowOffset = value;
                    }
                }
            }

            private void VerifyNotDisposed()
            {
                if (isDisposed)
                {
                    throw new InvalidOperationException("Attempted to set or get time value from disposed Scope.");
                }
            }

            public void Dispose()
            {
                lock (synclocker)
                {
                    isDisposed = true;
                    clock.ClearScope();
                }
            }

            internal void SetDisposed()
            {
                lock (synclocker)
                {
                    isDisposed = true;
                }
            }
        }
    }

    public static class Time
    {
        private static IClock clock;
        private static object locker = new object();
        private static volatile bool initialized = false;

        public static IClock Clock
        {
            get
            {
                if (!initialized)
                {
                    lock (locker)
                    {
                        if (!initialized)
                        {
                            clock = new Clock();
                            initialized = true;
                        }
                    }
                }
                return clock;
            }
            set
            {
                if (clock == null)
                {
                    lock (locker)
                    {
                        if (clock == null)
                        {
                            clock = value;
                            initialized = true;
                        }
                        else
                        {
                            throw new InvalidOperationException("Clock already set");
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("Clock already set");
                }
            }
        }
    }
}
