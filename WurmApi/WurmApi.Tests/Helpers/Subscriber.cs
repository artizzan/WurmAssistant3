using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;

namespace AldurSoft.WurmApi.Tests.Helpers
{
    class Subscriber<TMessage> : IHandle<TMessage> where TMessage : Message
    {
        readonly List<TMessage> receivedMessages = new List<TMessage>();

        public Subscriber(IInternalEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
        }

        public IEnumerable<TMessage> ReceivedMessages
        {
            get { return receivedMessages; }
        }

        public void Handle(TMessage message)
        {
            receivedMessages.Add(message);
        }
    }
}
