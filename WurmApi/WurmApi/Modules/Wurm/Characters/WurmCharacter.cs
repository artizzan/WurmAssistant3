using System;
using System.IO;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Characters
{
    public class WurmCharacter : IWurmCharacter, IDisposable
    {
        readonly IWurmConfigs wurmConfigs;
        readonly IWurmServers wurmServers;
        readonly IWurmServerHistory wurmServerHistory;
        readonly ILogger logger;

        readonly FileSystemWatcher configFileWatcher;
        readonly string configDefiningFileFullPath;
        string currentConfigName = string.Empty;

        private const string ConfigDefinerFileName = "config.txt";

        public WurmCharacter([NotNull] CharacterName name, [NotNull] string playerDirectoryFullPath,
            [NotNull] IWurmConfigs wurmConfigs, [NotNull] IWurmServers wurmServers,
            [NotNull] IWurmServerHistory wurmServerHistory,
            [NotNull] ILogger logger)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (playerDirectoryFullPath == null) throw new ArgumentNullException("playerDirectoryFullPath");
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmServers == null) throw new ArgumentNullException("wurmServers");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logger == null) throw new ArgumentNullException("logger");

            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.logger = logger;

            Name = name;
            configDefiningFileFullPath = Path.Combine(playerDirectoryFullPath, ConfigDefinerFileName);

            configFileWatcher = new FileSystemWatcher()
            {
                Filter = ConfigDefinerFileName
            };
            configFileWatcher.Changed += ConfigFileWatcherOnChanged;
            configFileWatcher.Created += ConfigFileWatcherOnChanged;
            configFileWatcher.Deleted += ConfigFileWatcherOnChanged;
            configFileWatcher.Renamed += ConfigFileWatcherOnChanged;
            configFileWatcher.EnableRaisingEvents = true;
            RefreshCurrentConfig();
        }

        void ConfigFileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            RefreshCurrentConfig();
        }

        private void RefreshCurrentConfig()
        {
            if (!File.Exists(configDefiningFileFullPath))
            {
                logger.Log(LogLevel.Error,
                    string.Format("{0} is missing for the player {1}, cannot obtain config!", ConfigDefinerFileName,
                        Name), this, null);
            }
            else
            {
                currentConfigName = File.ReadAllText(configDefiningFileFullPath).Trim();
                if (string.IsNullOrWhiteSpace(currentConfigName))
                {
                    logger.Log(LogLevel.Error,
                        string.Format("Could not read config for player {0}, because {1} contains no config name!",
                            Name, ConfigDefinerFileName), this, null);
                }
                else
                {
                    try
                    {
                        CurrentConfig = wurmConfigs.GetConfig(currentConfigName);
                    }
                    catch (Exception exception)
                    {
                        logger.Log(LogLevel.Error,
                            string.Format("Could not read config named {0} for player {1}", currentConfigName, Name), this,
                            exception);
                    }
                }
            }
        }

        public virtual CharacterName Name { get; private set; }

        public IWurmConfig CurrentConfig { get; private set; }

        public async Task<IWurmServer> GetHistoricServerAtLogStamp(DateTime stamp)
        {
            var serverName = await wurmServerHistory.GetServer(this.Name, stamp).ConfigureAwait(false);
            var server = wurmServers.GetByName(serverName);
            return server;
        }

        public async Task<IWurmServer> GetCurrentServer()
        {
            var serverName = await wurmServerHistory.GetCurrentServer(this.Name).ConfigureAwait(false);
            var server = wurmServers.GetByName(serverName);
            return server;
        }

        public void Dispose()
        {
            configFileWatcher.EnableRaisingEvents = false;
            configFileWatcher.Dispose();
        }
    }
}