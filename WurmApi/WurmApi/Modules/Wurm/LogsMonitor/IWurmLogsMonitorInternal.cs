using System;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    internal interface IWurmLogsMonitorInternal
    {
        void SubscribeInternal(CharacterName characterName, LogType logType,
            EventHandler<LogsMonitorEventArgs> eventHandler);

        void Unsubscribe(CharacterName characterName, EventHandler<LogsMonitorEventArgs> eventHandler);

        void SubscribeAllActiveInternal(EventHandler<LogsMonitorEventArgs> eventHandler);

        void UnsubscribeAllActive(EventHandler<LogsMonitorEventArgs> eventHandler);

        void UnsubscribeFromAll(EventHandler<LogsMonitorEventArgs> eventHandler);
    }
}