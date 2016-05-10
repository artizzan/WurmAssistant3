using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Messages;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    public interface IEventBus
    {
        bool HandlerExistsFor(Type messageType);
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
        void Publish(Message message, Action<Action> marshal);
        void PublishOnUiThread(Message message);
    }
}
