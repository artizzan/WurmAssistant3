using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager
{
    class PlayerManager
    {
        readonly GrangerFeature parentModule;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        readonly CreatureUpdatesManager creatureUpdateManager;

        readonly IWurmCharacter character;

        readonly Dictionary<ServerGroup, float> serverGroupToAhSkillMap = new Dictionary<ServerGroup, float>();

        public PlayerManager(
            [NotNull] GrangerFeature parentModule,
            [NotNull] GrangerContext context,
            [NotNull] string playerName,
            [NotNull] IWurmApi wurmApi, 
            [NotNull] ILogger logger, 
            [NotNull] ITrayPopups trayPopups,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            if (parentModule == null) throw new ArgumentNullException(nameof(parentModule));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (playerName == null) throw new ArgumentNullException(nameof(playerName));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
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

        public string PlayerName { get; }

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
                logger.Error(exception, "Error at updating Animal Husbandry skill for " + PlayerName);
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
