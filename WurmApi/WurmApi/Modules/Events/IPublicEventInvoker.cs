using System;
using System.Collections.Generic;
using System.Text;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Events
{
    interface IPublicEventInvoker
    {
        /// <summary>
        /// Creates a new handle to event publication.
        /// </summary>
        /// <param name="action">Action to execute upon publication.</param>
        /// <param name="invocationMinDelay">Minimum delay between invocations</param>
        /// <returns></returns>
        PublicEvent Create(Action action, TimeSpan invocationMinDelay);

        /// <summary>
        /// Flags the event as pending. Will be invoked according to scheduling parameters.
        /// </summary>
        /// <param name="publicEvent"></param>
        void Signal(PublicEvent publicEvent);
    }

    class PublicEvent
    {
        public Action Action { get; private set; }

        public PublicEvent(Action action)
        {
            Action = action;
        }
    }
}
