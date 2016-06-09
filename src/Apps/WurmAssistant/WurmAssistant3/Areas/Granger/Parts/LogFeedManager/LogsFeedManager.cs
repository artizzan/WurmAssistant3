using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.Singletons;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Parts.LogFeedManager
{
    public class LogsFeedManager : IDisposable
    {
        readonly GrangerContext context;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly ITrayPopups trayPopups;
        readonly IWurmAssistantConfig wurmAssistantConfig;
        readonly GrangerFeature parentModule;
        readonly Dictionary<string, PlayerManager> playerManagers = new Dictionary<string, PlayerManager>();

        public LogsFeedManager(GrangerFeature parentModule, GrangerContext context, [NotNull] IWurmApi wurmApi,
            [NotNull] ILogger logger, [NotNull] ITrayPopups trayPopups,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            this.parentModule = parentModule;
            this.context = context;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.trayPopups = trayPopups;
            this.wurmAssistantConfig = wurmAssistantConfig;
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
                        wurmAssistantConfig);
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
