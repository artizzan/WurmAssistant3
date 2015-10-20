using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Profiling.Modules;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers
{
    public abstract class WurmTimer : PersistentObjectBase
    {
        protected readonly ITrayPopups TrayPopups;
        protected readonly ILogger Logger;
        protected readonly IWurmApi WurmApi;
        protected readonly ISoundEngine SoundEngine;

        protected TimerDisplayView TimerDisplayView;
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
            [NotNull] IWurmApi wurmApi, [NotNull] ISoundEngine soundEngine) : base(persistentObjectId)
        {
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            this.TrayPopups = trayPopups;
            this.Logger = logger;
            this.WurmApi = wurmApi;
            this.SoundEngine = soundEngine;
        }

        public virtual void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            //derived must call this base before their own inits!
            Name = definition.ToString();
            ShortName = definition.ToCompactString();
            playerTimersGroup = parentGroup;
            Player = player;
            TimerDefinitionId = definition.TimerDefinitionId;

            TimerDisplayView = new TimerDisplayView(this);
            this.playerTimersGroup.RegisterNewControlTimer(TimerDisplayView);

            CDNotify = new CooldownHandler(Logger, SoundEngine, TrayPopups)
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

        public TimerDefinitionId TimerDefinitionId { get; private set; }

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

        public virtual void Stop()
        {
            //children should basecall this after their own cleanup
            playerTimersGroup.UnregisterControlTimer(TimerDisplayView);
            TimerDisplayView.Dispose();
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
            float skillLevel = await character.Skills.TryGetCurrentSkillLevelAsync(skillName, TimerDefinitionId.ServerGroupId, since)
                               ?? 0;

            return skillLevel;
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

            float skillLevel =
                await
                    character.Skills.TryGetCurrentSkillLevelAsync(skillName,
                        TimerDefinitionId.ServerGroupId,
                        HowLongAgoWasThisDate(lastCheckup)) ?? 0;

            return skillLevel;
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
                TimerDefinitionId.ServerGroupId);

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
            TimerDefaultSettingsForm ui = new TimerDefaultSettingsForm(this, SoundEngine);
            ui.ShowDialogCenteredOnForm(this.GetModuleUi());
        }

        public TimersFeature TimersFeature { get { return playerTimersGroup.TimersFeature; } }

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

                var server = WurmApi.Servers.GetByName(playerTimersGroup.GroupToServerMap[TimerDefinitionId.ServerGroupId]);

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
                    Logger.Info(string.Format("could not get server uptime, timerID: {0}, group: {1}, server: {2}, player: {3}", 
                        Name, TimerDefinitionId, playerTimersGroup.CurrentServerName, Player));
                    return DateTime.MinValue;
                }
            }
            catch (Exception exception)
            {
                Logger.Info(exception, string.Format("could not get server uptime, timerID: {0}, group: {1}, server: {2}, player: {3}",
                        Name, TimerDefinitionId, playerTimersGroup.CurrentServerName, Player));
                return DateTime.MinValue;
            }
        }

        public override string ToString()
        {
            return string.Format("Player: {0}, Name: {1}", Player, Name);
        }
    }
}
