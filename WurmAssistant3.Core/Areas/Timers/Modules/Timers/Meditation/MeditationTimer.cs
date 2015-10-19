using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Meditation;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Meditation
{
    [PersistentObject("TimersFeature_MeditationTimer")]
    public class MeditationTimer : WurmTimer
    {
        const double FloatAboveZeroCompareValue = 0.00001;

        [JsonProperty]
        public DateTime meditSkillLastCheckup;
        [JsonProperty]
        public float meditationSkill;
        [JsonProperty]
        public bool sleepBonusReminder;
        [JsonProperty]
        public int sleepBonusPopupDuration;
        [JsonProperty]
        public bool showMeditSkill;
        [JsonProperty]
        public bool showMeditCount;

        enum MeditationStates { Unlimited, Limited }
        enum MeditHistoryEntryTypes { Meditation, LongCooldownTrigger }


        public MeditationTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
            sleepBonusPopupDuration = 4000;
        }

        class MeditHistoryEntry
        {
            public MeditHistoryEntryTypes EntryType;
            public DateTime EntryDateTime;
            public bool Valid = false;
            /// <summary>
            /// this is a bandaid fix flag for uptime not reseting medit cooldown
            /// </summary>
            /// <remarks>
            /// due to recent change in wurm, cooldowns do not appear to reset immediatelly after uptime
            /// it is unknown if this is some delay, random issue or the cooldown just simply doesnt reset on uptime swap
            /// this flag can be used to mark an entry, that happened just prior to cooldown reset
            /// in can then be checked and entry returned as cooldown trigger, if no more recent entries are found,
            /// this in effect triggers a cooldown but does not mess the "Valid" flag, which is used to count medits during
            /// single uptime window (so that long/short cooldown count remains correct).
            /// </remarks>
            public bool ThisShouldTriggerCooldownButNotCountForThisUptimeWindow = false;

            public MeditHistoryEntry(MeditHistoryEntryTypes type, DateTime date)
            {
                this.EntryType = type;
                this.EntryDateTime = date;
            }
        }

        static TimeSpan LongMeditCooldown = new TimeSpan(3, 0, 0);
        static TimeSpan ShortMeditCooldown = new TimeSpan(0, 30, 0);

        List<MeditHistoryEntry> MeditHistory = new List<MeditHistoryEntry>();

        DateTime _nextMeditationDate = DateTime.MinValue;
        DateTime NextMeditationDate
        {
            get { return _nextMeditationDate; }
            set { _nextMeditationDate = value; CDNotify.CooldownTo = value; }
        }

        DateTime CooldownResetSince = DateTime.Now;
        TimeSpan TimeOnThisCooldownReset = new TimeSpan(0);
        bool isLongMeditCooldown = false;

        private MeditationStates MeditState = MeditationStates.Unlimited;

        SleepBonusNotify SleepNotify;

        //persist
        //DateTime MeditSkillLastCheckup = DateTime.MinValue;
        //persist
        //float _meditationSkill = 0;

        public bool ShowMeditCount
        {
            get { return showMeditCount; }
            set
            {
                TimerDisplayView.ShowMeditCount = value;
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
                Logger.Info(string.Format("{0} meditation skill is now {1} on {2}", Player, value, TimerDefinitionId));
                TimerDisplayView.UpdateSkill(value);
                if (value < 20f)
                {
                    if (MeditState != MeditationStates.Unlimited)
                    {
                        UpdateMeditationCooldown();
                        MeditState = MeditationStates.Unlimited;
                    }
                }
                else
                {
                    if (MeditState != MeditationStates.Limited)
                    {
                        UpdateMeditationCooldown();
                        MeditState = MeditationStates.Limited;
                    }
                }
                meditationSkill = value;
                FlagAsChanged();
            }
        }

        public bool SleepBonusReminder
        {
            get { return sleepBonusReminder; }
            set
            {
                SleepNotify.Enabled = value;
                sleepBonusReminder = value;
                FlagAsChanged();
            }
        }

        /// <summary>
        /// milliseconds
        /// </summary>
        public int SleepBonusPopupDuration
        {
            get { return sleepBonusPopupDuration; }
            set
            {
                SleepNotify.PopupDuration = value;
                sleepBonusPopupDuration = value;
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
                TimerDisplayView.ShowSkill = value;
            }
        }

        SkillEntryParser skillEntryParser;

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            //more inits
            TimerDisplayView.SetCooldown(ShortMeditCooldown);

            skillEntryParser = new SkillEntryParser(WurmApi);

            SleepNotify = new SleepBonusNotify(Logger, SoundEngine, TrayPopups, Player, "Can turn off sleep bonus now");
            SleepNotify.Enabled = SleepBonusReminder;

            TimerDisplayView.UpdateSkill(MeditationSkill);
            TimerDisplayView.ShowSkill = ShowMeditSkill;
            TimerDisplayView.ShowMeditCount = ShowMeditCount;

            MoreOptionsAvailable = true;
            PerformAsyncInits();
        }

        async Task PerformAsyncInits()
        {
            try
            {
                var hasArchivalLevel = MeditationSkill > FloatAboveZeroCompareValue;
                float skill = await FindMeditationSkill(hasArchivalLevel);

                if (skill > FloatAboveZeroCompareValue)
                {
                    MeditationSkill = skill;
                }
                else
                {
                    // forcing update of the timer and "limited" flag
                    MeditationSkill = MeditationSkill;
                }
                MeditSkillLastCheckup = DateTime.Now;

                List<LogEntry> historyEntries = await GetLogLinesFromLogHistoryAsync(LogType.Event, TimeSpan.FromDays(3));
                foreach (LogEntry line in historyEntries)
                {
                    if (line.Content.Contains("You finish your meditation"))
                    {
                         MeditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.Meditation, line.Timestamp));
                    }
                    else if (line.Content.Contains("You feel that it will take you a while before you are ready to meditate again"))
                    {
                        MeditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.LongCooldownTrigger, line.Timestamp));
                    }
                }

                UpdateMeditationCooldown();

                //now new log events can be handled
                InitCompleted = true;
            }
            catch (Exception _e)
            {
                Logger.Error(_e, "problem while preparing timer");
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
                            TimerDefinitionId));
                    skill = await TryGetSkillFromLogHistoryAsync("Meditating", TimeSpan.FromDays(365));
                    if (skill < FloatAboveZeroCompareValue)
                    {
                        skill = await TryGetSkillFromLogHistoryAsync("Meditating", TimeSpan.FromDays(1460));
                        if (skill < FloatAboveZeroCompareValue)
                        {
                            Logger.Info(string.Format("could not get any meditation skill for player: {0} server group: {1}", Player, TimerDefinitionId));
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

        public override void Update(bool engineSleeping)
        {
            base.Update(engineSleeping);
            SleepNotify.Update();
            if (TimerDisplayView.Visible) TimerDisplayView.UpdateCooldown(NextMeditationDate - DateTime.Now);
        }

        //this happens only if new server is of current group

        //this includes when player is coming back to this timer group!

        protected override void HandleServerChange()
        {
            UpdateMeditationCooldown();
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            MeditationTimerOptionsForm moreOptUI = new MeditationTimerOptionsForm(this, form);
            moreOptUI.ShowDialog();
        }

        //ported 1.x methods

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("You finish your meditation", StringComparison.Ordinal))
            {
                MeditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.Meditation, DateTime.Now));
                UpdateMeditationCooldown();
                SleepNotify.MeditationHappened();
            }
            else if (line.Content.StartsWith("The server has been up", StringComparison.Ordinal)) //"The server has been up 14 hours and 22 minutes."
            {
                //this is no longer needed because of HandleServerChange
                //which fires every time player logs into a server (start new wurm or relog or server travel)
                UpdateMeditationCooldown();
            }
            else if (line.Content.StartsWith("You start using the sleep bonus", StringComparison.Ordinal))
            {
                SleepNotify.SleepBonusActivated();
            }
            else if (line.Content.StartsWith("You refrain from using the sleep bonus", StringComparison.Ordinal))
            {
                SleepNotify.SleepBonusDeactivated();
            }
            //[04:31:56] You feel that it will take you a while before you are ready to meditate again.
            else if (line.Content.StartsWith("You feel that it will take", StringComparison.Ordinal))
            {
                if (line.Content.StartsWith("You feel that it will take you a while before you are ready to meditate again", StringComparison.Ordinal))
                {
                    MeditHistory.Add(new MeditHistoryEntry(MeditHistoryEntryTypes.LongCooldownTrigger, DateTime.Now));
                    UpdateMeditationCooldown();
                }
            }
        }

        void UpdateMeditationCooldown()
        {
            UpdateDateOfLastCooldownReset();
            RevalidateMeditHistory();
            UpdateNextMeditDate();
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

        void UpdateDateOfLastCooldownReset()
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                DateTime cooldownResetDate = currentTime;

                //todo reimpl: these are blocking calls!
                var currentServer = WurmApi.Characters.Get(Player).TryGetCurrentServer();
                if (currentServer != null)
                {
                    var uptime = currentServer.TryGetCurrentUptime();
                    if (uptime != null)
                    {
                        TimeSpan timeSinceLastServerReset = uptime.Value;
                        TimeSpan daysSinceLastServerReset = new TimeSpan(timeSinceLastServerReset.Days, 0, 0, 0);
                        timeSinceLastServerReset = timeSinceLastServerReset.Subtract(daysSinceLastServerReset);

                        cooldownResetDate = currentTime - timeSinceLastServerReset;
                        this.CooldownResetSince = cooldownResetDate;
                    }
                    else
                        Logger.Warn(string.Format("no server uptime found for server: {0}, Timer: {1}, Player: {2}",
                            currentServer.ServerName,
                            TimerDefinitionId,
                            Player));
                }
                else
                {
                    Logger.Warn(string.Format("no current server found for Timer: {0}, Player: {1}",
                        TimerDefinitionId,
                        Player));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during UpdateDateOfLastCooldownReset, " + TimerDefinitionId + ", " + Player);
            }
        }

        void RevalidateMeditHistory()
        {
            DateTime lastValidEntry = new DateTime(0);
            TimeSpan currentCooldownTimeSpan = ShortMeditCooldown;
            int countThisReset = 0;
            //validate entries
            foreach (MeditHistoryEntry entry in MeditHistory)
            {
                entry.Valid = false;

                // all entries are default invalid
                // discard any entry prior to cooldown reset
                if (entry.EntryDateTime > CooldownResetSince)
                {
                    if (entry.EntryType == MeditHistoryEntryTypes.LongCooldownTrigger)
                    {
                        //apply longer cooldown from this point
                        currentCooldownTimeSpan = LongMeditCooldown;
                        this.isLongMeditCooldown = true;
                    }

                    // if entry date is later, than last valid + cooldown period, medit counts for daily cap
                    if (entry.EntryDateTime > lastValidEntry + currentCooldownTimeSpan)
                    {
                        entry.Valid = true;
                        lastValidEntry = entry.EntryDateTime;
                    }
                }
                else if (entry.EntryDateTime > CooldownResetSince - TimeSpan.FromMinutes(30))
                {
                    entry.ThisShouldTriggerCooldownButNotCountForThisUptimeWindow = true;
                }

                if (entry.Valid)
                {
                    countThisReset++;
                }
            }
            TimerDisplayView.SetMeditCount(countThisReset);
            // resets medit cooldown type in case long is set from previous uptime period
            if (currentCooldownTimeSpan == ShortMeditCooldown) this.isLongMeditCooldown = false;

            //debug
            //List<string> dumphistory = new List<string>();
            //foreach (MeditHistoryEntry entry in MeditHistory)
            //{
            //    dumphistory.Add(entry.EntryDateTime.ToString() + ", " + entry.EntryType.ToString() + ", " + entry.Valid.ToString());
            //}
            //DebugDump.DumpToTextFile("meditvalidatedlist.txt", dumphistory);
        }

        void UpdateNextMeditDate()
        {
            if (MeditState == MeditationStates.Limited)
            {
                if (isLongMeditCooldown)
                {
                    NextMeditationDate = FindLastValidMeditInHistory() + LongMeditCooldown;
                }
                else
                {
                    NextMeditationDate = FindLastValidMeditInHistory() + ShortMeditCooldown;
                }

                if (NextMeditationDate > CooldownResetSince + TimeSpan.FromDays(1))
                {
                    NextMeditationDate = CooldownResetSince + TimeSpan.FromDays(1);
                }
            }
            else this.NextMeditationDate = DateTime.Now;
        }

        DateTime FindLastValidMeditInHistory()
        {
            if (MeditHistory.Count > 0)
            {
                for (int i = MeditHistory.Count - 1; i >= 0; i--)
                {
                    if (MeditHistory[i].EntryType == MeditHistoryEntryTypes.Meditation)
                    {
                        if (MeditHistory[i].Valid) return MeditHistory[i].EntryDateTime;
                    }
                }
                // due to recent change in wurm, cooldowns do not appear to reset immediatelly after uptime
                // it is 
                // if nothing found, we need to apply cooldown based on any medits just prior to cooldown reset
                // currently it doesnt care if last medit was short or long cooldown and always applies 30 min
                // this may need adjustment after proper testing, which frankly is a pain in the ass to do
                for (int i = MeditHistory.Count - 1; i >= 0; i--)
                {
                    if (MeditHistory[i].EntryType == MeditHistoryEntryTypes.Meditation)
                    {
                        if (MeditHistory[i].ThisShouldTriggerCooldownButNotCountForThisUptimeWindow) return MeditHistory[i].EntryDateTime;
                    }
                }
            }
            return new DateTime(0);
        }

        class SleepBonusNotify
        {
            NotifyHandler Handler;

            DateTime? SleepBonusStarted = null;
            DateTime? LastMeditHappenedOn = DateTime.MinValue;

            bool meditHappened = false;

            public bool Enabled { get; set; }
            public int PopupDuration { get { return Handler.Duration; } set { Handler.Duration = value; } }

            public SleepBonusNotify(ILogger logger, ISoundEngine soundEngine, ITrayPopups trayPopups, string popupTitle,
                string popupMessage, bool popupPersistent = false)
            {
                Handler = new NotifyHandler(logger, soundEngine, trayPopups);
                Handler.PopupPersistent = popupPersistent;
                Handler.Message = (popupMessage ?? "");
                Handler.Title = (popupTitle ?? "");
            }

            public void Update()
            {
                if (SleepBonusStarted != null && meditHappened && DateTime.Now > SleepBonusStarted + TimeSpan.FromMinutes(5))
                {
                    if (LastMeditHappenedOn > DateTime.Now - TimeSpan.FromMinutes(10))
                    {
                        if (Enabled) Handler.Show();
                    }
                    meditHappened = false;
                }
                Handler.Update();
            }

            public void SleepBonusActivated()
            {
                meditHappened = false;
                SleepBonusStarted = DateTime.Now;
            }

            public void SleepBonusDeactivated()
            {
                SleepBonusStarted = null;
                meditHappened = false;
            }

            public void MeditationHappened()
            {
                LastMeditHappenedOn = DateTime.Now;
                meditHappened = true;
            }
        }
    }
}
