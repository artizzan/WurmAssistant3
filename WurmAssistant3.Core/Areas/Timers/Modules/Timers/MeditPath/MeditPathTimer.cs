using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.MeditPath;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.MeditPath
{
    [PersistentObject("TimersFeature_MeditPathTimer")]
    public class MeditPathTimer : WurmTimer
    {
        public static class MeditPathHelper
        {
            // knowledge, insanity, power, love, hate
            static string[] Level0 = { "Uninitiated" };
            static string[] Level1 = { "Initiate" };
            static string[] Level2 = { "Eager", "Disturbed", "Gatherer", "Nice", "Ridiculous" };
            static string[] Level3 = { "Explorer", "Crazed", "Greedy", "Gentle", "Envious" };
            static string[] Level4 = { "Sheetfolder", "Deranged", "Strong", "Warm", "Hateful" };
            static string[] Level5 = { "Desertmind", "Sicko", "Released", "Goodhearted", "Finger" };
            static string[] Level6 = { "Observer", "Mental", "Unafraid", "Giving", "Sheep" };
            static string[] Level7 = { "Bookkeeper", "Psycho", "Brave", "Rock", "Snake" };
            static string[] Level8 = { "Mud-dweller", "Beast", "Performer", "Splendid", "Shark" };
            static string[] Level9 = { "Thought Eater", "Maniac", "Liberator", "Protector", "Infection" };
            static string[] Level10 = { "Crooked", "Drooling", "Force", "Respectful", "Swarm" };
            static string[] Level11 = { "Enlightened", "Gone", "Vibrant Light", "Saint", "Free" };
            static string[] Level12 = { "12th Hierophant", "12th Eidolon", "12th Sovereign", "12th Deva", "12th Harbinger" };
            static string[] Level13 = { "13th Hierophant", "13th Eidolon", "13th Sovereign", "13th Deva", "13th Harbinger" };
            static string[] Level14 = { "14th Hierophant", "14th Eidolon", "14th Sovereign", "14th Deva", "14th Harbinger" };
            static string[] Level15 = { "15th Hierophant", "15th Eidolon", "15th Sovereign", "15th Deva", "15th Harbinger" };

            public static Dictionary<int, string[]> LevelToTitlesMap = new Dictionary<int, string[]>();
            public static Dictionary<int, int> LevelToCooldownInHoursMap = new Dictionary<int, int>();

            static MeditPathHelper()
            {
                LevelToTitlesMap.Add(0, Level0);
                LevelToTitlesMap.Add(1, Level1);
                LevelToTitlesMap.Add(2, Level2);
                LevelToTitlesMap.Add(3, Level3);
                LevelToTitlesMap.Add(4, Level4);
                LevelToTitlesMap.Add(5, Level5);
                LevelToTitlesMap.Add(6, Level6);
                LevelToTitlesMap.Add(7, Level7);
                LevelToTitlesMap.Add(8, Level8);
                LevelToTitlesMap.Add(9, Level9);
                LevelToTitlesMap.Add(10, Level10);
                LevelToTitlesMap.Add(11, Level11);
                LevelToTitlesMap.Add(12, Level12);
                LevelToTitlesMap.Add(13, Level13);
                LevelToTitlesMap.Add(14, Level14);
                LevelToTitlesMap.Add(15, Level15);

                LevelToCooldownInHoursMap.Add(0, 0);
                LevelToCooldownInHoursMap.Add(1, 12);
                LevelToCooldownInHoursMap.Add(2, 24);
                LevelToCooldownInHoursMap.Add(3, 72);
                LevelToCooldownInHoursMap.Add(4, 144);
                LevelToCooldownInHoursMap.Add(5, 288);
                LevelToCooldownInHoursMap.Add(6, 576);
                LevelToCooldownInHoursMap.Add(7, 576);
                LevelToCooldownInHoursMap.Add(8, 576);
                LevelToCooldownInHoursMap.Add(9, 576);
                LevelToCooldownInHoursMap.Add(10, 576);
                LevelToCooldownInHoursMap.Add(11, 576);
                LevelToCooldownInHoursMap.Add(12, 576);
                LevelToCooldownInHoursMap.Add(13, 576);
                LevelToCooldownInHoursMap.Add(14, 576);
                LevelToCooldownInHoursMap.Add(15, 576);
            }

            public static int FindLevel(string line)
            {
                foreach (var item in LevelToTitlesMap)
                {
                    foreach (string title in item.Value)
                    {
                        if (Regex.IsMatch(line, title))
                        {
                            return item.Key;
                        }
                    }
                }
                return -1;
            }
        }

        [JsonProperty]
        DateTime dateOfNextQuestionAttempt = DateTime.MinValue;
        [JsonProperty]
        DateTime lastCheckup = DateTime.MinValue;
        [JsonProperty]
        DateTime nextQuestionAttemptOverridenUntil = DateTime.MinValue;


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

        public MeditPathTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
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

        SkillEntryParser skillEntryParser;

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            TimerDisplayView.SetCooldown(TimeSpan.FromDays(1));

            skillEntryParser = new SkillEntryParser(WurmApi);

            MoreOptionsAvailable = true;

            PerformAsyncInits();
        }

        async Task PerformAsyncInits()
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

        public override void Update(bool engineSleeping)
        {
            base.Update(engineSleeping);
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
            else nextMeditLevel = MeditPathHelper.FindLevel(line.Content) + 1;

            if (nextMeditLevel > 15) nextMeditLevel = 15;
            MeditPathHelper.LevelToCooldownInHoursMap.TryGetValue(
                 nextMeditLevel, out cdInHrs);

            DateOfNextQuestionAttempt = line.Timestamp + TimeSpan.FromHours(cdInHrs);
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            base.OpenMoreOptions(form);
            MeditPathTimerOptionsForm ui = new MeditPathTimerOptionsForm(form, this);
            ui.ShowDialog();
        }

        internal void SetManualQTimer(int meditLevel, DateTime originDate)
        {
            int hours = MeditPathHelper.LevelToCooldownInHoursMap[meditLevel];
            NextQuestionAttemptOverridenUntil = originDate + TimeSpan.FromHours(hours);
            UpdateCooldown();
        }
    }
}
