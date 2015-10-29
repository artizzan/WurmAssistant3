using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldursLab.Essentials.Asynchronous;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Meditation;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Meditation
{
    [PersistentObject("TimersFeature_MeditationTimer")]
    public class MeditationTimer : WurmTimer
    {
        class MeditHistoryEntry
        {
            public MeditHistoryEntryTypes EntryType { get; private set; }
            public DateTime EntryDateTime { get; private set; }
            public bool Valid { get; set; }

            /// <summary>
            /// this is a quick fix flag for uptime not reseting medit cooldown
            /// </summary>
            /// <remarks>
            /// due to recent change in wurm, cooldowns do not appear to reset immediatelly after uptime
            /// it is unknown if this is some delay, random issue or the cooldown just simply doesnt reset on uptime swap
            /// this flag can be used to mark an entry, that happened just prior to cooldown reset.
            /// It can then be checked and entry returned as cooldown trigger, if no more recent entries are found,
            /// this in effect triggers a cooldown but does not mess the "Valid" flag, which is used to count medits during
            /// single uptime window (so that long/short cooldown count remains correct).
            /// </remarks>
            public bool ThisShouldTriggerCooldownButNotCountForThisUptimeWindow { get; set; }

            public MeditHistoryEntry(MeditHistoryEntryTypes type, DateTime date)
            {
                this.EntryType = type;
                this.EntryDateTime = date;
            }
        }

        class SleepBonusNotify
        {
            readonly NotifyHandler handler;

            DateTime? sleepBonusStarted = null;
            DateTime? lastMeditHappenedOn = DateTime.MinValue;

            bool meditHappened = false;

            public bool Enabled { get; set; }
            public int PopupDuration { get { return handler.Duration; } set { handler.Duration = value; } }

            public SleepBonusNotify(ILogger logger, ISoundEngine soundEngine, ITrayPopups trayPopups, string popupTitle,
                string popupMessage, bool popupPersistent = false)
            {
                handler = new NotifyHandler(logger, soundEngine, trayPopups)
                {
                    PopupPersistent = popupPersistent,
                    Message = (popupMessage ?? ""),
                    Title = (popupTitle ?? "")
                };
            }

            public void Update()
            {
                if (sleepBonusStarted != null && meditHappened && DateTime.Now > sleepBonusStarted + TimeSpan.FromMinutes(5))
                {
                    if (lastMeditHappenedOn > DateTime.Now - TimeSpan.FromMinutes(10))
                    {
                        if (Enabled)
                            handler.Show();
                    }
                    meditHappened = false;
                }
                handler.Update();
            }

            public void SleepBonusActivated()
            {
                meditHappened = false;
                sleepBonusStarted = DateTime.Now;
            }

            public void SleepBonusDeactivated()
            {
                sleepBonusStarted = null;
                meditHappened = false;
            }

            public void MeditationHappened()
            {
                lastMeditHappenedOn = DateTime.Now;
                meditHappened = true;
            }
        }

        enum MeditationStates { Unlimited, Limited }

        enum MeditHistoryEntryTypes { Meditation, LongCooldownTrigger }

        const double FloatAboveZeroCompareValue = 0.00001;
        static readonly TimeSpan LongMeditCooldown = new TimeSpan(3, 0, 0);
        static readonly TimeSpan ShortMeditCooldown = new TimeSpan(0, 30, 0);

        readonly List<MeditHistoryEntry> meditHistory = new List<MeditHistoryEntry>();
        readonly TriggerableAsyncOperation cooldownUpdateOperation;

        [JsonProperty]
        DateTime meditSkillLastCheckup;

        [JsonProperty]
        float meditationSkill;

        [JsonProperty]
        bool sleepBonusReminder;

        [JsonProperty]
        int sleepBonusPopupDurationMillis = 4000;

        [JsonProperty]
        bool showMeditSkill;

        [JsonProperty]
        bool showMeditCount;

        DateTime cooldownResetSince = DateTime.Now;
        TimeSpan timeOnThisCooldownReset = new TimeSpan(0);
        bool isLongMeditCooldown = false;
        MeditationStates meditState = MeditationStates.Unlimited;
        SleepBonusNotify sleepNotify;
        DateTime nextMeditationDate = DateTime.MinValue;

        SkillEntryParser skillEntryParser;

        public MeditationTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
            cooldownUpdateOperation = new TriggerableAsyncOperation(UpdateMeditationCooldown);
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);

            View.SetCooldown(ShortMeditCooldown);

            skillEntryParser = new SkillEntryParser(WurmApi);

            sleepNotify = new SleepBonusNotify(Logger, SoundEngine, TrayPopups, Player, "Can turn off sleep bonus now");
            sleepNotify.Enabled = SleepBonusReminder;

            View.UpdateSkill(MeditationSkill);
            View.ShowSkill = ShowMeditSkill;
            View.ShowMeditCount = ShowMeditCount;

            MoreOptionsAvailable = true;
            PerformAsyncInits();
        }

        async void PerformAsyncInits()
        {
            try
            {
                var hasArchivalLevel = MeditationSkill > FloatAboveZeroCompareValue;
                float skill = await FindMeditationSkill(hasArchivalLevel);

                if (skill > FloatAboveZeroCompareValue)
                {
                    SetMeditationSkill(skill, triggerCooldownUpdate: false);
                }
                else
                {
                    // forcing update of the timer and "limited" flag
                    SetMeditationSkill(MeditationSkill, triggerCooldownUpdate: false);
                }
                MeditSkillLastCheckup = DateTime.Now;

                List<LogEntry> historyEntries = await GetLogLinesFromLogHistoryAsync(LogType.Event, TimeSpan.FromDays(3));

                foreach (LogEntry line in historyEntries)
                {
                    if (line.Content.Contains("You finish your meditation"))
                    {
                        meditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.Meditation, line.Timestamp));
                    }
                    else if (line.Content.Contains("You feel that it will take you a while before you are ready to meditate again"))
                    {
                        meditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.LongCooldownTrigger, line.Timestamp));
                    }
                }

                await UpdateMeditationCooldown();

                //now new log events can be handled
                InitCompleted = true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "problem while preparing timer");
            }
        }

        async Task UpdateMeditationCooldown()
        {
            await UpdateDateOfLastCooldownReset();
            RevalidateMeditHistory();
            UpdateNextMeditDate();
        }

        DateTime NextMeditationDate
        {
            get { return nextMeditationDate; }
            set { nextMeditationDate = value; CDNotify.CooldownTo = value; }
        }

        public bool ShowMeditCount
        {
            get { return showMeditCount; }
            set
            {
                View.ShowMeditCount = value;
                showMeditCount = value;
                FlagAsChanged();
            }
        }

        DateTime MeditSkillLastCheckup
        {
            get { return meditSkillLastCheckup; }
            set
            {
                meditSkillLastCheckup = value;
                FlagAsChanged();
            }
        }

        float MeditationSkill
        {
            get { return meditationSkill; }
            set
            {
                SetMeditationSkill(value);
            }
        }

        void SetMeditationSkill(float newValue, bool triggerCooldownUpdate = true)
        {
            Logger.Info(string.Format("{0} meditation skill is now {1} on {2}", Player, newValue, ServerGroupId));
            View.UpdateSkill(newValue);
            if (newValue < 20f)
            {
                if (meditState != MeditationStates.Unlimited)
                {
                    cooldownUpdateOperation.Trigger();
                    meditState = MeditationStates.Unlimited;
                }
            }
            else
            {
                if (meditState != MeditationStates.Limited)
                {
                    cooldownUpdateOperation.Trigger();
                    meditState = MeditationStates.Limited;
                }
            }
            meditationSkill = newValue;
            FlagAsChanged();
        }

        public bool SleepBonusReminder
        {
            get { return sleepBonusReminder; }
            set
            {
                sleepNotify.Enabled = value;
                sleepBonusReminder = value;
                FlagAsChanged();
            }
        }

        public int SleepBonusPopupDurationMillis
        {
            get { return sleepBonusPopupDurationMillis; }
            set
            {
                sleepNotify.PopupDuration = value;
                sleepBonusPopupDurationMillis = value;
                FlagAsChanged();
            }
        }

        public bool ShowMeditSkill
        {
            get
            {
                return showMeditSkill;
            }
            set
            {
                showMeditSkill = value;
                FlagAsChanged();
                View.ShowSkill = value;
            }
        }

        async Task<float> FindMeditationSkill(bool hasArchivalLevel)
        {
            var searchFromDate = DateTime.Now;
            if (MeditSkillLastCheckup > new DateTime(1900, 1, 1))
            {
                searchFromDate = MeditSkillLastCheckup;
            }
            searchFromDate -= TimeSpan.FromDays(30);
            float skill = await TryGetSkillFromLogHistoryAsync("Meditating", searchFromDate);
            if (skill < FloatAboveZeroCompareValue)
            {
                if (!hasArchivalLevel)
                {
                    Logger.Info(
                        string.Format(
                            "while preparing medit timer for player: {0} server group: {1}, skill appears to be 0, attempting wider search",
                            Player,
                            ServerGroupId));
                    skill = await TryGetSkillFromLogHistoryAsync("Meditating", TimeSpan.FromDays(365));
                    if (skill < FloatAboveZeroCompareValue)
                    {
                        skill = await TryGetSkillFromLogHistoryAsync("Meditating", TimeSpan.FromDays(1460));
                        if (skill < FloatAboveZeroCompareValue)
                        {
                            Logger.Info(string.Format("could not get any meditation skill for player: {0} server group: {1}", Player, ServerGroupId));
                        }
                    }
                }
                else
                {
                    Logger.Info("Archival level available, skipping wider search for player: " + Player);
                }
            }
            return skill;
        }

        public override void Update()
        {
            base.Update();
            sleepNotify.Update();
            if (View.Visible)
                View.UpdateCooldown(NextMeditationDate - DateTime.Now);
        }

        protected override void HandleServerChange()
        {
            cooldownUpdateOperation.Trigger();
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            MeditationTimerOptionsForm moreOptUi = new MeditationTimerOptionsForm(this);
            moreOptUi.ShowDialogCenteredOnForm(form);
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("You finish your meditation", StringComparison.Ordinal))
            {
                meditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.Meditation, DateTime.Now));
                cooldownUpdateOperation.Trigger();
                sleepNotify.MeditationHappened();
            }
            else if (line.Content.StartsWith("The server has been up", StringComparison.Ordinal)) 
            //"The server has been up 14 hours and 22 minutes."
            {
                cooldownUpdateOperation.Trigger();
            }
            else if (line.Content.StartsWith("You start using the sleep bonus", StringComparison.Ordinal))
            {
                sleepNotify.SleepBonusActivated();
            }
            else if (line.Content.StartsWith("You refrain from using the sleep bonus", StringComparison.Ordinal))
            {
                sleepNotify.SleepBonusDeactivated();
            }
            //[04:31:56] You feel that it will take you a while before you are ready to meditate again.
            else if (line.Content.StartsWith("You feel that it will take", StringComparison.Ordinal))
            {
                if (line.Content.StartsWith("You feel that it will take you a while before you are ready to meditate again", StringComparison.Ordinal))
                {
                    meditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.LongCooldownTrigger, DateTime.Now));
                    cooldownUpdateOperation.Trigger();
                }
            }
        }

        public override void HandleNewSkillLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("Meditating increased", StringComparison.Ordinal)
                || line.Content.StartsWith("Meditating decreased", StringComparison.Ordinal))
            {
                //parse into value
                var info = skillEntryParser.TryParseSkillInfoFromLogLine(line);
                if (info != null)
                {
                    this.MeditationSkill = info.Value;
                    Logger.Info("updated meditation skill for " + Player + " to " + MeditationSkill);
                }
            }
        }

        async Task UpdateDateOfLastCooldownReset()
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                var currentServer = await WurmApi.Characters.Get(Player).TryGetCurrentServerAsync();
                if (currentServer != null)
                {
                    var uptime = await currentServer.TryGetCurrentUptimeAsync();
                    if (uptime != null)
                    {
                        TimeSpan timeSinceLastServerReset = uptime.Value;
                        TimeSpan daysSinceLastServerReset = new TimeSpan(timeSinceLastServerReset.Days, 0, 0, 0);
                        timeSinceLastServerReset = timeSinceLastServerReset.Subtract(daysSinceLastServerReset);

                        var cooldownResetDate = currentTime - timeSinceLastServerReset;
                        this.cooldownResetSince = cooldownResetDate;
                    }
                    else
                        Logger.Warn(string.Format("no server uptime found for server: {0}, Timer: {1}, Player: {2}",
                            currentServer.ServerName,
                            TimerDefinition,
                            Player));
                }
                else
                {
                    Logger.Warn(string.Format("no current server found for Timer: {0}, Player: {1}",
                        TimerDefinition,
                        Player));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during UpdateDateOfLastCooldownReset, " + TimerDefinition + ", " + Player);
            }
        }

        void RevalidateMeditHistory()
        {
            DateTime lastValidEntry = new DateTime(0);
            TimeSpan currentCooldownTimeSpan = ShortMeditCooldown;
            int countThisReset = 0;
            //validate entries
            foreach (MeditHistoryEntry entry in meditHistory)
            {
                entry.Valid = false;

                // entries before last cooldown reset should be discarded
                if (entry.EntryDateTime > cooldownResetSince)
                {
                    if (entry.EntryType == MeditHistoryEntryTypes.LongCooldownTrigger)
                    {
                        currentCooldownTimeSpan = LongMeditCooldown;
                        this.isLongMeditCooldown = true;
                    }

                    if (entry.EntryDateTime > lastValidEntry + currentCooldownTimeSpan)
                    {
                        entry.Valid = true;
                        lastValidEntry = entry.EntryDateTime;
                    }
                }
                else if (entry.EntryDateTime > cooldownResetSince - TimeSpan.FromMinutes(30))
                {
                    entry.ThisShouldTriggerCooldownButNotCountForThisUptimeWindow = true;
                }

                if (entry.Valid)
                {
                    countThisReset++;
                }
            }
            View.SetMeditCount(countThisReset);

            if (currentCooldownTimeSpan == ShortMeditCooldown) this.isLongMeditCooldown = false;

        }

        void UpdateNextMeditDate()
        {
            if (meditState == MeditationStates.Limited)
            {
                if (isLongMeditCooldown)
                {
                    NextMeditationDate = FindLastValidMeditInHistory() + LongMeditCooldown;
                }
                else
                {
                    NextMeditationDate = FindLastValidMeditInHistory() + ShortMeditCooldown;
                }

                if (NextMeditationDate > cooldownResetSince + TimeSpan.FromDays(1))
                {
                    NextMeditationDate = cooldownResetSince + TimeSpan.FromDays(1);
                }
            }
            else this.NextMeditationDate = DateTime.Now;
        }

        DateTime FindLastValidMeditInHistory()
        {
            if (meditHistory.Count > 0)
            {
                for (int i = meditHistory.Count - 1; i >= 0; i--)
                {
                    if (meditHistory[i].EntryType == MeditHistoryEntryTypes.Meditation)
                    {
                        if (meditHistory[i].Valid) return meditHistory[i].EntryDateTime;
                    }
                }
                // due to a change in wurm, cooldowns do not appear to reset immediatelly after uptime,
                // if nothing found, we need to apply cooldown based on any medits just prior to cooldown reset
                // currently it doesnt care if last medit was short or long cooldown and always applies 30 min
                // this may need adjustment after proper testing, which frankly is a pain in the ass to do
                for (int i = meditHistory.Count - 1; i >= 0; i--)
                {
                    if (meditHistory[i].EntryType == MeditHistoryEntryTypes.Meditation)
                    {
                        if (meditHistory[i].ThisShouldTriggerCooldownButNotCountForThisUptimeWindow) return meditHistory[i].EntryDateTime;
                    }
                }
            }
            return new DateTime(0);
        }
    }
}
