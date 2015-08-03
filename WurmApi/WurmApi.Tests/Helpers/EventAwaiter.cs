using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AldurSoft.WurmApi.Tests.Helpers
{
    class EventAwaiter<TEventArgs> where TEventArgs : EventArgs
    {
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
                    if (invocations.Count() >= messageCount)
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