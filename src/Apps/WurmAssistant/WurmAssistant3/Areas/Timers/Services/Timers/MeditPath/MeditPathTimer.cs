using System;
using System.Collections.Generic;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Parts;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers.MeditPath;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Timers.Services.Timers.MeditPath
{
    [KernelBind, PersistentObject("TimersFeature_MeditPathTimer")]
    public class MeditPathTimer : WurmTimer
    {
        enum PathSwitchKind { PathAdvance, PathBegin, PathReset }

        [JsonProperty]
        DateTime dateOfNextQuestionAttempt = DateTime.MinValue;
        [JsonProperty]
        DateTime lastCheckup = DateTime.MinValue;
        [JsonProperty]
        DateTime nextQuestionAttemptOverridenUntil = DateTime.MinValue;

        readonly TimeSpan pathCooldown = TimeSpan.FromDays(1);

        public MeditPathTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundManager soundManager,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundManager)
        {
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            View.SetCooldown(pathCooldown);

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
            if (View.Visible)
            {
                View.SetCooldown(pathCooldown);
                View.UpdateCooldown(GetCooldownDate());
            }
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("Congratulations", StringComparison.Ordinal))
            {
                if (line.Content.Contains("Congratulations! You have now reached the level"))
                {
                    UpdateDateOfNextQuestionAttempt(line, PathSwitchKind.PathAdvance);
                    RemoveManualCooldown();
                    UpdateCooldown();
                }
            }
            else if (line.Content.StartsWith("You decide", StringComparison.Ordinal))
            {
                if (line.Content.Contains("You decide to start pursuing the insights of the path of"))
                {
                    UpdateDateOfNextQuestionAttempt(line, PathSwitchKind.PathBegin);
                    RemoveManualCooldown();
                    UpdateCooldown();
                }
                else if (line.Content.StartsWith("You decide to stop pursuing the insights of the path of "))
                {
                    UpdateDateOfNextQuestionAttempt(line, PathSwitchKind.PathReset);
                    UpdateCooldown();
                }
            }
        }

        bool ProcessQuestionLogSearch(List<LogEntry> lines)
        {
            bool result = false;

            PathSwitchKind switchKind = PathSwitchKind.PathAdvance;
            
            LogEntry lastPathModifyLine = null;
            LogEntry lastPathFailLine = null;
            foreach (LogEntry line in lines)
            {
                if (line.Content.Contains("You decide to start pursuing the insights of the path of"))
                {
                    lastPathModifyLine = line;
                    lastPathFailLine = null;
                    switchKind = PathSwitchKind.PathBegin;
                }

                //[00:35:09] Congratulations! You have now reached the level of Rock of the path of love!
                if (line.Content.Contains("Congratulations! You have now reached the level"))
                {
                    lastPathModifyLine = line;
                    lastPathFailLine = null;
                    switchKind = PathSwitchKind.PathAdvance;
                }

                // [05:49:11] You decide to stop pursuing the insights of the path of knowledge. 
                if (line.Content.StartsWith("You decide to stop pursuing the insights of the path of "))
                {
                    lastPathModifyLine = line;
                    lastPathFailLine = null;
                    switchKind = PathSwitchKind.PathReset;
                }
                //if (line.Contains("[fail message]")
                //    lastPathFailLine = line;
            }
            if (lastPathModifyLine != null)
            {
                UpdateDateOfNextQuestionAttempt(lastPathModifyLine, switchKind);
                result = true;
            }
            if (lastPathFailLine != null)
            {
                //NYI
            }
            return result;
        }

        void UpdateDateOfNextQuestionAttempt(LogEntry line, PathSwitchKind switchKind)
        {
            if (line == null) throw new ArgumentNullException("line");

            int cdInHrs = 0;
            int nextMeditLevel = 0;

            if (switchKind == PathSwitchKind.PathBegin)
            {
                nextMeditLevel = 1;
            }
            else if (switchKind == PathSwitchKind.PathReset)
            {
                nextMeditLevel = 0;
                // there is a 24h cooldown before new path can be learned
                NextQuestionAttemptOverridenUntil = line.Timestamp + TimeSpan.FromHours(24);
            }
            else
            {
                // PathAdvance
                nextMeditLevel = MeditationPaths.FindLevel(line.Content) + 1;
            }

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
