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
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Prayer;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Prayer
{
    [PersistentObject("TimersFeature_PrayerTimer")]
    public class PrayerTimer : WurmTimer
    {
        [JsonProperty]
        public float faithLevel = 0;
        [JsonProperty]
        public DateTime faithLevelLastCheckup = DateTime.MinValue;
        [JsonProperty]
        public FavorTimerNotify.FavorTimerNotifySettings favorSettings = new FavorTimerNotify.FavorTimerNotifySettings();
        [JsonProperty]
        public bool showFaithSkill;

        SkillEntryParser skillEntryParser;
        TriggerableAsyncOperation updatePrayerCooldownOperation;

        public PrayerTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundEngine soundEngine,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundEngine)
        {
            updatePrayerCooldownOperation = new TriggerableAsyncOperation(UpdatePrayerCooldown);
        }

        enum PrayHistoryEntryTypes { Prayed, SermonMightyPleased, FaithGainBelow120, FaithGain120orMore }

        class PrayHistoryEntry : IComparable<PrayHistoryEntry>
        {
            public PrayHistoryEntryTypes EntryType;
            public DateTime EntryDateTime;
            public bool Valid = false;

            public PrayHistoryEntry(PrayHistoryEntryTypes type, DateTime date)
            {
                this.EntryType = type;
                this.EntryDateTime = date;
            }

            public int CompareTo(PrayHistoryEntry dtlm)
            {
                return this.EntryDateTime.CompareTo(dtlm.EntryDateTime);
            }
        }

        public static TimeSpan PrayCooldown = new TimeSpan(0, 20, 0);
        List<PrayHistoryEntry> PrayerHistory = new List<PrayHistoryEntry>();
        DateTime CooldownResetSince = DateTime.MinValue;

        DateTime _nextPrayDate = DateTime.MinValue;
        DateTime NextPrayDate
        {
            get { return _nextPrayDate; }
            set
            {
                _nextPrayDate = value; 
                CDNotify.CooldownTo = value;
                if (isPrayCountMax) TimerDisplayView.ExtraInfo = " (max)";
                else TimerDisplayView.ExtraInfo = null;
            }
        }

        bool isPrayCountMax = false;

        FavorTimerNotify FavorNotify;

        float FaithLevel
        {
            get { return faithLevel; }
            set
            {
                faithLevel = value;
                FavorNotify.CurrentFavorMAX = value;
                TimerDisplayView.UpdateSkill(value);
                FlagAsChanged();
                Logger.Info(string.Format("{0} faith level is now {1} on {2}", Player, value, TimerDefinitionId));
            }
        }
        DateTime FaithLevelLastCheckup
        {
            get { return faithLevelLastCheckup; }
            set
            {
                faithLevelLastCheckup = value;
                FlagAsChanged();
            }
        }

        public bool ShowFaithSkillOnTimer
        {
            get
            {
                return showFaithSkill;
            }
            set
            {
                showFaithSkill = value;
                TimerDisplayView.ShowSkill = value;
                FlagAsChanged();
            }
        }

        public FavorTimerNotify.FavorTimerNotifySettings FavorSettings
        {
            get { return favorSettings; }
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            TimerDisplayView.SetCooldown(PrayCooldown);
            MoreOptionsAvailable = true;

            skillEntryParser = new SkillEntryParser(WurmApi);

            FavorNotify = new FavorTimerNotify(this, Player, TimerDefinitionId.ServerGroupId, Logger, SoundEngine, TrayPopups, skillEntryParser);

            TimerDisplayView.UpdateSkill(FaithLevel);
            TimerDisplayView.ShowSkill = ShowFaithSkillOnTimer;

            PerformAsyncInits();
        }

        async void PerformAsyncInits()
        {
            try
            {
                float skill = await TryGetSkillFromLogHistoryAsync("Faith", FaithLevelLastCheckup);
                if (skill > 0)
                {
                    FaithLevel = skill;
                }
                else if (FaithLevel == 0)
                {
                    Logger.Info("faith was 0 while preparing prayer timer for player: " + Player + ". Attempting 1-year thorough search");
                    skill = await TryGetSkillFromLogHistoryAsync("Faith", TimeSpan.FromDays(365));
                    if (skill > 0)
                    {
                        FaithLevel = skill;
                    }
                    else
                        Logger.Info("faith still 0, giving up, this may be a bug or no faith gained yet");
                }
                FaithLevelLastCheckup = DateTime.Now;

                List<LogEntry> lines = await GetLogLinesFromLogHistoryAsync(LogType.Skills, TimeSpan.FromDays(3));

                foreach (LogEntry line in lines)
                {
                    if (line.Content.Contains("Faith increased"))
                    {
                        var info = skillEntryParser.TryParseSkillInfoFromLogLine(line);

                        if (info != null && info.Gain != null)
                        {
                            float faithskillgain = info.Gain.Value;
                            DateTime datetime;

                            if (faithskillgain >= 0.120F)
                            {
                                PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGain120orMore,
                                    line.Timestamp));
                            }
                            else
                            {
                                PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGainBelow120,
                                    line.Timestamp));
                            }
                        }
                    }
                }

                lines = await GetLogLinesFromLogHistoryAsync(LogType.Event, TimeSpan.FromDays(3));

                foreach (LogEntry line in lines)
                {
                    if (line.Content.Contains("You feel sincere devotion")
                        || line.Content.Contains("Was that a sudden gust of wind") 
                        || line.Content.Contains("You are relying on") 
                        || line.Content.Contains("feel that an envoy of") 
                        || line.Content.Contains("Deep in your heart") 
                        || line.Content.Contains("You feel calm and solemn")
                        || line.Content.Contains("is here with you now."))
                    {
                        PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.Prayed, line.Timestamp));
                    }
                    else if (line.Content.Contains("is mighty pleased with you"))
                    {
                        PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.SermonMightyPleased, line.Timestamp));
                    }
                }

                await UpdatePrayerCooldown();

                InitCompleted = true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "init problem");
            }
        }

        public void ForceUpdateFavorNotify(Guid? soundId = null, bool? popupPersistent = null)
        {
            FavorNotify.ForceUpdateNotifyHandler(soundId, popupPersistent);
        }

        public override void Update()
        {
            base.Update();
            if (TimerDisplayView.Visible) TimerDisplayView.UpdateCooldown(NextPrayDate);
            FavorNotify.Update();
        }

        protected override void HandleServerChange()
        {
            updatePrayerCooldownOperation.Trigger();
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            PrayerTimerOptionsForm ui = new PrayerTimerOptionsForm(this, form, SoundEngine);
            ui.ShowDialogCenteredOnForm(form);
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.Contains("You feel sincere devotion")
                || line.Content.Contains("Was that a sudden gust of wind") ||
                line.Content.Contains("You are relying on") || line.Content.Contains("feel that an envoy of") ||
                line.Content.Contains("Deep in your heart") || line.Content.Contains("You feel calm and solemn") ||
                line.Content.Contains("is here with you now."))
            {
                PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.Prayed, DateTime.Now));
                updatePrayerCooldownOperation.Trigger();
            }
            else if (line.Content.StartsWith("The server has been up", StringComparison.Ordinal))
                //"The server has been up 14 hours and 22 minutes."
            {
                updatePrayerCooldownOperation.Trigger();
            }
            else if (line.Content.Contains("is mighty pleased with you"))
            {
                PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.SermonMightyPleased, DateTime.Now));
                updatePrayerCooldownOperation.Trigger();
            }
        }

        public override void HandleNewSkillLogLine(LogEntry line)
        {
            FavorNotify.HandleNewSkillLogLine(line);
            // "[02:03:41] Faith increased by 0,124 to 27,020"
            if (line.Content.StartsWith("Faith increased", StringComparison.Ordinal)
                || line.Content.StartsWith("Faith decreased", StringComparison.Ordinal))
            {

                var info = skillEntryParser.TryParseSkillInfoFromLogLine(line);
                if (info != null && info.Gain != null)
                {
                    float faithskillgain = info.Gain.Value;

                    if (faithskillgain >= 0.120F)
                    {
                        PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGain120orMore, DateTime.Now));
                    }
                    else
                    {
                        PrayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGainBelow120, DateTime.Now));
                    }
                    FaithLevel = info.Value;
                }
                updatePrayerCooldownOperation.Trigger();
            }
        }

        async Task UpdatePrayerCooldown()
        {
            try
            {
                await UpdateDateOfLastCooldownReset();
                RevalidateFaithHistory();
                UpdateNextPrayerDate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during UpdatePrayerCooldown");
            }
        }

        async Task UpdateDateOfLastCooldownReset()
        {
            var result = await GetLatestUptimeCooldownResetDate();
            if (result > DateTime.MinValue) CooldownResetSince = result;
        }

        void RevalidateFaithHistory()
        {
            //sort the history based on entry datetimes
            PrayerHistory.Sort();

            DateTime lastValidEntry = new DateTime(0);
            PrayHistoryEntry lastMayorSkillGain = null,
                //lastMinorSkillGain = null, //useless because very small ticks will never be logged regardless of setting
                lastMightyPleased = null,
                lastPrayer = null;
            TimeSpan currentPrayCooldownTimeSpan = PrayCooldown;
            int validPrayerCount = 0;
            this.isPrayCountMax = false;
            for (int i = 0; i < PrayerHistory.Count; i++)
            {
                PrayHistoryEntry entry = PrayerHistory[i];
                entry.Valid = false;

                if (entry.EntryDateTime > CooldownResetSince)
                {
                    if (entry.EntryType == PrayHistoryEntryTypes.Prayed) lastPrayer = entry;
                    //else if (entry.EntryType == PrayHistoryEntryTypes.FaithGainBelow120) lastMinorSkillGain = entry;
                    else if (entry.EntryType == PrayHistoryEntryTypes.SermonMightyPleased) lastMightyPleased = entry;
                    else if (entry.EntryType == PrayHistoryEntryTypes.FaithGain120orMore) lastMayorSkillGain = entry;

                    //on sermon event, check if recently there was big faith skill gain, if yes reset prayers
                    if (entry.EntryType == PrayHistoryEntryTypes.SermonMightyPleased)
                    {
                        if (lastMayorSkillGain != null
                            && lastMayorSkillGain.EntryDateTime > entry.EntryDateTime - TimeSpan.FromSeconds(15))
                        {
                            validPrayerCount = 0;
                            this.isPrayCountMax = false;
                        }
                    }
                    //on big faith skill gain, check if recently there was a sermon event, if yes reset prayers
                    else if (entry.EntryType == PrayHistoryEntryTypes.FaithGain120orMore)
                    {
                        if (lastMightyPleased != null
                            && lastMightyPleased.EntryDateTime > entry.EntryDateTime - TimeSpan.FromSeconds(15))
                        {
                            validPrayerCount = 0;
                            this.isPrayCountMax = false;
                        }
                    }
                    //on prayed, if prayer cap not reached, check if it's later than last valid prayer + cooldown, if yes, validate
                    else if (!this.isPrayCountMax
                        && entry.EntryType == PrayHistoryEntryTypes.Prayed
                        && entry.EntryDateTime > lastValidEntry + currentPrayCooldownTimeSpan)
                    {
                        entry.Valid = true;
                        validPrayerCount++;
                        lastValidEntry = entry.EntryDateTime;
                    }

                    //if prayer cap reached, set flag
                    if (validPrayerCount >= 5)
                    {
                        this.isPrayCountMax = true;
                    }
                }
            }
        }

        void UpdateNextPrayerDate()
        {
            if (isPrayCountMax)
            {
                NextPrayDate = CooldownResetSince + TimeSpan.FromDays(1);
            }
            else
            {
                NextPrayDate = FindLastValidPrayerInHistory() + PrayCooldown;
            }

            if (NextPrayDate > CooldownResetSince + TimeSpan.FromDays(1))
            {
                NextPrayDate = CooldownResetSince + TimeSpan.FromDays(1);
            }
        }

        DateTime FindLastValidPrayerInHistory()
        {
            if (PrayerHistory.Count > 0)
            {
                for (int i = PrayerHistory.Count - 1; i >= 0; i--)
                {
                    if (PrayerHistory[i].EntryType == PrayHistoryEntryTypes.Prayed)
                    {
                        if (PrayerHistory[i].Valid) return PrayerHistory[i].EntryDateTime;
                    }
                }
            }
            return new DateTime(0);
        }


        public class FavorTimerNotify
        {
            [JsonObject(MemberSerialization.OptIn)]
            public class FavorTimerNotifySettings
            {
                [JsonProperty]
                public bool FavorNotifySound = false;
                [JsonProperty]
                public bool FavorNotifyPopup = false;
                [JsonProperty]
                public Guid FavorNotifySoundId;
                [JsonProperty]
                public bool FavorNotifyWhenMax = false;
                [JsonProperty]
                public float FavorNotifyOnLevel = 0;
                [JsonProperty]
                public bool FavorNotifyPopupPersist = false;

                [Obsolete] //moved to faith timer settings
                public bool ShowFaithSkill;
            }

            public FavorTimerNotifySettings Settings { get; private set; }

            readonly NotifyHandler FavorHandler;

            public float CurrentFavorMAX { get; set; }

            float _currentFavorLevel = 0;
            float CurrentFavorLevel
            {
                get { return _currentFavorLevel; }
                set
                {
                    CheckIfNotify(_currentFavorLevel, value);
                    _currentFavorLevel = value;
                }
            }

            ServerGroupId Group;

            readonly SkillEntryParser skillEntryParser;

            public FavorTimerNotify(PrayerTimer timer, string player, ServerGroupId group, ILogger logger, ISoundEngine soundEngine, ITrayPopups trayPopups, [NotNull] SkillEntryParser skillEntryParser)
            {
                if (skillEntryParser == null) throw new ArgumentNullException("skillEntryParser");
                Group = group;
                this.skillEntryParser = skillEntryParser;
                Settings = timer.favorSettings;

                FavorHandler = new NotifyHandler(
                    logger, soundEngine, trayPopups,
                    Settings.FavorNotifySoundId,
                    player,
                    "",
                    Settings.FavorNotifyPopupPersist);

                
            }

            public void ForceUpdateNotifyHandler(Guid? soundId, bool? persistPopup = null)
            {
                if (soundId != null)
                {
                    FavorHandler.SoundId = soundId.Value;
                }
                if (persistPopup != null)
                {
                    FavorHandler.PopupPersistent = persistPopup.Value;
                }
            }

            void CheckIfNotify(float oldFavor, float newFavor)
            {
                if (Settings.FavorNotifySound || Settings.FavorNotifyPopup)
                {
                    bool notify = false;
                    if (Settings.FavorNotifyWhenMax)
                    {
                        if (oldFavor < CurrentFavorMAX && newFavor > CurrentFavorMAX) notify = true;
                    }
                    else if (oldFavor < Settings.FavorNotifyOnLevel && newFavor > Settings.FavorNotifyOnLevel) notify = true;
                    if (notify)
                    {
                        if (Settings.FavorNotifySound) FavorHandler.Play();
                        if (Settings.FavorNotifyPopup)
                        {
                            FavorHandler.Message = string.Format("Favor on {0} reached {1}", this.Group, newFavor);
                            FavorHandler.Show();
                        }
                    }
                }
            }

            public void HandleNewSkillLogLine(LogEntry line)
            {
                if (line.Content.StartsWith("Favor increased"))
                {
                    var info = skillEntryParser.TryParseSkillInfoFromLogLine(line);
                    if (info != null)
                    {
                        CurrentFavorLevel = info.Value;
                    }
                }
            }

            public void Update()
            {
                FavorHandler.Update();
            }
        }
    }
}
