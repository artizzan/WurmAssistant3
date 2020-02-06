using System;
using System.Collections.Generic;
using System.IO;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Public;
using AldursLab.WurmApi.Modules.Networking;
using AldursLab.WurmApi.Modules.Wurm.Autoruns;
using AldursLab.WurmApi.Modules.Wurm.CharacterDirectories;
using AldursLab.WurmApi.Modules.Wurm.Characters;
using AldursLab.WurmApi.Modules.Wurm.ConfigDirectories;
using AldursLab.WurmApi.Modules.Wurm.Configs;
using AldursLab.WurmApi.Modules.Wurm.LogDefinitions;
using AldursLab.WurmApi.Modules.Wurm.LogFiles;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.Modules.Wurm.Paths;
using AldursLab.WurmApi.Modules.Wurm.ServerGroups;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory;
using AldursLab.WurmApi.Modules.Wurm.Servers;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Host of all WurmApi services.
    /// </summary>
    sealed class WurmApiManager : IWurmApi, IDisposable
    {
        readonly List<IDisposable> disposables = new List<IDisposable>();

        /// <summary>
        /// Public factory constructor.
        /// </summary>
        internal WurmApiManager(
            WurmApiDataDirectory dataDirectory,
            IWurmClientInstallDirectory installDirectory,
            IWurmApiLogger wurmApiLogger,
            IWurmApiEventMarshaller publicEventMarshaller,
            WurmApiConfig wurmApiConfig)
        {
            var threadPoolMarshaller = new ThreadPoolMarshaller(wurmApiLogger);
            if (publicEventMarshaller == null)
            {
                publicEventMarshaller = threadPoolMarshaller;
            }
            IHttpWebRequests httpRequests = new HttpWebRequests();
            ConstructSystems(dataDirectory.FullPath,
                installDirectory,
                httpRequests,
                wurmApiLogger,
                publicEventMarshaller,
                threadPoolMarshaller,
                wurmApiConfig);
        }

        /// <summary>
        /// Constructor for integration testing.
        /// </summary>
        internal WurmApiManager(
            string dataDir,
            IWurmClientInstallDirectory installDirectory,
            IHttpWebRequests httpWebRequests,
            IWurmApiLogger logger,
            WurmApiConfig wurmApiConfig)
        {
            ConstructSystems(dataDir,
                installDirectory,
                httpWebRequests,
                logger,
                new SimpleMarshaller(),
                new SimpleMarshaller(),
                wurmApiConfig);
        }

        void ConstructSystems(string wurmApiDataDirectoryFullPath, IWurmClientInstallDirectory installDirectory,
            IHttpWebRequests httpWebRequests, IWurmApiLogger logger, IWurmApiEventMarshaller publicEventMarshaller,
            IWurmApiEventMarshaller internalEventMarshaller, WurmApiConfig wurmApiConfig)
        {
            IWurmApiConfig internalWurmApiConfig = wurmApiConfig.CreateCopy();

            LogFileStreamReaderFactory logFileStreamReaderFactory = Wire(new LogFileStreamReaderFactory(internalWurmApiConfig));

            Wire(installDirectory);
            Wire(httpWebRequests);

            if (logger == null)
                logger = new WurmApiLoggerStub();

            PublicEventInvoker publicEventInvoker = Wire(new PublicEventInvoker(publicEventMarshaller, logger));

            TaskManager taskManager = Wire(new TaskManager(logger));

            InternalEventAggregator internalEventAggregator = Wire(new InternalEventAggregator());

            InternalEventInvoker internalEventInvoker =
                Wire(new InternalEventInvoker(internalEventAggregator, logger, internalEventMarshaller));

            WurmPaths paths = Wire(new WurmPaths(installDirectory, wurmApiConfig));
            WurmServerGroups serverGroups = Wire(new WurmServerGroups(internalWurmApiConfig.ServerInfoMap));

            WurmServerList serverList = Wire(new WurmServerList(internalWurmApiConfig.ServerInfoMap));

            WurmLogDefinitions logDefinitions = Wire(new WurmLogDefinitions());

            WurmConfigDirectories configDirectories =
                Wire(new WurmConfigDirectories(paths, internalEventAggregator, taskManager, logger));
            WurmCharacterDirectories characterDirectories =
                Wire(new WurmCharacterDirectories(paths, internalEventAggregator, taskManager, logger));
            WurmLogFiles logFiles =
                Wire(new WurmLogFiles(characterDirectories,
                    logger,
                    logDefinitions,
                    internalEventAggregator,
                    internalEventInvoker,
                    taskManager,
                    paths));

            WurmLogsMonitor logsMonitor =
                Wire(new WurmLogsMonitor(logFiles,
                    logger,
                    publicEventInvoker,
                    internalEventAggregator,
                    characterDirectories,
                    internalEventInvoker,
                    taskManager,
                    logFileStreamReaderFactory));
            var heuristicsDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmLogsHistory");
            if (internalWurmApiConfig.ClearAllCaches)
            {
                ClearDir(heuristicsDataDirectory, logger);
            }

            WurmLogsHistory logsHistory =
                Wire(new WurmLogsHistory(logFiles,
                    logger,
                    heuristicsDataDirectory,
                    logFileStreamReaderFactory,
                    wurmApiConfig));

            WurmConfigs wurmConfigs =
                Wire(new WurmConfigs(configDirectories,
                    logger,
                    publicEventInvoker,
                    internalEventAggregator,
                    taskManager));
            WurmAutoruns autoruns = Wire(new WurmAutoruns(wurmConfigs, characterDirectories, logger));

            var wurmServerHistoryDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmServerHistory");
            if (internalWurmApiConfig.ClearAllCaches)
            {
                ClearDir(wurmServerHistoryDataDirectory, logger);
            }
            WurmServerHistory wurmServerHistory =
                Wire(new WurmServerHistory(wurmServerHistoryDataDirectory,
                    logsHistory,
                    serverList,
                    logger,
                    logsMonitor,
                    logFiles,
                    internalEventAggregator,
                    serverGroups,
                    wurmApiConfig));

            var wurmServersDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmServers");
            if (internalWurmApiConfig.ClearAllCaches)
            {
                ClearDir(wurmServersDataDirectory, logger);
            }
            WurmServers wurmServers =
                Wire(new WurmServers(logsHistory,
                    logsMonitor,
                    serverList,
                    httpWebRequests,
                    wurmServersDataDirectory,
                    characterDirectories,
                    wurmServerHistory,
                    logger,
                    serverGroups,
                    wurmApiConfig));

            WurmCharacters characters =
                Wire(new WurmCharacters(characterDirectories,
                    wurmConfigs,
                    wurmServers,
                    wurmServerHistory,
                    logger,
                    taskManager,
                    logsMonitor,
                    publicEventInvoker,
                    internalEventAggregator,
                    paths,
                    logsHistory,
                    serverGroups));

            HttpWebRequests = httpWebRequests;
            Autoruns = autoruns;
            Characters = characters;
            Configs = wurmConfigs;
            LogDefinitions = logDefinitions;
            LogsHistory = logsHistory;
            LogsMonitor = logsMonitor;
            Servers = wurmServers;
            WurmLogFiles = logFiles;
            WurmServerHistory = wurmServerHistory;
            WurmCharacterDirectories = characterDirectories;
            WurmConfigDirectories = configDirectories;
            InternalEventAggregator = internalEventAggregator;
            Paths = paths;
            ServerGroups = serverGroups;
            Logger = logger;
            ServersList = serverList;
        }

        void ClearDir(string directoryPath, IWurmApiLogger logger)
        {
            var di = new DirectoryInfo(directoryPath);
            if (di.Exists)
            {
                di.Delete(recursive: true);
                logger.Log(LogLevel.Info, "Clearing cache completed for dir " + directoryPath, this, null);
            }
        }

        public IWurmAutoruns Autoruns { get; private set; }
        public IWurmCharacters Characters { get; private set; }
        public IWurmConfigs Configs { get; private set; }
        public IWurmLogDefinitions LogDefinitions { get; private set; }
        public IWurmLogsHistory LogsHistory { get; private set; }
        public IWurmLogsMonitor LogsMonitor { get; private set; }
        public IWurmServers Servers { get; private set; }

        public IWurmPaths Paths { get; private set; }
        public IWurmServerGroups ServerGroups { get; private set; }

        public IWurmServerHistory WurmServerHistory { get; private set; }
        public IWurmCharacterDirectories WurmCharacterDirectories { get; private set; }
        public IWurmConfigDirectories WurmConfigDirectories { get; private set; }
        public IWurmLogFiles WurmLogFiles { get; private set; }
        public IWurmApiLogger Logger { get; private set; }

        internal IInternalEventAggregator InternalEventAggregator { get; private set; }
        internal IHttpWebRequests HttpWebRequests { get; private set; }

        public IWurmServerList ServersList { get; private set; }

        public void Dispose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }

        private TSystem Wire<TSystem>(TSystem system)
        {
            var disposable = system as IDisposable;
            if (disposable != null)
            {
                if (disposables.Contains(disposable))
                {
                    throw new InvalidOperationException("attempted to wire same object twice for IDisposable, obj type: " + system.GetType());
                }
                disposables.Add(disposable);
            }
            return system;
        }
    }

    internal class WurmApiTuningParams
    {
        public static TimeSpan PublicEventMarshallerDelay = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// Additional configuration options for WurmApi
    /// </summary>
    public class WurmApiConfig : IWurmApiConfig
    {
        /// <summary>
        /// Constructs new instance of WurmApiConfig with default settings.
        /// </summary>
        public WurmApiConfig()
        {
            Platform = Platform.Windows;
            ServerInfoMap = new Dictionary<ServerName, WurmServerInfo>();

            var defaultDescriptions = new[]
            {
                new WurmServerInfo(
                    Constants.ServerNames.GoldenValley,
                    "http://jenn001.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Independence",
                    "http://freedom001.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Deliverance",
                    "http://freedom002.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Exodus",
                    "http://freedom003.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Celebration",
                    "http://freedom004.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Chaos",
                    "http://wild001.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Pristine",
                    "http://freedom005.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Release",
                    "http://freedom006.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Xanadu",
                    "http://freedom007.game.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.FreedomId)),
                new WurmServerInfo(
                    "Elevation",
                    "http://elevation.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.EpicId)),
                new WurmServerInfo(
                    "Serenity",
                    "http://serenity.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.EpicId)),
                new WurmServerInfo(
                    "Desertion",
                    "http://desertion.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.EpicId)),
                new WurmServerInfo(
                    "Affliction",
                    "http://affliction.wurmonline.com/battles/stats.html",
                    new ServerGroup(ServerGroup.EpicId)),
            };

            foreach (var defaultDescription in defaultDescriptions)
            {
                ServerInfoMap.Add(defaultDescription.ServerName, defaultDescription);
            }

        }

        /// <summary>
        /// Current operating system
        /// </summary>
        public Platform Platform { get; set; }

        /// <summary>
        /// If set, will clear all WurmApi caches, such as log searching heuristics.
        /// </summary>
        public bool ClearAllCaches { get; set; }

        /// <summary>
        /// Set to maximize compatibility with Wurm Unlimited.
        /// </summary>
        public bool WurmUnlimitedMode { get; set; }

        /// <summary>
        /// Flat files are supported on Windows, Linux and Mac and are default saving method. Sqlite is currently supported only on Windows.
        /// </summary>
        public WurmApiPersistenceMethod PersistenceMethod { get; set; }

        /// <summary>
        /// Contains all default mappings between server names and their details - such as server group or stats url.
        /// Mappings can be modified, added and removed.
        /// </summary>
        public IDictionary<ServerName, WurmServerInfo> ServerInfoMap { get; private set; }

        internal WurmApiConfig CreateCopy()
        {
            var config = new WurmApiConfig
            {
                ClearAllCaches = ClearAllCaches,
                Platform = Platform,
                ServerInfoMap = ServerInfoMap,
                WurmUnlimitedMode = WurmUnlimitedMode,
                PersistenceMethod = PersistenceMethod
            };
            return config;
        }
    }

    internal interface IWurmApiConfig
    {
        Platform Platform { get; }

        bool ClearAllCaches { get; }

        bool WurmUnlimitedMode { get; }

        /// <summary>
        /// Contains all default mappings between server names and their details - such as server group or stats url.
        /// Mappings can be modified, added and removed.
        /// </summary>
        IDictionary<ServerName, WurmServerInfo> ServerInfoMap { get; }
    }

    /// <summary>
    /// Operating System
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Unknown / unspecified
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Any version of desktop Microsoft Windows
        /// </summary>
        Windows = 1,
        /// <summary>
        /// Any version of desktop Linux (eg. Ubuntu)
        /// </summary>
        Linux = 2,
        /// <summary>
        /// Any version of Apple MAC
        /// </summary>
        Mac = 3
    }
}
