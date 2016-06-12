using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Contracts.ActionQueueParsing;
using AldursLab.WurmAssistant3.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Areas.Triggers.Parts.TriggersManager;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Services
{
    [PersistentObject("TriggersFeature_ActiveTriggers")]
    public class ActiveTriggers : PersistentObjectBase
    {
        readonly ISoundManager soundManager;
        readonly ITrayPopups trayPopups;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly IActionQueueConditions actionQueueConditions;

        [JsonProperty]
        private readonly Dictionary<Guid,TriggerData> triggerDatas = new Dictionary<Guid,TriggerData>();

        private readonly Dictionary<Guid,ITrigger> triggers = new Dictionary<Guid,ITrigger>();
        Func<bool> mutedEvaluator;

        public ActiveTriggers(string persistentObjectId, [NotNull] ISoundManager soundManager,
            [NotNull] ITrayPopups trayPopups, [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger,
            [NotNull] IActionQueueConditions actionQueueConditions) : base(persistentObjectId)
        {
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (actionQueueConditions == null) throw new ArgumentNullException("actionQueueConditions");

            CharacterName = persistentObjectId;

            this.soundManager = soundManager;
            this.trayPopups = trayPopups;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.actionQueueConditions = actionQueueConditions;
        }

        public string CharacterName { get; private set; }

        protected override void OnPersistentDataLoaded()
        {
            foreach (var settings in triggerDatas.Values)
            {
                settings.DataChanged += DataOnDataChanged;
                try
                {
                    triggers.Add(settings.TriggerId, BuildTrigger(settings));
                }
                catch (Exception exception)
                {
                    logger.Error(exception,
                        string.Format("Error initializing trigger id {0}, name: {1}", settings.TriggerId, settings.Name));
                }
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
                    return new SimpleTrigger(settings, soundManager, trayPopups, wurmApi, logger);
                case TriggerKind.Regex:
                    return new RegexTrigger(settings, soundManager, trayPopups, wurmApi, logger);
                case TriggerKind.ActionQueue:
                    return new ActionQueueTrigger(settings, soundManager, trayPopups, wurmApi, logger, actionQueueConditions);
                case TriggerKind.SkillLevel:
                    return new SkillLevelTrigger(CharacterName, settings, soundManager, trayPopups, wurmApi, logger);
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
            FlagAsChanged();
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
                trigger.Dispose();
                FlagAsChanged();
                return true;
            }

            return false;
        }

        void DataOnDataChanged(object sender, EventArgs eventArgs)
        {
            FlagAsChanged();
        }

        public void DisposeAll()
        {
            foreach (var trigger in All)
            {
                trigger.Dispose();
            }
        }
    }
}
