using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldursLab.Essentials.Asynchronous;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Timers.Prayer
{
    [KernelBind, PersistentObject("TimersFeature_PrayerTimer")]
    public class PrayerTimer : WurmTimer
    {
        class PrayHistoryEntry : IComparable<PrayHistoryEntry>
        {
            public PrayHistoryEntryTypes EntryType { get; private set; }
            public DateTime EntryDateTime { get; private set; }
            public bool Valid { get; set; }

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

        enum PrayHistoryEntryTypes { Prayed, SermonMightyPleased, FaithGainBelow120, FaithGain120OrMore }

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
            }

            public FavorTimerNotifySettings Settings { get; private set; }

            readonly NotifyHandler favorHandler;

            public float CurrentFavorMax { get; set; }

            float currentFavorLevel = 0;
            float CurrentFavorLevel
            {
                get { return currentFavorLevel; }
                set
                {
                    CheckIfNotify(currentFavorLevel, value);
                    currentFavorLevel = value;
                }
            }

            readonly string serverGroupId;

            readonly SkillEntryParser skillEntryParser;

            public FavorTimerNotify(PrayerTimer timer, string player, string serverGroupId, ILogger logger,
                ISoundManager soundManager, ITrayPopups trayPopups, [NotNull] SkillEntryParser skillEntryParser)
            {
                if (skillEntryParser == null)
                    throw new ArgumentNullException("skillEntryParser");
                this.serverGroupId = serverGroupId;
                this.skillEntryParser = skillEntryParser;
                Settings = timer.favorSettings;

                favorHandler = new NotifyHandler(
                    logger,
                    soundManager,
                    trayPopups,
                    Settings.FavorNotifySoundId,
                    player,
                    "",
                    Settings.FavorNotifyPopupPersist);
            }

            public void ForceUpdateNotifyHandler(Guid? soundId, bool? persistPopup = null)
            {
                if (soundId != null)
                {
                    favorHandler.SoundId = soundId.Value;
                }
                if (persistPopup != null)
                {
                    favorHandler.PopupPersistent = persistPopup.Value;
                }
            }

            void CheckIfNotify(float oldFavor, float newFavor)
            {
                if (Settings.FavorNotifySound || Settings.FavorNotifyPopup)
                {
                    bool notify = false;
                    if (Settings.FavorNotifyWhenMax)
                    {
                        if (oldFavor < CurrentFavorMax && newFavor > CurrentFavorMax)
                            notify = true;
                    }
                    else if (oldFavor < Settings.FavorNotifyOnLevel && newFavor > Settings.FavorNotifyOnLevel)
                        notify = true;
                    if (notify)
                    {
                        if (Settings.FavorNotifySound)
                            favorHandler.Play();
                        if (Settings.FavorNotifyPopup)
                        {
                            favorHandler.Message = string.Format("Favor on {0} reached {1}", this.serverGroupId, newFavor);
                            favorHandler.Show();
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
                favorHandler.Update();
            }
        }

        static readonly TimeSpan PrayCooldown = new TimeSpan(0, 20, 0);

        [JsonProperty]
        float faithLevel;

        [JsonProperty]
        DateTime faithLevelLastCheckup = DateTime.MinValue;

        [JsonProperty]
        readonly FavorTimerNotify.FavorTimerNotifySettings favorSettings = new FavorTimerNotify.FavorTimerNotifySettings();

        [JsonProperty]
        bool showFaithSkill;

        readonly List<PrayHistoryEntry> prayerHistory = new List<PrayHistoryEntry>();
        readonly TriggerableAsyncOperation updatePrayerCooldownOperation;

        DateTime cooldownResetSince = DateTime.MinValue;
        DateTime nextPrayDate = DateTime.MinValue;
        bool isPrayCountMax = false;

        FavorTimerNotify favorNotify;
        SkillEntryParser skillEntryParser;

        public PrayerTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundManager soundManager,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundManager)
        {
            updatePrayerCooldownOperation = new TriggerableAsyncOperation(UpdatePrayerCooldown);
        }


        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            View.SetCooldown(PrayCooldown);
            MoreOptionsAvailable = true;

            skillEntryParser = new SkillEntryParser(WurmApi);

            favorNotify = new FavorTimerNotify(this, Character, ServerGroupId, Logger, SoundManager, TrayPopups, skillEntryParser);

            View.UpdateSkill(FaithLevel);
            View.ShowSkill = ShowFaithSkillOnTimer;

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
                else if (Math.Abs(FaithLevel) < 0.00001f)
                {
                    Logger.Info("faith was 0 while preparing prayer timer for player: " + Character + ". Attempting 1-year thorough search");
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

                            if (faithskillgain >= 0.120F)
                            {
                                prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGain120OrMore,
                                    line.Timestamp));
                            }
                            else
                            {
                                prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGainBelow120,
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
                        prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.Prayed, line.Timestamp));
                    }
                    else if (line.Content.Contains("is mighty pleased with you"))
                    {
                        prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.SermonMightyPleased, line.Timestamp));
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

        DateTime NextPrayDate
        {
            get { return nextPrayDate; }
            set
            {
                nextPrayDate = value; 
                CDNotify.CooldownTo = value;
                if (isPrayCountMax)
                    View.ExtraInfo = " (max)";
                else
                    View.ExtraInfo = null;
            }
        }

        float FaithLevel
        {
            get { return faithLevel; }
            set
            {
                faithLevel = value;
                favorNotify.CurrentFavorMax = value;
                View.UpdateSkill(value);
                FlagAsChanged();
                Logger.Info(string.Format("{0} faith level is now {1} on {2}", Character, value, ServerGroupId));
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
                View.ShowSkill = value;
                FlagAsChanged();
            }
        }

        public FavorTimerNotify.FavorTimerNotifySettings FavorSettings
        {
            get { return favorSettings; }
        }

        public void ForceUpdateFavorNotify(Guid? soundId = null, bool? popupPersistent = null)
        {
            favorNotify.ForceUpdateNotifyHandler(soundId, popupPersistent);
        }

        public override void Update()
        {
            base.Update();
            if (View.Visible)
            {
                View.SetCooldown(PrayCooldown);
                View.UpdateCooldown(NextPrayDate);
            }
            favorNotify.Update();
        }

        protected override void HandleServerChange()
        {
            updatePrayerCooldownOperation.Trigger();
        }

        public override void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
            PrayerTimerOptionsForm ui = new PrayerTimerOptionsForm(this, form, SoundManager);
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
                prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.Prayed, DateTime.Now));
                updatePrayerCooldownOperation.Trigger();
            }
            else if (line.Content.StartsWith("The server has been up", StringComparison.Ordinal))
            //"The server has been up 14 hours and 22 minutes."
            {
                updatePrayerCooldownOperation.Trigger();
            }
            else if (line.Content.Contains("is mighty pleased with you"))
            {
                prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.SermonMightyPleased, DateTime.Now));
                updatePrayerCooldownOperation.Trigger();
            }
        }

        public override void HandleNewSkillLogLine(LogEntry line)
        {
            Logger.Debug($"Timer: {this}; processing skill log message: {line}");

            favorNotify.HandleNewSkillLogLine(line);
            // "[02:03:41] Faith increased by 0,124 to 27,020"
            if (line.Content.StartsWith("Faith increased", StringComparison.Ordinal)
                || line.Content.StartsWith("Faith decreased", StringComparison.Ordinal))
            {
                Logger.Debug($"Timer: {this}; skill log message ident as faith gain: {line}");
                var info = skillEntryParser.TryParseSkillInfoFromLogLine(line);
                if (info != null && info.Gain != null)
                {
                    float faithskillgain = info.Gain.Value;

                    Logger.Debug($"Timer: {this}; skill log faith gain message parsed: {line}; faith gain: {info.Gain.Value}");

                    if (faithskillgain >= 0.120F)
                    {
                        prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGain120OrMore, DateTime.Now));
                    }
                    else
                    {
                        prayerHistory.Add(new PrayHistoryEntry(PrayHistoryEntryTypes.FaithGainBelow120, DateTime.Now));
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
            if (result > DateTime.MinValue) cooldownResetSince = result;
        }

        void RevalidateFaithHistory()
        {
            //sort the history based on entry datetimes
            prayerHistory.Sort();

            DateTime lastValidEntry = new DateTime(0);
            PrayHistoryEntry 
                lastMayorSkillGain = null,
                lastMightyPleased = null,
                lastPrayer = null;
            TimeSpan currentPrayCooldownTimeSpan = PrayCooldown;
            int validPrayerCount = 0;
            this.isPrayCountMax = false;
            for (int i = 0; i < prayerHistory.Count; i++)
            {
                PrayHistoryEntry entry = prayerHistory[i];
                entry.Valid = false;

                if (entry.EntryDateTime > cooldownResetSince)
                {
                    if (entry.EntryType == PrayHistoryEntryTypes.Prayed) lastPrayer = entry;
                    //else if (entry.EntryType == PrayHistoryEntryTypes.FaithGainBelow120) lastMinorSkillGain = entry;
                    else if (entry.EntryType == PrayHistoryEntryTypes.SermonMightyPleased) lastMightyPleased = entry;
                    else if (entry.EntryType == PrayHistoryEntryTypes.FaithGain120OrMore) lastMayorSkillGain = entry;

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
                    else if (entry.EntryType == PrayHistoryEntryTypes.FaithGain120OrMore)
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
                NextPrayDate = cooldownResetSince + TimeSpan.FromDays(1);
            }
            else
            {
                NextPrayDate = FindLastValidPrayerInHistory() + PrayCooldown;
            }

            if (NextPrayDate > cooldownResetSince + TimeSpan.FromDays(1))
            {
                NextPrayDate = cooldownResetSince + TimeSpan.FromDays(1);
            }
        }

        DateTime FindLastValidPrayerInHistory()
        {
            if (prayerHistory.Count > 0)
            {
                for (int i = prayerHistory.Count - 1; i >= 0; i--)
                {
                    if (prayerHistory[i].EntryType == PrayHistoryEntryTypes.Prayed)
                    {
                        if (prayerHistory[i].Valid) return prayerHistory[i].EntryDateTime;
                    }
                }
            }
            return new DateTime(0);
        }
    }
}
