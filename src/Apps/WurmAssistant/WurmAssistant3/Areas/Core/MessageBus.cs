using System;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using Caliburn.Micro;
using Action = System.Action;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    class MessageBus : IMessageBus
    {
        readonly IEventAggregator eventAggregator;

        public bool HandlerExistsFor(Type messageType)
        {
            return eventAggregator.HandlerExistsFor(messageType);
        }

        public void Subscribe(object subscriber)
        {
            eventAggregator.Subscribe(subscriber);
        }

        public void Unsubscribe(object subscriber)
        {
            eventAggregator.Unsubscribe(subscriber);
        }

        public void Publish(Messages.Message message, Action<Action> marshal)
        {
            eventAggregator.Publish(message, marshal);
        }

        public void PublishOnUiThread(Messages.Message message)
        {
            eventAggregator.PublishOnUIThread(message);
        }

        public MessageBus()
        {
            eventAggregator = new EventAggregator();
        }
    }
}
