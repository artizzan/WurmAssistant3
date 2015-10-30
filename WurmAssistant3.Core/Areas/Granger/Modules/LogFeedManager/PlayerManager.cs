using System;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.LogFeedManager
{
    class PlayerManager
    {
        private readonly GrangerFeature parentModule;
        private readonly GrangerContext context;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        readonly HorseUpdatesManager horseUpdateManager;

        readonly IWurmCharacter character;

        public string PlayerName { get; private set; }

        private float ahFreedomSkill;
        public float AhFreedomSkill
        {
            get { return ahFreedomSkill; }
            set
            {
                ahFreedomSkill = value;
                parentModule.Settings.SetAhSkill(PlayerName, new ServerGroup(ServerGroup.FreedomId), value);
            }
        }

        private float ahEpicSkill;
        public float AhEpicSkill
        {
            get { return ahEpicSkill; }
            set
            {
                ahEpicSkill = value;
                parentModule.Settings.SetAhSkill(PlayerName, new ServerGroup(ServerGroup.EpicId), value);
            }
        }

        bool _skillObtainedFlag = false;

        public PlayerManager(GrangerFeature parentModule, GrangerContext context, string playerName,
            [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger, [NotNull] ITrayPopups trayPopups)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            this.parentModule = parentModule;
            this.context = context;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.PlayerName = playerName;

            //SGManager = new ManualServerGroupManager(PlayerName);
            horseUpdateManager = new HorseUpdatesManager(this.parentModule, this.context, this, trayPopups, logger);

            wurmApi.LogsMonitor.Subscribe(PlayerName, LogType.AllLogs, OnNewLogEvents);

            character = wurmApi.Characters.Get(PlayerName);
            character.Skills.SkillsChanged += SkillsOnSkillsChanged;

            BeginInitSkill();
        }

        public void Update()
        {
            horseUpdateManager.Update();
        }

        async void BeginInitSkill()
        {
            try
            {
                UpdateSkill();

                _skillObtainedFlag = true;

                var eh = SkillObtained;
                if (eh != null) eh(this, new LogsFeedManager.SkillObtainedEventArgs(PlayerName));
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
        public float? GetAhSkill(ServerGroup serverGroup)
        {
            if (!_skillObtainedFlag) return null;

            if (serverGroup.ServerGroupId == ServerGroup.EpicId)
                return AhEpicSkill;
            else if (serverGroup.ServerGroupId == ServerGroup.FreedomId)
                return AhFreedomSkill;
            else
            {
                logger.Debug("unknown server group for ah skill request, returning null, player: " + PlayerName);
                return null;
            }
        }

        public ServerGroup GetCurrentServerGroup()
        {
            //todo: this is a blocking call, refactor?
            var currentServer = character.TryGetCurrentServer();
            if (currentServer == null) return new ServerGroup(ServerGroup.UnknownId);
            return currentServer.ServerGroup;
        }

        public event EventHandler<LogsFeedManager.SkillObtainedEventArgs> SkillObtained;

        private void OnNewLogEvents(object sender, LogsMonitorEventArgs e)
        {
            if (e.LogType == LogType.Event)
            {
                foreach (var wurmLogEntry in e.WurmLogEntries)
                {
                    if (parentModule.Settings.LogCaptureEnabled)
                    {
                        horseUpdateManager.ProcessEventForHorseUpdates(wurmLogEntry);
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
            //todo: blocking calls... refactor
            var freedomskill = character.Skills.TryGetCurrentSkillLevel(
                "Animal husbandry",
                new ServerGroup(ServerGroup.FreedomId),
                TimeSpan.FromDays(90));
            AhFreedomSkill = freedomskill != null ? freedomskill.Value : 0;

            var epicskill = character.Skills.TryGetCurrentSkillLevel(
                "Animal husbandry",
                new ServerGroup(ServerGroup.EpicId),
                TimeSpan.FromDays(90));
            AhFreedomSkill = epicskill != null ? epicskill.Value : 0;
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
