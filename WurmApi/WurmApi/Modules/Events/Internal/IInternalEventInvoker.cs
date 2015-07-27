using System;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;

namespace AldurSoft.WurmApi.Modules.Events.Internal
{
    interface IInternalEventInvoker
    {
        /// <summary>
        /// Creates a new handle to event publication.
        /// </summary>
        /// <param name="messageFactory">Factory of messages to be sent on publication.</param>
        /// <returns></returns>
        InternalEvent Create(Func<Message> messageFactory);

        /// <summary>
        /// Creates a new handle to event publication.
        /// </summary>
        /// <param name="messageFactory">Factory of messages to be sent on publication.</param>
        /// <param name="invocationMinDelay">Minimum delay between invocations</param>
        /// <returns></returns>
        InternalEvent Create(Func<Message> messageFactory, TimeSpan invocationMinDelay);
    }
}