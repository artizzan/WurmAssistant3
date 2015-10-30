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

        readonly CreatureUpdatesManager creatureUpdateManager;

        readonly IWurmCharacter character;

        public string PlayerName { get; private set; }

        public float AhFreedomSkill { get; private set; }
        public float AhEpicSkill { get; private set; }

        bool skillObtainedFlag = false;

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

            creatureUpdateManager = new CreatureUpdatesManager(this.parentModule, this.context, this, trayPopups, logger);

            wurmApi.LogsMonitor.Subscribe(PlayerName, LogType.AllLogs, OnNewLogEvents);

            character = wurmApi.Characters.Get(PlayerName);
            character.Skills.SkillsChanged += SkillsOnSkillsChanged;

            BeginInitSkill();
        }

        async void BeginInitSkill()
        {
            try
            {
                var freedomskill = await character.Skills.TryGetCurrentSkillLevelAsync(
                    "Animal husbandry",
                    new ServerGroup(ServerGroup.FreedomId),
                    TimeSpan.FromDays(90));
                AhFreedomSkill = freedomskill != null ? freedomskill.Value : 0;

                var epicskill = await character.Skills.TryGetCurrentSkillLevelAsync(
                    "Animal husbandry",
                    new ServerGroup(ServerGroup.EpicId),
                    TimeSpan.FromDays(90));
                AhEpicSkill = epicskill != null ? epicskill.Value : 0;

                skillObtainedFlag = true;
            }
            catch (Exception _e)
            {
                logger.Error(_e, "Something went wrong while trying to get AH skill for " + PlayerName);
            }
        }

        public void Update()
        {
            creatureUpdateManager.Update();
        }

        /// <summary>
        /// returns null if no skill data available yet
        /// </summary>
        /// <param name="serverGroup"></param>
        /// <returns></returns>
        public float? GetAhSkill(ServerGroup serverGroup)
        {
            if (!skillObtainedFlag) return null;

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

        private void OnNewLogEvents(object sender, LogsMonitorEventArgs e)
        {
            if (e.LogType == LogType.Event)
            {
                foreach (var wurmLogEntry in e.WurmLogEntries)
                {
                    if (parentModule.Settings.LogCaptureEnabled)
                    {
                        creatureUpdateManager.ProcessEventForCreatureUpdates(wurmLogEntry);
                    }
                }
            }
        }

        void SkillsOnSkillsChanged(object sender, SkillsChangedEventArgs skillsChangedEventArgs)
        {
            foreach (var skillInfo in skillsChangedEventArgs.SkillChanges)
            {
                if (skillInfo.IsSkillName("Animal husbandry") && skillInfo.Server != null)
                {
                    if (skillInfo.Server.ServerGroup.ServerGroupId == ServerGroup.FreedomId)
                    {
                        AhFreedomSkill = skillInfo.Value;
                    }
                    else if (skillInfo.Server.ServerGroup.ServerGroupId == ServerGroup.FreedomId)
                    {
                        AhEpicSkill = skillInfo.Value;
                    }
                }
            }
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
