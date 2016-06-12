using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Services;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers
{
    public abstract class WurmTimer : PersistentObjectBase
    {
        protected readonly ITrayPopups TrayPopups;
        protected readonly ILogger Logger;
        protected readonly IWurmApi WurmApi;
        protected readonly ISoundManager SoundManager;

        PlayerTimersGroup playerTimersGroup;
        protected string Player;

        protected CooldownHandler CDNotify;

        IWurmCharacter character;

        [JsonProperty]
        bool soundNotify;
        [JsonProperty]
        bool popupNotify;
        [JsonProperty]
        bool persistentPopup;
        [JsonProperty]
        Guid soundId;
        [JsonProperty]
        bool popupOnWaLaunch;
        [JsonProperty]
        int popupDurationMillis = 4000;
        [JsonProperty]
        TimeSpan lastUptimeSnapshot;

        public WurmTimer(string persistentObjectId, [NotNull] ITrayPopups trayPopups, [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi, [NotNull] ISoundManager soundManager) : base(persistentObjectId)
        {
            Id = Guid.Parse(persistentObjectId);
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            this.TrayPopups = trayPopups;
            this.Logger = logger;
            this.WurmApi = wurmApi;
            this.SoundManager = soundManager;

        }

        public virtual void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            //derived must call this base before their own inits!
            TimerDefinition = definition;
            Name = definition.ToString();
            ShortName = definition.ToString();
            playerTimersGroup = parentGroup;
            Player = player;

            View = new TimerDisplayView(this);

            CDNotify = new CooldownHandler(Logger, SoundManager, TrayPopups)
            {
                DurationMillis = PopupDurationMillis,
                Title = Player,
                Message = Name + " cooldown finished",
                SoundEnabled = SoundNotify,
                PopupEnabled = PopupNotify,
                PersistentPopup = PersistentPopup,
                SoundId = SoundId
            };
            if (PopupOnWaLaunch) CDNotify.ResetShownAndPlayed();

            character = WurmApi.Characters.Get(new CharacterName(player));
            character.LogInOrCurrentServerPotentiallyChanged += _handleServerChange;
        }

        public TimerDisplayView View { get; private set; }

        public Guid Id { get; private set; }

        public TimerDefinition TimerDefinition { get; private set; }

        public TimersFeature TimersFeature { get { return playerTimersGroup.TimersFeature; } }

        public string ServerGroupId { get { return playerTimersGroup.ServerGroupId; } }

        public bool SoundNotify
        {
            get { return soundNotify; }
            set { soundNotify = value; CDNotify.SoundEnabled = value; FlagAsChanged(); }
        }

        public bool PopupNotify
        {
            get { return popupNotify; }
            set { popupNotify = value; CDNotify.PopupEnabled = value; FlagAsChanged(); }
        }

        public bool PersistentPopup
        {
            get { return persistentPopup; }
            set { persistentPopup = value; CDNotify.PersistentPopup = value; FlagAsChanged(); }
        }

        public Guid SoundId
        {
            get { return soundId; }
            set { soundId = value; CDNotify.SoundId = value; FlagAsChanged(); }
        }

        public bool PopupOnWaLaunch
        {
            get { return popupOnWaLaunch; }
            set { popupOnWaLaunch = value; FlagAsChanged(); }
        }

        public int PopupDurationMillis
        {
            get { return popupDurationMillis; }
            set { popupDurationMillis = value; CDNotify.DurationMillis = value; FlagAsChanged(); }
        }

        public TimeSpan LastUptimeSnapshot
        {
            get { return lastUptimeSnapshot; }
            set { lastUptimeSnapshot = value; FlagAsChanged(); }
        }

        public string Name { get; private set; }

        public string ShortName { get; private set; }

        /// <summary>
        /// set true to enable "more options" button in default timer settings
        /// </summary>
        public bool MoreOptionsAvailable { get; protected set; }

        /// <summary>
        /// set true in derived timer when its fully initialized and ready to receive updates
        /// </summary>
        public bool InitCompleted { get; protected set; }

        /// <summary>
        /// set true to run update on this timer regardless if InitCompleted is flagged true
        /// </summary>
        public bool RunUpdateRegardlessOfInitCompleted { get; protected set; }

        public string Character
        {
            get { return Player; }
        }

        public virtual void Stop()
        {
            //children should basecall this after their own cleanup
            playerTimersGroup.StopTimer(this);
            View.Dispose();
            character.LogInOrCurrentServerPotentiallyChanged -= _handleServerChange;
        }

        public virtual void Update()
        {
            CDNotify.Update();
        }

        /// <summary>
        /// Returns 0 if no finds or error, filters out other server groups,
        /// min/max search date is unbound using this overload
        /// </summary>
        /// <param name="skillName">case sens</param>
        /// <param name="since"></param>
        /// <returns></returns>
        protected async Task<float> TryGetSkillFromLogHistoryAsync(string skillName, TimeSpan since)
        {
            var skillLevel =
                await
                    character.Skills.TryGetCurrentSkillLevelAsync(skillName,
                        new ServerGroup(playerTimersGroup.ServerGroupId),
                        since);

            return skillLevel != null ? skillLevel.Value : 0;
        }

        /// <summary>
        /// Returns 0 if no finds or error, filters out other server groups
        /// min search 7 days ago, max search 120 days ago
        /// </summary>
        /// <param name="skillName">case sens</param>
        /// <param name="lastCheckup"></param>
        /// <returns></returns>
        protected async Task<float> TryGetSkillFromLogHistoryAsync(string skillName, DateTime lastCheckup)
        {

            var skillLevel =
                await
                    character.Skills.TryGetCurrentSkillLevelAsync(skillName,
                        new ServerGroup(playerTimersGroup.ServerGroupId),
                        HowLongAgoWasThisDate(lastCheckup));

            return skillLevel != null ? skillLevel.Value : 0;
        }

        /// <summary>
        /// returns null on error, filters out other server groups,
        /// min/max search date is unbound using this overload
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="since">amount of time to look back, will accept any value</param>
        /// <returns></returns>
        protected async Task<List<LogEntry>> GetLogLinesFromLogHistoryAsync(LogType logType, TimeSpan since)
        {
            var results = await character.Logs.ScanLogsServerGroupRestrictedAsync(DateTime.Now - since,
                DateTime.Now,
                logType,
                new ServerGroup(playerTimersGroup.ServerGroupId));

            var result = results.ToList();
            return result;
        }

        /// <summary>
        /// returns null on error, filters out other server groups
        /// min search 7 days ago, max search 120 days ago
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="lastCheckup"></param>
        /// <returns></returns>
        protected async Task<List<LogEntry>> GetLogLinesFromLogHistoryAsync(LogType logType, DateTime lastCheckup)
        {
            return await GetLogLinesFromLogHistoryAsync(logType, HowLongAgoWasThisDate(lastCheckup));
        }

        private TimeSpan HowLongAgoWasThisDate(DateTime lastCheckup)
        {
            TimeSpan timeNotChecked = DateTime.Now - lastCheckup - TimeSpan.FromDays(1);
            return timeNotChecked.ConstrainToRange(TimeSpan.FromDays(7), TimeSpan.FromDays(365));
        }

        /// <summary>
        /// override to handle new wurm events in Event log
        /// </summary>
        /// <param name="line"></param>
        public virtual void HandleNewEventLogLine(LogEntry line)
        {
        }

        /// <summary>
        /// override to handle new wurm events in Skills log
        /// </summary>
        /// <param name="line"></param>
        public virtual void HandleNewSkillLogLine(LogEntry line)
        {
        }

        public virtual void HandleAnyLogLine(LogsMonitorEventArgs container)
        {

        }

        private void _handleServerChange(object sender, EventArgs e)
        {
            HandleServerChange();
        }

        /// <summary>
        /// triggered after wurm date and uptime info was refreshed
        /// </summary>
        protected virtual void HandleServerChange()
        {
        }

        /// <summary>
        /// opens config for default timer settings
        /// </summary>
        public void OpenTimerConfig()
        {
            TimerDefaultSettingsForm ui = new TimerDefaultSettingsForm(this, SoundManager);
            ui.ShowDialogCenteredOnForm(this.GetModuleUi());
        }

        /// <summary>
        /// override to show "more options" window
        /// </summary>
        /// <param name="form"></param>
        public virtual void OpenMoreOptions(TimerDefaultSettingsForm form)
        {
        }

        /// <summary>
        /// returns ref to main module Form
        /// </summary>
        /// <returns></returns>
        public TimersForm GetModuleUi()
        {
            return playerTimersGroup.GetModuleUI();
        }

        /// <summary>
        /// closes and disposes of this timer
        /// </summary>
        internal void TurnOff()
        {
            playerTimersGroup.RemoveTimer(this);
        }

        protected async Task<DateTime> GetLatestUptimeCooldownResetDate()
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                var server = playerTimersGroup.CurrentServerOnTheGroup;

                if (server != null)
                {
                    var serverUptime = await server.TryGetCurrentUptimeAsync();
                    if (serverUptime != null)
                    {
                        TimeSpan timeSinceLastServerReset = serverUptime.Value;
                        TimeSpan daysSinceLastServerReset = new TimeSpan(timeSinceLastServerReset.Days, 0, 0, 0);
                        timeSinceLastServerReset = timeSinceLastServerReset.Subtract(daysSinceLastServerReset);

                        var cooldownResetDate = currentTime - timeSinceLastServerReset;
                        return cooldownResetDate;
                    }
                    else
                    {
                        Logger.Info(
                            string.Format(
                                "could not get server uptime, timerID: {0}, group: {1}, server: {2}, player: {3}",
                                Name,
                                TimerDefinition,
                                playerTimersGroup.CurrentServerOnTheGroup,
                                Player));
                        return DateTime.MinValue;
                    }
                }
                else
                {
                    Logger.Info(
                        string.Format(
                            "could not establish server for this server group, timerID: {0}, group: {1}, player: {2}",
                            Name,
                            TimerDefinition,
                            Player));
                    return DateTime.MinValue;
                }
            }
            catch (Exception exception)
            {
                Logger.Info(exception, string.Format("could not get server uptime, timerID: {0}, group: {1}, server: {2}, player: {3}",
                        Name, TimerDefinition, playerTimersGroup.CurrentServerOnTheGroup, Player));
                return DateTime.MinValue;
            }
        }

        public override string ToString()
        {
            return string.Format("Player: {0}, Name: {1}", Player, Name);
        }
    }
}
