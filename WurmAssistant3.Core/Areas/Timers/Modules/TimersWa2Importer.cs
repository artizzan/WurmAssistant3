using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Views;
using WurmAssistantDataTransfer.Dtos;
using Timer = WurmAssistantDataTransfer.Dtos.Timer;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules
{
    public class TimersWa2Importer
    {
        private readonly TimersFeature timersFeature;
        private readonly TimerDefinitions timerDefinitions;
        private readonly ISoundManager soundManager;
        private readonly ILogger logger;

        public TimersWa2Importer(TimersFeature timersFeature, TimerDefinitions timerDefinitions,
            ISoundManager soundManager, ILogger logger)
        {
            if (timersFeature == null) throw new ArgumentNullException("timersFeature");
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (logger == null) throw new ArgumentNullException("logger");
            this.timersFeature = timersFeature;
            this.timerDefinitions = timerDefinitions;
            this.soundManager = soundManager;
            this.logger = logger;
        }

        public async Task ImportFromDtoAsync(WurmAssistantDto dto)
        {
            await ImportCustomTimerDefinitions(dto);
            await ImportTimers(dto);
        }

        public async Task ImportCustomTimerDefinitions(WurmAssistantDto dto)
        {
            List<ImportItem<LegacyCustomTimerDefinition, TimerDefinition>> importItems =
                new List<ImportItem<LegacyCustomTimerDefinition, TimerDefinition>>();

            var existingCustomTimerDefs = timerDefinitions.GetCustomTimerDefinitions().ToArray();

            foreach (var legacyCustomTimerDefinition in dto.LegacyCustomTimerDefinitions)
            {
                var matchingDef =
                    existingCustomTimerDefs.FirstOrDefault(
                        definition => definition.Name == legacyCustomTimerDefinition.Name);

                importItems.Add(new ImportItem<LegacyCustomTimerDefinition, TimerDefinition>()
                {
                    Source = legacyCustomTimerDefinition,
                    Destination = matchingDef,
                    ResolutionAction = (result, source, dest) =>
                    {
                        if (result == MergeResult.AddAsNew)
                        {
                            AddCustomTimerDefinition(source, source.Id ?? Guid.NewGuid());
                        }
                    },
                    SourceAspectConverter =
                        definition =>
                            definition != null
                                ? string.Format("{0} ({1})", definition.ToString(), definition.LegacyServerGroupId)
                                : string.Empty,
                    DestinationAspectConverter =
                        definition => definition != null ? definition.ToVerboseString() : string.Empty
                });
            }

            var mergeAssistantView = new ImportMergeAssistantView(importItems, logger);
            mergeAssistantView.Text = "Choose custom timer definitions to import...";
            mergeAssistantView.StartPosition = FormStartPosition.CenterScreen;
            mergeAssistantView.Show();
            await mergeAssistantView.Completed;
        }

        public async Task ImportTimers(WurmAssistantDto dto)
        {
            var groupedByCharacter = dto.Timers.GroupBy(timer => timer.CharacterName).ToArray();

            List<ImportItem<Timer, WurmTimer>> importItems = new List<ImportItem<Timer, WurmTimer>>();
            int timerImportErrorCount = 0;

            foreach (var characterGrouped in groupedByCharacter)
            {
                var characterName = new CharacterName(characterGrouped.Key);
                var groupedByServerGroup = characterGrouped.GroupBy(timer => timer.ServerGroupId).ToArray();
                foreach (var serverGrouped in groupedByServerGroup)
                {
                    var serverGroup = serverGrouped.Key;
                    var manager = timersFeature.GetActivePlayerGroups().SingleOrDefault(group =>
                        characterName == new CharacterName(group.CharacterName)
                        && String.Equals(group.ServerGroupId, serverGroup, StringComparison.InvariantCultureIgnoreCase));
                    bool blocked = false;
                    string comment = null;
                    if (manager == null)
                    {
                        blocked = true;
                        comment =
                            string.Format(
                                "Timers manager does not exist for a combination of character named {0} and server group id {1}. To enable importing, please create such setup.",
                                characterName,
                                serverGroup);
                    }

                    foreach (var timer in serverGrouped)
                    {
                        try
                        {
                            importItems.Add(BuildImportItem(timer, blocked, comment, manager, dto));
                        }
                        catch (Exception exception)
                        {
                            timerImportErrorCount++;
                            logger.Error(exception, "Timer import error, timer: " + timer.ToString());
                        }
                    }
                }
            }

            if (timerImportErrorCount > 0)
            {
                MessageBox.Show(timerImportErrorCount + (timerImportErrorCount > 1 ? " timers" : " timer")
                                + "failed to import, see app log for details. You can continue to import remaining timers.",
                    "Import errors",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            var mergeAssistantView = new ImportMergeAssistantView(importItems, logger);
            mergeAssistantView.Text = "Choose timers to import...";
            mergeAssistantView.StartPosition = FormStartPosition.CenterScreen;
            mergeAssistantView.Show();
            await mergeAssistantView.Completed;
        }

        private ImportItem<Timer, WurmTimer> BuildImportItem(Timer timer, bool blocked, string comment, PlayerTimersGroup manager, WurmAssistantDto dto)
        {
            var importItem = new ImportItem<Timer, WurmTimer>()
            {
                Source = timer,
                Destination = TryGetDestination(timer, manager),
                Blocked = blocked,
                Comment = comment,
                SourceAspectConverter =
                    t =>
                        string.Format("Id: {0}, Name: {1}, Game character: {2}, Server Group: {3}, Type: {4}",
                            timer.Id,
                            timer.Name,
                            timer.CharacterName,
                            timer.ServerGroupId,
                            timer.RuntimeTypeIdEnum),
                DestinationAspectConverter =
                    t =>
                        t != null
                            ? string.Format("Id: {0}, Name: {1}, Game character: {2}, Server Group: {3}, Type: {4}",
                                t.Id,
                                t.Name,
                                t.Character,
                                t.ServerGroupId,
                                t.TimerDefinition.RuntimeTypeId)
                            : null,
                ResolutionAction = (result, source, dest) =>
                {
                    if (result == MergeResult.AddAsNew)
                    {
                        TimerDefinition definition = FindOrMergeTimerDefinition(dto, source);

                        var newTimer = manager.AddNewTimer(definition);
                        try
                        {
                            ConfigureNewTimer(newTimer, source);
                        }
                        catch (Exception)
                        {
                            manager.RemoveTimer(newTimer);
                            throw;
                        }
                    }
                }
            };
            return importItem;
        }

        private void ConfigureNewTimer(WurmTimer newTimer, Timer source)
        {
            if (source.SoundNotify)
            {
                newTimer.SoundId = SoundEngineImportHelper.MergeSoundAndGetId(soundManager, source.Sound);
                newTimer.SoundNotify = true;
            }
            if (source.PopupNotify)
            {
                newTimer.PopupDurationMillis = source.PopupDurationMillis;
                newTimer.PopupOnWaLaunch = source.PopupOnWaLaunch;
                newTimer.PopupNotify = true;
                newTimer.PersistentPopup = source.PersistentPopup;
            }
            RuntimeTypeId typeId;
            if (Enum.TryParse(source.RuntimeTypeIdEnum, out typeId))
            {
                if (typeId == RuntimeTypeId.Meditation)
                {
                    var t = (Timers.Meditation.MeditationTimer) newTimer;
                    var s = (MeditationTimer) source;
                    t.SleepBonusReminder = s.SleepBonusReminder;
                    t.SleepBonusPopupDurationMillis = s.SleepBonusPopupDurationMillis;
                    t.ShowMeditSkill = s.ShowMeditSkill;
                    t.ShowMeditCount = s.ShowMeditCount;
                }
                else if (typeId == RuntimeTypeId.Alignment)
                {
                    var t = (Timers.Alignment.AlignmentTimer) newTimer;
                    var s = (AlignmentTimer) source;
                    t.IsWhiteLighter = s.IsWhiteLighter;
                    t.PlayerReligion =
                        (Timers.Alignment.AlignmentTimer.WurmReligions)
                            Enum.Parse(typeof (Timers.Alignment.AlignmentTimer.WurmReligions), s.WurmReligion, true);
                }
                else if (typeId == RuntimeTypeId.Prayer)
                {
                    var t = (Timers.Prayer.PrayerTimer) newTimer;
                    var s = (PrayerTimer) source;
                    t.FavorSettings.FavorNotifySound = s.FavorNotifySoundEnabled;
                    if (s.FavorNotifySound != null)
                    {
                        t.FavorSettings.FavorNotifySoundId = SoundEngineImportHelper.MergeSoundAndGetId(soundManager,
                            s.FavorNotifySound);
                    }
                    t.FavorSettings.FavorNotifyPopup = s.FavorNotifyPopupEnabled;
                    t.FavorSettings.FavorNotifyWhenMax = s.FavorNotifyWhenMax;
                    t.FavorSettings.FavorNotifyOnLevel = s.FavorNotifyOnLevel;
                    t.FavorSettings.FavorNotifyPopupPersist = s.FavorNotifyPopupPersist;
                }
            }
        }

        private TimerDefinition FindOrMergeTimerDefinition(WurmAssistantDto dto, Timer source)
        {
            TimerDefinition definition = null;
            if (source.DefinitionId != null)
            {
                definition = timerDefinitions.TryGetById(source.DefinitionId.Value);
            }

            if (definition == null
                && source.RuntimeTypeIdEnum.Equals(RuntimeTypeId.LegacyCustom.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                if (source.DefinitionId == null)
                {
                    definition = timerDefinitions.TryGetByName(source.LegacyDefinitionName);
                }

                if (definition == null)
                {
                    LegacyCustomTimerDefinition definitionToImport;
                    if (source.DefinitionId != null)
                    {
                        definitionToImport =
                            dto.LegacyCustomTimerDefinitions.FirstOrDefault(
                                legacyDef => legacyDef.Id == source.DefinitionId);
                    }
                    else
                    {
                        definitionToImport =
                            dto.LegacyCustomTimerDefinitions.FirstOrDefault(
                                legacyDef => legacyDef.Name == source.Name);
                    }
                    if (definitionToImport == null)
                    {
                        throw new InvalidOperationException(
                            string.Format("Could not find a custom timer definition for Id: {0} Name: {1}",
                                source.DefinitionId,
                                source.Name));
                    }

                    AddCustomTimerDefinition(definitionToImport, source.DefinitionId ?? Guid.NewGuid());
                }

                return definition;
            }
            
            if (definition == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unexpected definition of the timer Id: {0}, named: {1}, Character {2}, Server group {3}",
                        source.Id,
                        source.Name,
                        source.CharacterName,
                        source.ServerGroupId));
            }

            return definition;
        }

        private void AddCustomTimerDefinition(LegacyCustomTimerDefinition definitionToImport, Guid? id = null)
        {
            var definition = new TimerDefinition(id ?? Guid.NewGuid())
            {
                Name = definitionToImport.Name,
                RuntimeTypeId = RuntimeTypeId.LegacyCustom,
                CustomTimerConfig = new CustomTimerDefinition()
                {
                    Duration = definitionToImport.Duration,
                    ResetConditions = definitionToImport.ResetConditions.Select(c =>
                        new CustomTimerDefinition.Condition()
                        {
                            LogType = ParseLogType(c.LogType),
                            RegexPattern = c.Pattern
                        }).ToArray(),
                    TriggerConditions = definitionToImport.TriggerConditions.Select(c =>
                        new CustomTimerDefinition.Condition()
                        {
                            LogType = ParseLogType(c.LogType),
                            RegexPattern = c.Pattern
                        }).ToArray(),
                    ResetOnUptime = definitionToImport.ResetOnUptime
                }
            };

            timerDefinitions.AddTimerDefinition(definition);
        }

        private LogType ParseLogType(string logType)
        {
            return (LogType)Enum.Parse(typeof(LogType), logType, true);
        }

        private WurmTimer TryGetDestination(Timer timer, PlayerTimersGroup manager)
        {
            if (manager == null) return null;
            if (timer.Id != null)
            {
                return manager.Timers.FirstOrDefault(wurmTimer => wurmTimer.Id == timer.Id);
            }
            else if (timer.DefinitionId != null)
            {
                return manager.Timers.FirstOrDefault(wurmTimer => wurmTimer.TimerDefinition.Id == timer.DefinitionId.Value);
            }
            else
            {
                return manager.Timers.FirstOrDefault(wurmTimer => wurmTimer.Name == timer.Name);
            }
        }
    }
}