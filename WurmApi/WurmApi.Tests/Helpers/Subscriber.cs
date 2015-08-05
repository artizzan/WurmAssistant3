using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Tests.Helpers
{
    class Subscriber<TMessage> : IHandle<TMessage> where TMessage : Message
    {
        readonly List<TMessage> receivedMessages = new List<TMessage>();
        readonly object locker = new object();

        public Subscriber(IInternalEventAggregator aggregator)
        {
            lock (locker)
            {
                aggregator.Subscribe(this);
            }
        }

        public IEnumerable<TMessage> ReceivedMessages
        {
            get 
            {
                lock (locker)
                {
                    return receivedMessages;
                }
            }
        }

        public void Handle(TMessage message)
        {
            lock (locker)
            {
                receivedMessages.Add(message);
            }
        }

        public void WaitMessages(int messageCount, int timeoutMillis = 5000)
        {
            WaitMessages(messageCount, null, timeoutMillis);
        }

        public void WaitMessages(int messageCount, [CanBeNull] Func<TMessage, bool> isMatch, int timeoutMillis = 5000)
        {
            int currentWait = 0;
            while (true)
            {
                var loopMillis = 10;
                Thread.Sleep(loopMillis);
                currentWait += loopMillis;
                lock (locker)
                {
                    if (ReceivedMessages.Count(message => isMatch == null || isMatch(message)) >= messageCount)
                    {
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
