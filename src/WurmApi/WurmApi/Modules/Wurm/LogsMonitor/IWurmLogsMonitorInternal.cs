using System;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    interface IWurmLogsMonitorInternal
    {
        void SubscribeInternal(CharacterName characterName, LogType logType,
            EventHandler<LogsMonitorEventArgs> eventHandler);

        void Unsubscribe(string characterName, EventHandler<LogsMonitorEventArgs> eventHandler);

        void SubscribeAllActiveInternal(EventHandler<LogsMonitorEventArgs> eventHandler);

        void UnsubscribeAllActive(EventHandler<LogsMonitorEventArgs> eventHandler);

        void UnsubscribeFromAll(EventHandler<LogsMonitorEventArgs> eventHandler);
    }
}