using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Modules.Events.Public;
using AldursLab.WurmApi.Modules.Wurm.Characters.Logs;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters
{
    class WurmCharacter : IWurmCharacter, IDisposable, IHandle<YouAreOnEventDetectedOnLiveLogs>
    {
        readonly IWurmConfigs wurmConfigs;
        readonly IWurmServers wurmServers;
        readonly IWurmServerHistory wurmServerHistory;
        readonly IWurmApiLogger logger;
        readonly TaskManager taskManager;
        readonly IWurmLogsMonitorInternal logsMonitor;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly InternalEventAggregator internalEventAggregator;
        readonly IWurmLogsHistory logsHistory;
        readonly IWurmServerGroups wurmServerGroups;
        readonly WurmCharacterSkills skills;

        readonly FileSystemWatcher configFileWatcher;
        readonly string configDefiningFileFullPath;
        string currentConfigName = string.Empty;

        const string ConfigDefinerFileName = "config.txt";

        readonly TaskHandle configUpdateTask;

        public WurmCharacter([NotNull] CharacterName name, [NotNull] string playerDirectoryFullPath,
            [NotNull] IWurmConfigs wurmConfigs, [NotNull] IWurmServers wurmServers,
            [NotNull] IWurmServerHistory wurmServerHistory,
            [NotNull] IWurmApiLogger logger, 
            [NotNull] TaskManager taskManager, [NotNull] IWurmLogsMonitorInternal logsMonitor,
            [NotNull] IPublicEventInvoker publicEventInvoker, [NotNull] InternalEventAggregator internalEventAggregator,
            [NotNull] IWurmLogsHistory logsHistory, [NotNull] IWurmPaths wurmPaths,
            [NotNull] IWurmServerGroups wurmServerGroups)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (playerDirectoryFullPath == null) throw new ArgumentNullException(nameof(playerDirectoryFullPath));
            if (wurmConfigs == null) throw new ArgumentNullException(nameof(wurmConfigs));
            if (wurmServers == null) throw new ArgumentNullException(nameof(wurmServers));
            if (wurmServerHistory == null) throw new ArgumentNullException(nameof(wurmServerHistory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (taskManager == null) throw new ArgumentNullException(nameof(taskManager));
            if (logsMonitor == null) throw new ArgumentNullException(nameof(logsMonitor));
            if (publicEventInvoker == null) throw new ArgumentNullException(nameof(publicEventInvoker));
            if (internalEventAggregator == null) throw new ArgumentNullException(nameof(internalEventAggregator));
            if (logsHistory == null) throw new ArgumentNullException(nameof(logsHistory));
            if (wurmPaths == null) throw new ArgumentNullException(nameof(wurmPaths));
            if (wurmServerGroups == null) throw new ArgumentNullException(nameof(wurmServerGroups));

            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.logger = logger;
            this.taskManager = taskManager;
            this.logsMonitor = logsMonitor;
            this.publicEventInvoker = publicEventInvoker;
            this.internalEventAggregator = internalEventAggregator;
            this.logsHistory = logsHistory;
            this.wurmServerGroups = wurmServerGroups;

            internalEventAggregator.Subscribe(this);

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

            try
            {
                wurmServerHistory.BeginTracking(Name);
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error,
                    string.Format("Failed to initiate tracking of server history for character {0}", name),
                    this,
                    exception);
            }

            skills = new WurmCharacterSkills(this,
                publicEventInvoker,
                logsMonitor,
                logsHistory,
                logger,
                wurmPaths,
                internalEventAggregator);
            Logs = new WurmCharacterLogs(this, wurmServerGroups, logsHistory, wurmServers, logger);
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

        public IWurmCharacterSkills Skills => skills;

        public IWurmCharacterLogs Logs { get; private set; }

        #region GetHistoricServerAtLogStamp

        public async Task<IWurmServer> TryGetHistoricServerAtLogStampAsync(DateTime stamp)
        {
            return await TryGetHistoricServerAtLogStampAsync(stamp, CancellationToken.None).ConfigureAwait(false);
        }

        public IWurmServer TryGetHistoricServerAtLogStamp(DateTime stamp)
        {
            return TryGetHistoricServerAtLogStamp(stamp, CancellationToken.None);
        }

        public async Task<IWurmServer> TryGetHistoricServerAtLogStampAsync(DateTime stamp, CancellationToken cancellationToken)
        {
            var serverName = await wurmServerHistory.TryGetServerAsync(Name, stamp, cancellationToken).ConfigureAwait(false);
            if (serverName == null) return null;
            var server = wurmServers.GetByName(serverName);
            return server;
        }

        public IWurmServer TryGetHistoricServerAtLogStamp(DateTime stamp, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetHistoricServerAtLogStampAsync(stamp, cancellationToken).Result);
        }

        #endregion

        #region GetCurrentServer

        public async Task<IWurmServer> TryGetCurrentServerAsync()
        {
            return await TryGetCurrentServerAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public IWurmServer TryGetCurrentServer()
        {
            return TryGetCurrentServer(CancellationToken.None);
        }

        public async Task<IWurmServer> TryGetCurrentServerAsync(CancellationToken cancellationToken)
        {
            var serverName = await wurmServerHistory.TryGetCurrentServerAsync(Name, cancellationToken).ConfigureAwait(false);
            if (serverName == null) return null;
            var server = wurmServers.GetByName(serverName);
            return server;
        }

        public IWurmServer TryGetCurrentServer(CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetCurrentServerAsync(cancellationToken).Result);
        }

        public event EventHandler<PotentialServerChangeEventArgs> LogInOrCurrentServerPotentiallyChanged;

        #endregion

        public void Dispose()
        {
            skills.Dispose();
            configFileWatcher.EnableRaisingEvents = false;
            configFileWatcher.Dispose();
        }

        public void Handle(YouAreOnEventDetectedOnLiveLogs message)
        {
            if (message.CharacterName == Name)
            {
                publicEventInvoker.TriggerInstantly(LogInOrCurrentServerPotentiallyChanged,
                    this,
                    new PotentialServerChangeEventArgs(message.ServerName, message.CurrentServerNameChanged));
            }
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }

    public class PotentialServerChangeEventArgs : EventArgs
    {
        public PotentialServerChangeEventArgs(ServerName serverName, bool serverChanged)
        {
            ServerName = serverName;
            ServerChanged = serverChanged;
        }

        /// <summary>
        /// Parsed server name
        /// </summary>
        public ServerName ServerName { get; private set; }

        /// <summary>
        /// Indicates, if detected server is different from last detected server.
        /// Note, that this may be a false positive. This would happen, when WurmApi hadn't known previous server for this character. 
        /// To use this property reliably, first do a GetCurrentServer() and assuming it has returned a server (and thus WurmApi knows it now), 
        /// this property will give accurate information on successive invocations.
        /// </summary>
        public bool ServerChanged { get; private set; }
    }
}