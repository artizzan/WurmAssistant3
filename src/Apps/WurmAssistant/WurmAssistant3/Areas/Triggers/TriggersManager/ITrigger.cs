using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AldursLab.Persistence;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using AldursLab.WurmAssistant3.Areas.Triggers.Notifiers;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    public enum ThreeStateBool { True, False, Error }

    public interface ITrigger : IDisposable
    {
        Guid TriggerId { get; }
        TriggerKind TriggerKind { get; }

        void AddLogType(LogType type);
        void RemoveLogType(LogType type);
        /// <summary>
        /// Is log type monitored by this trigger
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CheckLogType(LogType type);
        /// <summary>
        /// Delegate to check if trigger holder is in muted state
        /// </summary>
        Func<bool> MuteChecker { set; } 
        string Name { get; set; }

        /// <summary>
        /// Happens in a constant loop
        /// </summary>
        /// <param name="dateTimeNow"></param>
        void FixedUpdate(DateTime dateTimeNow);
        /// <summary>
        /// Happens when new log messages arrive, when matching log types
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="dateTimeNow"></param>
        void Update(LogEntry logEntry, DateTime dateTimeNow);

        IEnumerable<INotifier> GetNotifiers();

        TimeSpan Cooldown{ get; set; }
        bool Active { get; set; }

        void AddNotifier(INotifier notifier);
        void RemoveNotifier(INotifier notifier);

        string LogTypesAspect { get; }
        string ConditionAspect { get; }
        string TypeAspect { get; }
        string CooldownRemainingAspect { get; }
        ThreeStateBool HasSoundAspect { get; }
        ThreeStateBool HasPopupAspect { get; }

        /// <summary>
        /// Should log type user choice be disabled for this trigger
        /// </summary>
        bool LogTypesLocked { get; }
        IEnumerable<ITriggerConfig> Configs { get; }

        Guid SoundId { get; set; }

        string PopupTitle { get; set; }
        string PopupContent { get; set; }
        bool StayUntilClicked { get; set; }
        int PopupDurationMillis { get; set; }

        EditTrigger ShowAndGetEditUi(Form parent);

        TriggerEntity GetTriggerEntityCopy(ISerializer serializer);

        string GetDescription();
    }

    public interface ITriggerConfig
    {
        UserControl ControlHandle { get; }
    }
}
