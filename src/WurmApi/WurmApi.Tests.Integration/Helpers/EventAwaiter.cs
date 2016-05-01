using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Tests.Integration.Helpers
{
    class EventAwaiter<TEventArgs> where TEventArgs : EventArgs
    {
        EventHandler<TEventArgs> eventHandler;

        readonly List<TEventArgs> invocations = new List<TEventArgs>();
        readonly object locker = new object();

        public IEnumerable<TEventArgs> Invocations
        {
            get
            {
                lock (locker)
                {
                    return invocations;
                }
            }
        }

        public void Handle(object sender, TEventArgs eventArgs)
        {
            lock (locker)
            {
                invocations.Add(eventArgs);
            }
        }

        public EventHandler<TEventArgs> GetEventHandler()
        {
            if (eventHandler == null)
            {
                eventHandler = Handle;
            }
            return eventHandler;
        }

        public void WaitUntilMatch([NotNull] Func<List<TEventArgs>, bool> isEventArgsMatch, int timeoutMillis = 5000)
        {
            if (isEventArgsMatch == null) throw new ArgumentNullException(nameof(isEventArgsMatch));

            int currentWait = 0;
            while (true)
            {
                var loopMillis = 10;
                Thread.Sleep(loopMillis);
                currentWait += loopMillis;
                lock (locker)
                {
                    if (isEventArgsMatch(invocations))
                    {
                        //Trace.WriteLine("WaitInvocations:" + DateTime.Now.ToString("O"));
                        return;
                    }
                }
                if (currentWait > timeoutMillis)
                {
                    throw new Exception("timeout");
                }
            }
        }

        public void WaitInvocations(int messageCount, int timeoutMillis = 5000)
        {
            int currentWait = 0;
            while (true)
            {
                var loopMillis = 10;
                Thread.Sleep(loopMillis);
                currentWait += loopMillis;
                lock (locker)
                {
                    if (invocations.Count >= messageCount)
                    {
                        //Trace.WriteLine("WaitInvocations:" + DateTime.Now.ToString("O"));
                        return;
                    }
                }
                if (currentWait > timeoutMillis)
                {
                    throw new Exception("timeout");
                }
            }
        }
    }
}