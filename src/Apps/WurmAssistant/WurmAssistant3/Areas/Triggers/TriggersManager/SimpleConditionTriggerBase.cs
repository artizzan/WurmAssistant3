using System.Collections.Generic;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    public abstract class SimpleConditionTriggerBase : TriggerBase
    {
        protected SimpleConditionTriggerBase(TriggerData triggerData, ISoundManager soundManager, ITrayPopups trayPopups, IWurmApi wurmApi,
            ILogger logger)
            : base(triggerData, soundManager, trayPopups, wurmApi, logger)
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
