using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager
{
    public class LogsFeedManager : IDisposable
    {
        readonly GrangerContext context;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly ITrayPopups trayPopups;
        readonly IWurmAssistantConfig wurmAssistantConfig;
        [NotNull] readonly CreatureColorDefinitions creatureColorDefinitions;
        readonly GrangerFeature parentModule;
        readonly Dictionary<string, PlayerManager> playerManagers = new Dictionary<string, PlayerManager>();
        readonly GrangerSettings grangerSettings;

        public LogsFeedManager(
            [NotNull] GrangerFeature parentModule,
            [NotNull] GrangerContext context, 
            [NotNull] IWurmApi wurmApi,
            [NotNull] ILogger logger, 
            [NotNull] ITrayPopups trayPopups,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig,
            [NotNull] CreatureColorDefinitions creatureColorDefinitions,
            [NotNull] GrangerSettings grangerSettings)
        {
            if (parentModule == null) throw new ArgumentNullException(nameof(parentModule));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            if (creatureColorDefinitions == null) throw new ArgumentNullException(nameof(creatureColorDefinitions));
            if (grangerSettings == null) throw new ArgumentNullException(nameof(grangerSettings));
            this.parentModule = parentModule;
            this.context = context;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.trayPopups = trayPopups;
            this.wurmAssistantConfig = wurmAssistantConfig;
            this.creatureColorDefinitions = creatureColorDefinitions;
            this.grangerSettings = grangerSettings;
        }

        public void TryRegisterPlayer(string playerName)
        {
            if (!playerManagers.ContainsKey(playerName))
            {
                try
                {
                    playerManagers[playerName] = new PlayerManager(parentModule,
                        context,
                        playerName,
                        wurmApi,
                        logger,
                        trayPopups,
                        wurmAssistantConfig, 
                        creatureColorDefinitions,
                        grangerSettings);
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Count not register PlayerManager for player name: " + playerName);
                }
            }
        }


        public void UnregisterPlayer(string playerName)
        {
            PlayerManager ph;
            if (playerManagers.TryGetValue(playerName, out ph))
            {
                ph.Dispose();
                playerManagers.Remove(playerName);
            }
        }

        public void Dispose()
        {
            foreach (var keyval in playerManagers)
            {
                keyval.Value.Dispose();
            }
        }

        internal void UpdatePlayers(IEnumerable<string> players)
        {
            var list = players.ToList();

            foreach (var player in list)
            {
                if (!playerManagers.ContainsKey(player))
                {
                    TryRegisterPlayer(player);
                }
            }

            var managedPlayers = playerManagers.Keys.ToArray();
            foreach (var player in managedPlayers)
            {
                if (!list.Contains(player))
                    UnregisterPlayer(player);
            }
        }

        internal void Update()
        {
            foreach (var keyval in playerManagers)
            {
                keyval.Value.Update();
            }
        }
    }
}
