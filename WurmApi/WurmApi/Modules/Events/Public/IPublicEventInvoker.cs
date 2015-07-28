using System;

namespace AldurSoft.WurmApi.Modules.Events.Public
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
        /// Invokes the event instantly.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="handler"></param>
        /// <param name="source"></param>
        /// <param name="args"></param>
        void TriggerInstantly<TEventArgs>(EventHandler<TEventArgs> handler, object source, TEventArgs args) where TEventArgs : EventArgs;
    }
}
