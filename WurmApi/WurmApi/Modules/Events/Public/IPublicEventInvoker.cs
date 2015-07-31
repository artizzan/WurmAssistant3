using System;

namespace AldurSoft.WurmApi.Modules.Events.Public
{
    interface IPublicEventInvoker : IEventInvoker
    {
        /// <summary>
        /// Creates a new handle to event publication.
        /// </summary>
        /// <param name="action">Action to execute upon publication.</param>
        /// <param name="invocationMinDelay">Minimum delay between invocations</param>
        /// <returns></returns>
        PublicEvent Create(Action action, TimeSpan invocationMinDelay);
    }
}
