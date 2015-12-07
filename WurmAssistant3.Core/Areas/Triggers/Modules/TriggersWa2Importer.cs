using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Views;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules
{
    public class TriggersWa2Importer
    {
        private readonly ISoundManager soundManager;
        private readonly ITrayPopups trayPopups;
        private readonly Dictionary<string, TriggerManager> triggerManagers;
        private readonly ILogger logger;

        public TriggersWa2Importer(ISoundManager soundManager, ITrayPopups trayPopups,
            Dictionary<string, TriggerManager> triggerManagers, ILogger logger)
        {
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (triggerManagers == null) throw new ArgumentNullException("triggerManagers");
            if (logger == null) throw new ArgumentNullException("logger");
            this.soundManager = soundManager;
            this.trayPopups = trayPopups;
            this.triggerManagers = triggerManagers;
            this.logger = logger;
        }

        public async Task ImportFromDtoAsync(WurmAssistantDto dto)
        {
            var groupedByCharacter = dto.Triggers.GroupBy(trigger => trigger.CharacterName);

            List<ImportItem<Trigger, ITrigger>> allImportItems = new List<ImportItem<Trigger, ITrigger>>();

            foreach (var group in groupedByCharacter)
            {
                var characterName = new CharacterName(group.Key);
                TriggerManager manager;
                triggerManagers.TryGetValue(characterName.Capitalized, out manager);

                var items = group.Select(trigger =>
                {
                    ITrigger existingTrigger = null;
                    bool isActionQueue = false;
                    bool isActionQueueAndOneExists = false;
                    string comment = null;

                    var kind = (TriggerKind) Enum.Parse(typeof (TriggerKind), trigger.TriggerKind, true);
                    isActionQueue = kind == TriggerKind.ActionQueue;

                    if (manager != null)
                    {
                        existingTrigger = manager.Triggers.FirstOrDefault(
                            t =>
                                t.TriggerId == trigger.TriggerId
                                || (t.Name != null
                                    && t.Name.Equals(trigger.Name, StringComparison.InvariantCultureIgnoreCase)));

                        isActionQueueAndOneExists = isActionQueue
                                                    && manager.Triggers.Any(
                                                        t => t.TriggerKind == TriggerKind.ActionQueue);

                        if (isActionQueueAndOneExists)
                            comment = "This character already has an action queue trigger";
                    }
                    else
                    {
                        comment = "No manager exists for character name: " + characterName.Capitalized
                                  + ". Please create to enable importing this trigger";
                    }

                    return new ImportItem<Trigger, ITrigger>()
                    {
                        Comment = comment,
                        Blocked = manager == null || isActionQueueAndOneExists,
                        Source = trigger,
                        Destination = existingTrigger,
                        SourceAspectConverter =
                            t =>
                                t != null
                                    ? string.Format("Id: {0} Name: {1}, Type: {2}", t.TriggerId, t.Name, t.TriggerKind)
                                    : string.Empty,
                        DestinationAspectConverter =
                            t =>
                                t != null
                                    ? string.Format("Id: {0} Name: {1}, Type: {2}",
                                        t.TriggerId,
                                        t.Name,
                                        t.TriggerKind)
                                    : string.Empty,
                        ResolutionAction = (result, source, dest) =>
                        {
                            if (result == MergeResult.AddAsNew)
                            {
                                // manager can't and shouldn't be null at this point
                                if (manager == null)
                                    throw new ApplicationException("Manager is null");
                                RecreateImportedTrigger(manager, source, kind);
                            }
                        }
                    };
                }).ToArray();

                allImportItems.AddRange(items);
            }

            var mergeAssistantView = new ImportMergeAssistantView(allImportItems, logger);
            mergeAssistantView.Text = "Choose triggers to import...";
            mergeAssistantView.StartPosition = FormStartPosition.CenterScreen;
            mergeAssistantView.Show();
            await mergeAssistantView.Completed;
        }

        private void RecreateImportedTrigger(TriggerManager manager, Trigger source, TriggerKind sourceKind)
        {
            if (sourceKind == TriggerKind.ActionQueue)
            {
                var newTrigger =
                    (ActionQueueTrigger)manager.CreateTrigger(sourceKind);
                PopulateBaseTrigger(newTrigger, source);
                newTrigger.NotificationDelay = source.NotificationDelay ?? 1;
            }
            else if (sourceKind == TriggerKind.Regex)
            {
                var newTrigger =
                    (RegexTrigger)manager.CreateTrigger(sourceKind);
                PopulateBaseTrigger(newTrigger, source);
                newTrigger.Condition = source.Condition;
            }
            else if (sourceKind == TriggerKind.Simple)
            {
                var newTrigger =
                    (SimpleTrigger)manager.CreateTrigger(sourceKind);
                PopulateBaseTrigger(newTrigger, source);
                newTrigger.Condition = source.Condition;
            }
            else
            {
                throw new InvalidOperationException("Unsupported import of trigger kind: " + sourceKind);
            }
        }

        private void PopulateBaseTrigger(TriggerBase newTrigger, Trigger source)
        {
            newTrigger.Name = source.Name;

            newTrigger.Cooldown = source.Cooldown;
            newTrigger.CooldownEnabled = source.CooldownEnabled;
            newTrigger.Active = source.Active;
            if (source.LogTypes != null)
            {
                foreach (var logType in source.LogTypes)
                {
                    LogType logTypeEnum;
                    if (Enum.TryParse(logType, true, out logTypeEnum))
                    {
                        newTrigger.SetLogType(logTypeEnum, CheckState.Checked);
                    }
                }
            }
            newTrigger.ResetOnConditonHit = source.ResetOnConditonHit;
            newTrigger.DelayEnabled = source.DelayEnabled ?? false;
            newTrigger.Delay = source.Delay;
            newTrigger.StayUntilClicked = source.StayUntilClicked ?? false;
            if (source.HasSound && source.Sound != null)
            {
                var soundId = SoundEngineImportHelper.MergeSoundAndGetId(soundManager, source.Sound);
                newTrigger.AddNotifier(new SoundNotifier(newTrigger, soundManager)
                {
                    SoundId = soundId
                });
            }
            if (source.HasPopup)
            {
                newTrigger.AddNotifier(new PopupNotifier(newTrigger, trayPopups));
            }
            newTrigger.PopupTitle = source.PopupTitle;
            newTrigger.PopupContent = source.PopupContent;
            newTrigger.PopupDurationMillis = source.PopupDurationMillis ?? 3000;
        }
    }
}