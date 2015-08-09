using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Characters
{
    class WurmCharacter : IWurmCharacter, IDisposable
    {
        readonly IWurmConfigs wurmConfigs;
        readonly IWurmServers wurmServers;
        readonly IWurmServerHistory wurmServerHistory;
        readonly ILogger logger;
        readonly TaskManager taskManager;

        readonly FileSystemWatcher configFileWatcher;
        readonly string configDefiningFileFullPath;
        string currentConfigName = string.Empty;

        private const string ConfigDefinerFileName = "config.txt";

        readonly TaskHandle configUpdateTask;

        public WurmCharacter([NotNull] CharacterName name, [NotNull] string playerDirectoryFullPath,
            [NotNull] IWurmConfigs wurmConfigs, [NotNull] IWurmServers wurmServers,
            [NotNull] IWurmServerHistory wurmServerHistory,
            [NotNull] ILogger logger, 
            [NotNull] TaskManager taskManager)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (playerDirectoryFullPath == null) throw new ArgumentNullException("playerDirectoryFullPath");
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmServers == null) throw new ArgumentNullException("wurmServers");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logger == null) throw new ArgumentNullException("logger");
            if (taskManager == null) throw new ArgumentNullException("taskManager");

            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.logger = logger;
            this.taskManager = taskManager;

            Name = name;
            configDefiningFileFullPath = Path.Combine(playerDirectoryFullPath, ConfigDefinerFileName);

            RefreshCurrentConfig();

            configUpdateTask = new TaskHandle(RefreshCurrentConfig, "Current config update for player " + Name);
            taskManager.Add(configUpdateTask);

            configFileWatcher = new FileSystemWatcher(playerDirectoryFullPath)
            {
                Filter = ConfigDefinerFileName
            };
            configFileWatcher.Changed += ConfigFileWatcherOnChanged;
            configFileWatcher.Created += ConfigFileWatcherOnChanged;
            configFileWatcher.Deleted += ConfigFileWatcherOnChanged;
            configFileWatcher.Renamed += ConfigFileWatcherOnChanged;
            configFileWatcher.EnableRaisingEvents = true;
            
            configUpdateTask.Trigger();
        }

        void ConfigFileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            configUpdateTask.Trigger();
        }

        private void RefreshCurrentConfig()
        {
            if (!File.Exists(configDefiningFileFullPath))
            {
                throw new WurmApiException(
                    string.Format("{0} is missing for character {1}, cannot obtain config!",
                        ConfigDefinerFileName,
                        Name));
            }

            currentConfigName = File.ReadAllText(configDefiningFileFullPath).Trim();
            if (string.IsNullOrWhiteSpace(currentConfigName))
            {
                throw new WurmApiException(
                    string.Format("Could not read config for character {0}, because {1} contains no config name!",
                        Name,
                        ConfigDefinerFileName));
            }

            try
            {
                CurrentConfig = wurmConfigs.GetConfig(currentConfigName);
            }
            catch (Exception exception)
            {
                throw new WurmApiException(
                    string.Format("Could not read config {0} for player {1}. See inner exception for details.",
                        currentConfigName,
                        Name),
                    exception);
            }
        }

        public CharacterName Name { get; private set; }

        public IWurmConfig CurrentConfig { get; private set; }

        #region GetHistoricServerAtLogStamp

        public async Task<IWurmServer> GetHistoricServerAtLogStampAsync(DateTime stamp)
        {
            return await GetHistoricServerAtLogStampAsync(stamp, CancellationToken.None).ConfigureAwait(false);
        }

        public IWurmServer GetHistoricServerAtLogStamp(DateTime stamp)
        {
            return GetHistoricServerAtLogStamp(stamp, CancellationToken.None);
        }

        public async Task<IWurmServer> GetHistoricServerAtLogStampAsync(DateTime stamp, CancellationToken cancellationToken)
        {
            var serverName = await wurmServerHistory.GetServerAsync(this.Name, stamp, cancellationToken).ConfigureAwait(false);
            var server = wurmServers.GetByName(serverName);
            return server;
        }

        public IWurmServer GetHistoricServerAtLogStamp(DateTime stamp, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetHistoricServerAtLogStampAsync(stamp, cancellationToken).Result);
        }

        #endregion

        #region GetCurrentServer

        public async Task<IWurmServer> GetCurrentServerAsync()
        {
            return await GetCurrentServerAsync(CancellationToken.None);
        }

        public IWurmServer GetCurrentServer()
        {
            return GetCurrentServer(CancellationToken.None);
        }

        public async Task<IWurmServer> GetCurrentServerAsync(CancellationToken cancellationToken)
        {
            var serverName = await wurmServerHistory.GetCurrentServerAsync(this.Name, cancellationToken).ConfigureAwait(false);
            var server = wurmServers.GetByName(serverName);
            return server;
        }

        public IWurmServer GetCurrentServer(CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetCurrentServerAsync(cancellationToken).Result);
        }

        #endregion

        public void Dispose()
        {
            configFileWatcher.EnableRaisingEvents = false;
            configFileWatcher.Dispose();
        }
    }
}