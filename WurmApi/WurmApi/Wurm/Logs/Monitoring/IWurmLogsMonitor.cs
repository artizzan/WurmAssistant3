using System;
using AldurSoft.WurmApi.Wurm.Characters;

namespace AldurSoft.WurmApi.Wurm.Logs.Monitoring
{
    /// <summary>
    /// Provides means of observing currently active wurm logs and obtain notificions of new events.
    /// </summary>
    public interface IWurmLogsMonitor
    {
        void Subscribe(CharacterName characterName, LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler);

        void Unsubscribe(EventHandler<LogsMonitorEventArgs> eventHandler);

        void SubscribePm(
            CharacterName characterName,
            string pmRecipient,
            EventHandler<LogsMonitorEventArgs> eventHandler);

        void UnsubscribePm(EventHandler<LogsMonitorEventArgs> eventHandler);

        void SubscribeAll(EventHandler<LogsMonitorEventArgs> eventHandler);

        void UnsubscribeAll(EventHandler<LogsMonitorEventArgs> eventHandler);
    }
}