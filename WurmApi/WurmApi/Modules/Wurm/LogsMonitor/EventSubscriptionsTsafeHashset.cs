using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    class EventSubscriptionsTsafeHashset : IEnumerable<EventHandler<LogsMonitorEventArgs>>
    {
        readonly object locker = new object();
        readonly HashSet<EventHandler<LogsMonitorEventArgs>> handlers = new HashSet<EventHandler<LogsMonitorEventArgs>>();

        public IEnumerator<EventHandler<LogsMonitorEventArgs>> GetEnumerator()
        {
            lock (locker)
            {
                return handlers.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Add(EventHandler<LogsMonitorEventArgs> item)
        {
            lock (locker)
            {
                return handlers.Add(item);
            }
        }

        public bool Remove(EventHandler<LogsMonitorEventArgs> item)
        {
            lock (locker)
            {
                return handlers.Remove(item);
            }
        }

        public bool Any
        {
            get
            {
                lock (locker)
                {
                    return handlers.Any();
                }
            }
        }
    }
}