using System.Collections.Generic;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.TriggersManager;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager
{
    public abstract class SimpleConditionTriggerBase : TriggerBase
    {
        protected SimpleConditionTriggerBase(TriggerData triggerData, ISoundEngine soundEngine, ITrayPopups trayPopups, IWurmApi wurmApi,
            ILogger logger)
            : base(triggerData, soundEngine, trayPopups, wurmApi, logger)
        {
        }

        public string ConditionHelp { get; set; }
        public string SourceHelp { get; set; }

        public override string ConditionAspect
        {
            get { return TriggerData.Condition; }
        }

        public override IEnumerable<ITriggerConfig> Configs
        {
            get
            {
                return new List<ITriggerConfig>(base.Configs) { new SimpleConditionTriggerBaseConfig(this) };
            }
        }

        public string Condition
        {
            get { return TriggerData.Condition; }
            set { TriggerData.Condition = value; }
        }

        public bool MatchEveryLine
        {
            get { return TriggerData.MatchEveryLine; }
            set { TriggerData.MatchEveryLine = value; }
        }

        public string Source
        {
            get { return TriggerData.Source; }
            set { TriggerData.Source = value; }
        }
    }
}
