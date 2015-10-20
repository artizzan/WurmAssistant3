using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.MeditPath;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.MeditPath
{
    [PersistentObject("TimersFeature_MeditPathTimer")]
    public class MeditPathTimer : WurmTimer
    {
        [JsonProperty]
        DateTime dateOfNextQuestionAttempt = DateTime.MinValue;
        [JsonProperty]
        DateTime lastCheckup = DateTime.MinValue;
        [JsonProperty]
        DateTime nextQuestionAttemptOverridenUntil = DateTime.MinValue;

        public MeditPathTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            TimerDisplayView.SetCooldown(TimeSpan.FromDays(1));

            MoreOptionsAvailable = true;

            PerformAsyncInits();
        }

        async void PerformAsyncInits()
        {
            try
            {
                List<LogEntry> lines = await GetLogLinesFromLogHistoryAsync(LogType.Event, LastCheckup);
                if (!ProcessQuestionLogSearch(lines) && DateOfNextQuestionAttempt == default(DateTime))
                {
                    Logger.Info("could not figure when was last meditation question answer, trying 1-year log search");
                    lines = await GetLogLinesFromLogHistoryAsync(LogType.Event, TimeSpan.FromDays(365));
                    if (!ProcessQuestionLogSearch(lines))
                    {
                        Logger.Info("failed to figure when was last meditation question answer");
                    }
                }
                LastCheckup = DateTime.Now;

                UpdateCooldown();

                InitCompleted = true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "init problem");
            }
        }

        DateTime DateOfNextQuestionAttempt 
        {
            get { return dateOfNextQuestionAttempt; } 
            set
            {
                dateOfNextQuestionAttempt = value;
                FlagAsChanged();
            }
        }

        DateTime LastCheckup
        {
            get { return lastCheckup; }
            set
            {
                lastCheckup = value;
                FlagAsChanged();
            }
        }

        public DateTime NextQuestionAttemptOverridenUntil
        {
            get { return nextQuestionAttemptOverridenUntil; }
            set
            {
                nextQuestionAttemptOverridenUntil = value;
                FlagAsChanged();
            }
        }

        public void UpdateCooldown()
        {
            if (NextQuestionAttemptOverridenUntil < DateTime.Now)
                NextQuestionAttemptOverridenUntil = DateTime.MinValue;

            CDNotify.CooldownTo = GetCooldownDate();
        }

        public void RemoveManualCooldown()
        {
            NextQuestionAttemptOverridenUntil = DateTime.MinValue;
            UpdateCooldown();
        }

        private DateTime GetCooldownDate()
        {
            if (NextQuestionAttemptOverridenUntil == DateTime.MinValue)
            {
                return DateOfNextQuestionAttempt;
            }
            else return NextQuestionAttemptOverridenUntil;
        }

        public override void Update()
        {
            base.Update();
            if (TimerDisplayView.Visible) TimerDisplayView.UpdateCooldown(GetCooldownDate());
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("Congratulations", StringComparison.Ordinal))
            {
                if (line.Content.Contains("Congratulations! You have now reached the level"))
                {
                    UpdateDateOfNextQuestionAttempt(line, true, false);
                    RemoveManualCooldown();
                    UpdateCooldown();
                }
            }
            else if (line.Content.StartsWith("You decide", StringComparison.Ordinal))
            {
                if (line.Content.Contains("You decide to start pursuing the insights of the path of"))
                {
                    UpdateDateOfNextQuestionAttempt(line, true, true);
                    RemoveManualCooldown();
                    UpdateCooldown();
                }
            }
        }

        bool ProcessQuestionLogSearch(List<LogEntry> lines)
        {
            bool result = false;

            bool IsPathBegin = false;
            //[00:35:09] Congratulations! You have now reached the level of Rock of the path of love!
            LogEntry lastPathAdvancedLine = null;
            LogEntry lastPathFailLine = null;
            foreach (LogEntry line in lines)
            {
                if (line.Content.Contains("You decide to start pursuing the insights of the path of"))
                {
                    lastPathAdvancedLine = line;
                    lastPathFailLine = null;
                    IsPathBegin = true;
                }

                if (line.Content.Contains("Congratulations! You have now reached the level"))
                {
                    IsPathBegin = false;
                    lastPathAdvancedLine = line;
                    lastPathFailLine = null; //reset any previous fail finds because they are irrelevant now
                }
                //if (line.Contains("[fail message]")
                //    lastPathFailLine = line;
            }
            if (lastPathAdvancedLine != null)
            {
                UpdateDateOfNextQuestionAttempt(lastPathAdvancedLine, false, IsPathBegin);
                result = true;
            }
            if (lastPathFailLine != null)
            {
                //NYI
            }
            return result;
        }

        void UpdateDateOfNextQuestionAttempt(LogEntry line, bool liveLogs, bool pathBegin)
        {
            int cdInHrs = 0;
            int nextMeditLevel;

            if (pathBegin) nextMeditLevel = 1;
            else nextMeditLevel = MeditationPaths.FindLevel(line.Content) + 1;

            cdInHrs = MeditationPaths.GetCooldownHoursForLevel(nextMeditLevel).ConstrainToRange(0, int.MaxValue);

            DateOfNextQuestionAttempt = line.Timestamp + TimeSpan.FromHours(cdInHrs);
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            base.OpenMoreOptions(form);
            MeditPathTimerOptionsForm ui = new MeditPathTimerOptionsForm(form, this);
            ui.ShowDialogCenteredOnForm(form);
        }

        internal void SetManualQTimer(int meditLevel, DateTime originDate)
        {
            int hours = MeditationPaths.GetCooldownHoursForLevel(meditLevel).ConstrainToRange(0, int.MaxValue);
            NextQuestionAttemptOverridenUntil = originDate + TimeSpan.FromHours(hours);
            UpdateCooldown();
        }
    }
}
