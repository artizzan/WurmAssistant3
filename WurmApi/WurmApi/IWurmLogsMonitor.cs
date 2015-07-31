using System;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means of observing currently active wurm logs and obtain notificions of new events.
    /// </summary>
    public interface IWurmLogsMonitor
    {
        /// <summary>
        /// Subscribes to all live events matching character and log type.
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="logType"></param>
        /// <param name="eventHandler"></param>
        /// <exception cref="DataNotFoundException">Specified character does not exist or is unknown to WurmApi.</exception>
        void Subscribe(CharacterName characterName, LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler);

        /// <summary>
        /// Unsubscribes from <see cref="Subscribe"/>.
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="eventHandler"></param>
        void Unsubscribe(CharacterName characterName, EventHandler<LogsMonitorEventArgs> eventHandler);

        /// <summary>
        /// Subscribes to live events matching character and specific PM conversation.
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="pmRecipient"></param>
        /// <param name="eventHandler"></param>
        /// <exception cref="DataNotFoundException">Specified character does not exist or is unknown to WurmApi.</exception>
        void SubscribePm(
            CharacterName characterName,
            CharacterName pmRecipient,
            EventHandler<LogsMonitorEventArgs> eventHandler);

        /// <summary>
        /// Unsubscribes from <see cref="SubscribePm"/>.
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="pmRecipient"></param>
        /// <param name="eventHandler"></param>
        void UnsubscribePm(CharacterName characterName, CharacterName pmRecipient, EventHandler<LogsMonitorEventArgs> eventHandler);

        /// <summary>
        /// Subscribes to all events from all characters.
        /// </summary>
        /// <param name="eventHandler"></param>
        void SubscribeAllActive(EventHandler<LogsMonitorEventArgs> eventHandler);

        /// <summary>
        /// Unsubscribes from <see cref="SubscribeAllActive"/>.
        /// </summary>
        /// <param name="eventHandler"></param>
        void UnsubscribeAllActive(EventHandler<LogsMonitorEventArgs> eventHandler);

        /// <summary>
        /// Unsubscribes from all subscriptions.
        /// </summary>
        /// <param name="eventHandler"></param>
        void UnsubscribeFromAll(EventHandler<LogsMonitorEventArgs> eventHandler);
    }
}