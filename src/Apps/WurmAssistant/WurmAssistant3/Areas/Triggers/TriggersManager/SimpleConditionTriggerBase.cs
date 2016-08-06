using System.Collections.Generic;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    public abstract class SimpleConditionTriggerBase : TriggerBase
    {
        protected SimpleConditionTriggerBase(TriggerEntity triggerEntity, ISoundManager soundManager, ITrayPopups trayPopups, IWurmApi wurmApi,
            ILogger logger)
            : base(triggerEntity, soundManager, trayPopups, wurmApi, logger)
        {
        }

        public string ConditionHelp { get; set; }
        public string SourceHelp { get; set; }

        public override string ConditionAspect
        {
            get { return TriggerEntity.Condition; }
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
            get { return TriggerEntity.Condition; }
            set { TriggerEntity.Condition = value; }
        }

        public bool MatchEveryLine
        {
            get { return TriggerEntity.MatchEveryLine; }
            set { TriggerEntity.MatchEveryLine = value; }
        }

        public string Source
        {
            get { return TriggerEntity.Source; }
            set { TriggerEntity.Source = value; }
        }
    }
}
