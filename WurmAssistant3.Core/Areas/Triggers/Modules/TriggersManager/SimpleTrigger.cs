using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Data;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager
{
    //[DataContract]
    public class SimpleTrigger : SimpleConditionTriggerBase
    {
        public SimpleTrigger(TriggerData triggerData, ISoundEngine soundEngine, ITrayPopups trayPopups, IWurmApi wurmApi,
            ILogger logger)
            : base(triggerData, soundEngine, trayPopups, wurmApi, logger)
        {
            ConditionHelp = "Text to find in logs, case insensitive";
        }

        protected override bool CheckCondition(LogEntry logMessage)
        {
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
