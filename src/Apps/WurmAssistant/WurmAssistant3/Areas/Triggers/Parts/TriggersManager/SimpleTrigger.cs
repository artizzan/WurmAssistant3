using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Data;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Parts.TriggersManager
{
    public class SimpleTrigger : SimpleConditionTriggerBase
    {
        public SimpleTrigger(TriggerData triggerData, ISoundManager soundManager, ITrayPopups trayPopups, IWurmApi wurmApi,
            ILogger logger)
            : base(triggerData, soundManager, trayPopups, wurmApi, logger)
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
            if (!string.IsNullOrWhiteSpace(TriggerData.Source) 
                && !string.Equals(logMessage.Source, TriggerData.Source, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (TriggerData.MatchEveryLine) return true;
            if (string.IsNullOrEmpty(TriggerData.Condition)) return false;
            return CheckCaseInsensitive(logMessage.Content);
        }

        private bool CheckCaseInsensitive(string logMessage)
        {
            return logMessage.Contains(TriggerData.Condition, StringComparison.OrdinalIgnoreCase);
        }

        public override string TypeAspect
        {
            get { return "Simple"; }
        }
    }
}
