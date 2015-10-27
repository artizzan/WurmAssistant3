using System;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.LogFeedManager
{
    class PlayerManager
    {
        private readonly GrangerFeature _parentModule;
        private GrangerContext _context;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        //ManualServerGroupManager SGManager;
        HorseUpdatesManager HorseUpdateManager;

        IWurmCharacter character;

        public string PlayerName { get; private set; }

        private float _ahFreedomSkill;
        public float AhFreedomSkill
        {
            get { return _ahFreedomSkill; }
            set
            {
                _ahFreedomSkill = value;
                _parentModule.Settings.SetAHSkill(PlayerName, ServerGroupId.Freedom, value);
            }
        }

        private float _ahEpicSkill;
        public float AhEpicSkill
        {
            get { return _ahEpicSkill; }
            set
            {
                _ahEpicSkill = value;
                _parentModule.Settings.SetAHSkill(PlayerName, ServerGroupId.Epic, value);
            }
        }

        bool _skillObtainedFlag = false;

        public PlayerManager(GrangerFeature parentModule, GrangerContext context, string playerName,
            [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger, [NotNull] ITrayPopups trayPopups)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            this._parentModule = parentModule;
            this._context = context;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.PlayerName = playerName;

            //SGManager = new ManualServerGroupManager(PlayerName);
            HorseUpdateManager = new HorseUpdatesManager(_parentModule, _context, this, trayPopups, logger);

            wurmApi.LogsMonitor.Subscribe(PlayerName, LogType.AllLogs, OnNewLogEvents);

            character = wurmApi.Characters.Get(PlayerName);
            character.Skills.SkillsChanged += SkillsOnSkillsChanged;

            BeginInitSkill();
        }

        public void Update()
        {
            HorseUpdateManager.Update();
        }

        async void BeginInitSkill()
        {
            try
            {
                UpdateSkill();

                _skillObtainedFlag = true;

                var eh = SkillObtained;
                if (eh != null) eh(this, new LogFeedManager.SkillObtainedEventArgs(PlayerName));
            }
            catch (Exception _e)
            {
                logger.Error(_e, "Something went wrong while trying to get AH skill for " + PlayerName);
            }
        }

        /// <summary>
        /// returns null if no skill data available yet
        /// </summary>
        /// <param name="serverGroup"></param>
        /// <returns></returns>
        public float? GetAhSkill(ServerGroupId serverGroup)
        {
            if (!_skillObtainedFlag) return null;

            if (serverGroup == ServerGroupId.Epic)
                return AhEpicSkill;
            else if (serverGroup == ServerGroupId.Freedom)
                return AhFreedomSkill;
            else
            {
                logger.Debug("unknown server group for ah skill request, returning null, player: " + PlayerName);
                return null;
            }
        }

        public ServerGroupId GetCurrentServerGroup()
        {
            //todo: this is a blocking call, refactor?
            var currentServer = character.TryGetCurrentServer();
            if (currentServer == null) return ServerGroupId.Unknown;
            return currentServer.ServerGroup.ServerGroupId;
        }

        public event EventHandler<LogFeedManager.SkillObtainedEventArgs> SkillObtained;

        private void OnNewLogEvents(object sender, LogsMonitorEventArgs e)
        {
            if (e.LogType == LogType.Event)
            {
                foreach (var wurmLogEntry in e.WurmLogEntries)
                {
                    if (_parentModule.Settings.LogCaptureEnabled)
                    {
                        HorseUpdateManager.ProcessEventForHorseUpdates(wurmLogEntry);
                    }
                }
            }
        }

        void SkillsOnSkillsChanged(object sender, SkillsChangedEventArgs skillsChangedEventArgs)
        {
            if (skillsChangedEventArgs.HasSkillChanged("Animal husbandry"))
            {
                UpdateSkill();
            }
        }

        private void UpdateSkill()
        {
            //todo: blocking calls...
            AhFreedomSkill = character.Skills.TryGetCurrentSkillLevel(
                "Animal husbandry",
                ServerGroupId.Freedom,
                TimeSpan.FromDays(90)) ?? 0;
            AhEpicSkill = character.Skills.TryGetCurrentSkillLevel(
                "Animal husbandry",
                ServerGroupId.Epic,
                TimeSpan.FromDays(90)) ?? 0;
        }

        public void Dispose()
        {
            character.Skills.SkillsChanged -= SkillsOnSkillsChanged;
            wurmApi.LogsMonitor.Unsubscribe(PlayerName, OnNewLogEvents);
        }

        /// <summary>
        /// returns null if ah skill not yet found or server group not established
        /// </summary>
        /// <returns></returns>
        internal float? GetAhSkill()
        {
            return GetAhSkill(GetCurrentServerGroup());
        }
    }
}
