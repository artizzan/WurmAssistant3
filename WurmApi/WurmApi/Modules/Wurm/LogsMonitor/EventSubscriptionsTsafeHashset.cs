using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    class EventSubscriptionsTsafeHashset : IEnumerable<AllEventsSubscription>
    {
        readonly object locker = new object();
        readonly HashSet<AllEventsSubscription> handlers = new HashSet<AllEventsSubscription>();

        public IEnumerator<AllEventsSubscription> GetEnumerator()
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

        /// <summary>
        /// Returns true if added or false if subscription already added.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(AllEventsSubscription item)
        {
            lock (locker)
            {
                return handlers.Add(item);
            }
        }

        public bool Remove(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            lock (locker)
            {
                return handlers.RemoveWhere(subscription => subscription.EventHandler == eventHandler) > 0;
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