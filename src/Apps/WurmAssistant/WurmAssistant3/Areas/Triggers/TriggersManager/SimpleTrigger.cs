using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    public class SimpleTrigger : SimpleConditionTriggerBase
    {
        public SimpleTrigger(TriggerEntity triggerEntity, ISoundManager soundManager, ITrayPopups trayPopups, IWurmApi wurmApi,
            ILogger logger)
            : base(triggerEntity, soundManager, trayPopups, wurmApi, logger)
        {
            ConditionHelp = "Text to match against log entry content, case insensitive";
            SourceHelp =
                "Test to match against log entry source. " + Environment.NewLine +
                "Source is the text between < >, for example game character than sent a PM. " + Environment.NewLine +
                "Case insensitive. " + Environment.NewLine +
                "Leave empty to match everything.";
        }

        protected override bool CheckCondition(LogEntry logMessage)
        {
            if (!string.IsNullOrWhiteSpace(TriggerEntity.Source) 
                && !string.Equals(logMessage.Source, TriggerEntity.Source, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (TriggerEntity.MatchEveryLine) return true;
            if (string.IsNullOrEmpty(TriggerEntity.Condition)) return false;
            return CheckCaseInsensitive(logMessage.Content);
        }

        private bool CheckCaseInsensitive(string logMessage)
        {
            return logMessage.Contains(TriggerEntity.Condition, StringComparison.OrdinalIgnoreCase);
        }

        public override string TypeAspect
        {
            get { return "Simple"; }
        }
    }
}
