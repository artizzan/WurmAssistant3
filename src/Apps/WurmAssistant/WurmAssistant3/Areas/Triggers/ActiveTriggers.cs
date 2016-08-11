using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.ActionQueueParsing;
using AldursLab.WurmAssistant3.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers
{
    [KernelBind]
    public class ActiveTriggers
    {
        readonly ISoundManager soundManager;
        readonly ITrayPopups trayPopups;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly IActionQueueConditions actionQueueConditions;
        readonly CharacterTriggersConfig triggersConfig;

        private readonly Dictionary<Guid,ITrigger> triggers = new Dictionary<Guid,ITrigger>();
        Func<bool> mutedEvaluator;

        public ActiveTriggers(
            string characterName, 
            [NotNull] ISoundManager soundManager,
            [NotNull] ITrayPopups trayPopups, 
            [NotNull] IWurmApi wurmApi, 
            [NotNull] ILogger logger,
            [NotNull] IActionQueueConditions actionQueueConditions,
            [NotNull] TriggersDataContext triggersDataContext)
        {
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (actionQueueConditions == null) throw new ArgumentNullException(nameof(actionQueueConditions));
            if (triggersDataContext == null) throw new ArgumentNullException(nameof(triggersDataContext));

            CharacterName = characterName;

            this.soundManager = soundManager;
            this.trayPopups = trayPopups;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.actionQueueConditions = actionQueueConditions;
            this.triggersConfig = triggersDataContext.CharacterTriggersConfigs.GetOrCreate(characterName);

            foreach (var entity in triggersConfig.TriggerEntities.Values)
            {
                try
                {
                    triggers.Add(entity.TriggerId, BuildTrigger(entity));
                }
                catch (Exception exception)
                {
                    logger.Error(exception,
                        string.Format("Error initializing trigger id {0}, name: {1}", entity.TriggerId, entity.Name));
                }
            }
        }

        public string CharacterName { get; private set; }

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

        ITrigger BuildTrigger(TriggerEntity settings)
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
            var entity = new TriggerEntity(Guid.NewGuid(), kind);
            return CreateNewTriggerFromEntity(entity);
        }

        public ITrigger CreateNewTriggerFromEntity(TriggerEntity entity)
        {
            if (triggers.ContainsKey(entity.TriggerId))
            {
                throw new InvalidOperationException(
                    $"Trigger with ID {entity.TriggerId} already exists for player {CharacterName}.");
            }

            var trigger = BuildTrigger(entity);
            triggers.Add(entity.TriggerId, trigger);
            triggersConfig.TriggerEntities.Add(entity.TriggerId, entity);
            return trigger;
        }


        public bool RemoveTrigger(ITrigger trigger)
        {
            TriggerEntity entity;
            if (triggersConfig.TriggerEntities.TryGetValue(trigger.TriggerId, out entity))
            {
                triggers.Remove(trigger.TriggerId);
                triggersConfig.TriggerEntities.Remove(trigger.TriggerId);
                trigger.Dispose();
                return true;
            }

            return false;
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
