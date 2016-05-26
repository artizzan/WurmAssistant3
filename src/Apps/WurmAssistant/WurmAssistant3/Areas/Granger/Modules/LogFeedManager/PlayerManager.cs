using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Modules.LogFeedManager
{
    class PlayerManager
    {
        readonly GrangerFeature parentModule;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        readonly CreatureUpdatesManager creatureUpdateManager;

        readonly IWurmCharacter character;

        readonly Dictionary<ServerGroup, float> serverGroupToAhSkillMap = new Dictionary<ServerGroup, float>();

        public PlayerManager(GrangerFeature parentModule, GrangerContext context, string playerName,
            [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger, [NotNull] ITrayPopups trayPopups,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            this.parentModule = parentModule;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.PlayerName = playerName;

            creatureUpdateManager = new CreatureUpdatesManager(this.parentModule, context, this, trayPopups, logger, wurmAssistantConfig);

            wurmApi.LogsMonitor.Subscribe(PlayerName, LogType.Event, OnNewEventLogEvents);

            character = wurmApi.Characters.Get(PlayerName);
            character.LogInOrCurrentServerPotentiallyChanged += CharacterOnLogInOrCurrentServerPotentiallyChanged;
            character.Skills.SkillsChanged += SkillsOnSkillsChanged;

            BeginUpdateSkillInfo();
        }

        public string PlayerName { get; private set; }

        [CanBeNull]
        public IWurmServer CurrentServer { get; private set; }

        [CanBeNull]
        public float? CurrentServerAhSkill
        {
            get
            {
                var server = CurrentServer;
                if (server == null) return null;
                float skill;
                if (serverGroupToAhSkillMap.TryGetValue(server.ServerGroup, out skill))
                {
                    return skill;
                }
                else return null;
            }
        }

        private void CharacterOnLogInOrCurrentServerPotentiallyChanged(object sender, PotentialServerChangeEventArgs potentialServerChangeEventArgs)
        {
            BeginUpdateSkillInfo();
        }

        async void BeginUpdateSkillInfo()
        {
            try
            {
                await UpdateSkillInfoAsync();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Something went wrong while updating AH skill for " + PlayerName);
            }
        }

        private async Task UpdateSkillInfoAsync()
        {
            var server = await character.TryGetCurrentServerAsync();
            if (server != null)
            {
                var skill = await character.Skills.TryGetCurrentSkillLevelAsync("Animal husbandry", server.ServerGroup,
                    TimeSpan.FromDays(90));
                if (skill != null)
                {
                    serverGroupToAhSkillMap[server.ServerGroup] = skill.Value;
                }
            }
            this.CurrentServer = server;
        }

        public void Update()
        {
            creatureUpdateManager.Update();
        }

        private void OnNewEventLogEvents(object sender, LogsMonitorEventArgs e)
        {
            foreach (var wurmLogEntry in e.WurmLogEntries)
            {
                if (parentModule.Settings.LogCaptureEnabled)
                {
                    creatureUpdateManager.ProcessEventForCreatureUpdates(wurmLogEntry);
                }
            }
        }

        void SkillsOnSkillsChanged(object sender, SkillsChangedEventArgs skillsChangedEventArgs)
        {
            foreach (var skillInfo in skillsChangedEventArgs.SkillChanges)
            {
                if (skillInfo.IsSkillName("Animal husbandry") && skillInfo.Server != null)
                {
                    serverGroupToAhSkillMap[skillInfo.Server.ServerGroup] = skillInfo.Value;
                }
            }
        }

        public void Dispose()
        {
            character.LogInOrCurrentServerPotentiallyChanged -= CharacterOnLogInOrCurrentServerPotentiallyChanged;
            character.Skills.SkillsChanged -= SkillsOnSkillsChanged;
            wurmApi.LogsMonitor.Unsubscribe(PlayerName, OnNewEventLogEvents);
        }
    }
}
