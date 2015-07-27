using System;
using AldurSoft.WurmApi.Modules.Events.Messages;

#if NETFX_CORE && !WinRT
#define WinRT
#endif

namespace AldurSoft.WurmApi.Modules.Events {
    /// <summary>
    ///   A marker interface for classes that subscribe to messages.
    /// </summary>
    interface IHandle { }

    /// <summary>
    ///   Denotes a class which can handle a particular type of message.
    /// </summary>
    /// <typeparam name = "TMessage">The type of message to handle.</typeparam>
    interface IHandle<TMessage> : IHandle where TMessage : Message //don't use contravariance here
    {  
        /// <summary>
        ///   Handles the message.
        /// </summary>
        /// <param name = "message">The message.</param>
        void Handle(TMessage message);
    }

    /// <summary>
    ///   Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    interface IInternalEventAggregator 
    {
        /// <summary>
        ///   Gets or sets the default publication thread marshaller.
        /// </summary>
        /// <value>
        ///   The default publication thread marshaller.
        /// </value>
        Action<System.Action> PublicationThreadMarshaller { get; set; }

        /// <summary>
        /// Searches the subscribed handlers to check if we have a handler for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        bool HandlerExistsFor(Type messageType);

        /// <summary>
        ///   Subscribes an instance to all events declared through implementations of <see cref = "IHandle{TMessage}" />
        /// </summary>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        void Subscribe(object subscriber);

        /// <summary>
        ///   Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "subscriber">The instance to unsubscribe.</param>
        void Unsubscribe(object subscriber);

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <remarks>
        ///   Uses the default thread marshaller during publication.
        /// </remarks>
        void Send(Message message);

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        void Send(Message message, Action<System.Action> marshal);
    }
}
