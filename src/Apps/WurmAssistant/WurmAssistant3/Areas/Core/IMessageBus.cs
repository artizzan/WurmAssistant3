using System;
using AldursLab.WurmAssistant3.Messages;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface IMessageBus
    {
        bool HandlerExistsFor(Type messageType);
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
        void Publish(Message message, Action<Action> marshal);
        void PublishOnUiThread(Message message);
    }
}
