using System;

namespace AldursLab.WurmApi.Modules.Events.Public
{
    interface IEventInvoker
    {
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