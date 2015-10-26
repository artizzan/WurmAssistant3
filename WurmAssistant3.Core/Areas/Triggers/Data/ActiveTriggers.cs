using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Contracts.ActionQueueParsing;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Data
{
    [PersistentObject("TriggersFeature_ActiveTriggers")]
    public class ActiveTriggers : PersistentObjectBase
    {
        readonly ISoundEngine soundEngine;
        readonly ITrayPopups trayPopups;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly IActionQueueConditions actionQueueConditions;

        [JsonProperty]
        private readonly Dictionary<Guid,TriggerData> triggerDatas = new Dictionary<Guid,TriggerData>();

        private readonly Dictionary<Guid,ITrigger> triggers = new Dictionary<Guid,ITrigger>();
        Func<bool> mutedEvaluator;

        public ActiveTriggers(string persistentObjectId, [NotNull] ISoundEngine soundEngine,
            [NotNull] ITrayPopups trayPopups, [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger,
            [NotNull] IActionQueueConditions actionQueueConditions) : base(persistentObjectId)
        {
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (actionQueueConditions == null) throw new ArgumentNullException("actionQueueConditions");
            this.soundEngine = soundEngine;
            this.trayPopups = trayPopups;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.actionQueueConditions = actionQueueConditions;
        }

        protected override void OnPersistentDataLoaded()
        {
            foreach (var settings in triggerDatas.Values)
            {
                settings.DataChanged += DataOnDataChanged;
                triggers.Add(settings.TriggerId, BuildTrigger(settings));
            }
        }

        public Func<bool> MutedEvaluator
        {
            get { return mutedEvaluator; }
            set
            {
                mutedEvaluator = value;
                foreach (var trigger in triggers.Values)
                {
                    trigger.MuteChecker = value;
                }
            }
        }

        ITrigger BuildTrigger(TriggerData settings)
        {
            switch (settings.TriggerKind)
            {
                case TriggerKind.Simple:
                    return new SimpleTrigger(settings, soundEngine, trayPopups, wurmApi, logger);
                case TriggerKind.Regex:
                    return new RegexTrigger(settings, soundEngine, trayPopups, wurmApi, logger);
                case TriggerKind.ActionQueue:
                    return new ActionQueueTrigger(settings, soundEngine, trayPopups, wurmApi, logger, actionQueueConditions);
                default:
                    throw new ApplicationException("Unknown trigger kind: " + settings.TriggerKind);
            }
        }

        public IEnumerable<ITrigger> All { get { return triggers.Values.ToArray(); } }

        public ITrigger CreateNewTrigger(TriggerKind kind)
        {
            var settings = new TriggerData(Guid.NewGuid(), kind);
            var trigger = BuildTrigger(settings);
            triggers.Add(settings.TriggerId, trigger);
            triggerDatas.Add(settings.TriggerId, settings);
            settings.DataChanged += DataOnDataChanged;
            return trigger;
        }

        public bool RemoveTrigger(ITrigger trigger)
        {
            TriggerData data;
            if (triggerDatas.TryGetValue(trigger.TriggerId, out data))
            {
                if (data != null) data.DataChanged -= DataOnDataChanged;
                triggers.Remove(trigger.TriggerId);
                triggerDatas.Remove(trigger.TriggerId);
                return true;
            }

            return false;
        }

        void DataOnDataChanged(object sender, EventArgs eventArgs)
        {
            FlagAsChanged();
        }
    }
}
