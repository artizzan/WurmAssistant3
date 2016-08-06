using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    //[DataContract]
    public class RegexTrigger : SimpleConditionTriggerBase
    {
        readonly ILogger logger;

        public RegexTrigger(TriggerEntity triggerEntity, ISoundManager soundManager, ITrayPopups trayPopups, IWurmApi wurmApi,
            ILogger logger)
            : base(triggerEntity, soundManager, trayPopups, wurmApi, logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
            ConditionHelp = "C# regular expression pattern to match against log entry content.";
            SourceHelp =
                "C# regular expression pattern to match against log entry source. " + Environment.NewLine +
                "Source is the text between < >, for example game character than sent a PM. " + Environment.NewLine +
                "Leave empty to match everything.";
        }

        protected override bool CheckCondition(LogEntry logMessage)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TriggerEntity.Source))
                {
                    if (!Regex.IsMatch(logMessage.Source, TriggerEntity.Source))
                    {
                        return false;
                    }
                }

                if (TriggerEntity.MatchEveryLine) return true;

                if (string.IsNullOrEmpty(TriggerEntity.Condition))
                {
                    return false;
                }

                return Regex.IsMatch(logMessage.Content, TriggerEntity.Condition);
            }
            catch (Exception exception)
            {
                logger.Error(exception,
                    string.Format(
                        "Exception while checking regex trigger condition. Trigger name: {0}, Condition: {1}",
                        Name,
                        TriggerEntity.Condition));
                return false;
            }
        }

        public override string TypeAspect
        {
            get { return "Regex"; }
        }

        public override IEnumerable<ITriggerConfig> Configs
        {
            get { return new List<ITriggerConfig>(base.Configs) { new RegexTriggerConfig(this, logger) }; }
        }
    }
}
