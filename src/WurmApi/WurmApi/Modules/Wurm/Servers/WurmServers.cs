using System;
using System.Collections.Generic;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.Modules.Wurm.Servers.Jobs;
using AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel;
using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.PersistentObjects.FlatFiles;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    /// <summary>
    /// Manages information about wurm game servers.
    /// </summary>
    class WurmServers : IWurmServers, IDisposable
    {
        readonly IWurmServerGroups wurmServerGroups;
        readonly IWurmApiConfig wurmApiConfig;
        readonly Dictionary<ServerName, WurmServer> nameToServerMap = new Dictionary<ServerName, WurmServer>();

        readonly WurmServerFactory wurmServerFactory;
        readonly LiveLogsDataQueue liveLogsDataQueue;

        readonly QueuedJobsSyncRunner<Job, JobResult> runner;

        readonly PersistentCollectionsLibrary persistentCollectionsLibrary;

        public WurmServers(
            [NotNull] IWurmLogsHistory wurmLogsHistory,
            [NotNull] IWurmLogsMonitorInternal wurmLogsMonitor,
            [NotNull] IWurmServerList wurmServerList,
            [NotNull] IHttpWebRequests httpWebRequests,
            [NotNull] string dataDirectory,
            [NotNull] IWurmCharacterDirectories wurmCharacterDirectories,
            [NotNull] IWurmServerHistory wurmServerHistory,
            [NotNull] IWurmApiLogger logger, 
            [NotNull] IWurmServerGroups wurmServerGroups, 
            [NotNull] IWurmApiConfig wurmApiConfig)
        {
            if (wurmLogsHistory == null) throw new ArgumentNullException(nameof(wurmLogsHistory));
            if (wurmLogsMonitor == null) throw new ArgumentNullException(nameof(wurmLogsMonitor));
            if (wurmServerList == null) throw new ArgumentNullException(nameof(wurmServerList));
            if (httpWebRequests == null) throw new ArgumentNullException(nameof(httpWebRequests));
            if (dataDirectory == null) throw new ArgumentNullException(nameof(dataDirectory));
            if (wurmCharacterDirectories == null) throw new ArgumentNullException(nameof(wurmCharacterDirectories));
            if (wurmServerHistory == null) throw new ArgumentNullException(nameof(wurmServerHistory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmServerGroups == null) throw new ArgumentNullException(nameof(wurmServerGroups));
            if (wurmApiConfig == null) throw new ArgumentNullException(nameof(wurmApiConfig));

            this.wurmServerGroups = wurmServerGroups;
            this.wurmApiConfig = wurmApiConfig;

            liveLogsDataQueue = new LiveLogsDataQueue(wurmLogsMonitor);
            LiveLogs liveLogs = new LiveLogs(liveLogsDataQueue, wurmServerHistory);

            IPersistenceStrategy persistenceStrategy = new FlatFilesPersistenceStrategy(dataDirectory);

            persistentCollectionsLibrary =
                new PersistentCollectionsLibrary(persistenceStrategy,
                    new PersObjErrorHandlingStrategy(logger));
            var persistent = persistentCollectionsLibrary.DefaultCollection.GetObject<ServersData>("WurmServers");
            LogHistorySaved logHistorySaved = new LogHistorySaved(persistent);
            LogHistory logHistory = new LogHistory(wurmLogsHistory,
                wurmCharacterDirectories,
                wurmServerHistory,
                logHistorySaved,
                new LogEntriesParser(),
                logger);
            
            WebFeeds webFeeds = new WebFeeds(httpWebRequests, wurmServerList, logger);

            runner = new QueuedJobsSyncRunner<Job, JobResult>(new JobRunner(liveLogs, logHistory, webFeeds, persistentCollectionsLibrary), logger);

            wurmServerFactory = new WurmServerFactory(runner);

            var descriptions = wurmServerList.All;
            foreach (var serverDescription in descriptions)
            {
                RegisterServer(serverDescription);
            }
        }

        private WurmServer RegisterServer(WurmServerInfo wurmServerInfo)
        {
            var normalizedName = wurmServerInfo.ServerName;
            if (nameToServerMap.ContainsKey(normalizedName))
            {
                throw new WurmApiException("Server already registered: " + wurmServerInfo);
            }

            var server = wurmServerFactory.Create(wurmServerInfo);

            nameToServerMap.Add(normalizedName, server);

            return server;
        }

        public IEnumerable<IWurmServer> All => nameToServerMap.Values;

        public IWurmServer GetByName(ServerName name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            WurmServer server;
            if (!nameToServerMap.TryGetValue(name, out server))
            {
                // registering unknown server
                return RegisterServer(new WurmServerInfo(name.Original, String.Empty, wurmServerGroups.GetForServer(name)));
            }
            return server;
        }

        public IWurmServer GetByName(string name)
        {
            return GetByName(new ServerName(name));
        }

        public void Dispose()
        {
            persistentCollectionsLibrary.SaveChanged();
            liveLogsDataQueue.Dispose();
            runner.Dispose();
        }
    }
}