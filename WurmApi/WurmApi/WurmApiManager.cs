using System;
using System.Collections.Generic;
using System.IO;
using AldursLab.Essentials.Eventing;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Networking;
using AldurSoft.WurmApi.Modules.Wurm.Autoruns;
using AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Characters;
using AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Configs;
using AldurSoft.WurmApi.Modules.Wurm.LogDefinitions;
using AldurSoft.WurmApi.Modules.Wurm.LogFiles;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory;
using AldurSoft.WurmApi.Modules.Wurm.LogsMonitor;
using AldurSoft.WurmApi.Modules.Wurm.Paths;
using AldurSoft.WurmApi.Modules.Wurm.ServerHistory;
using AldurSoft.WurmApi.Modules.Wurm.Servers;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Host of all WurmApi services.
    /// </summary>
    public sealed class WurmApiManager : IWurmApi, IDisposable
    {
        readonly List<IRequireRefresh> requireRefreshes = new List<IRequireRefresh>();
        readonly List<IDisposable> disposables = new List<IDisposable>();

        /// <summary>
        /// Public factory constructor.
        /// </summary>
        internal WurmApiManager(
            WurmApiDataDirectory dataDirectory,
            IWurmInstallDirectory installDirectory,
            ILogger wurmApiLogger,
            IEventMarshaller publicEventMarshaller)
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
                threadPoolMarshaller);
        }

        /// <summary>
        /// Constructor for integration testing.
        /// </summary>
        internal WurmApiManager(
            string dataDir,
            IWurmInstallDirectory installDirectory,
            IHttpWebRequests httpWebRequests,
            ILogger logger)
        {
            ConstructSystems(dataDir, installDirectory, httpWebRequests, logger, new SimpleMarshaller(), new SimpleMarshaller());
        }

        void ConstructSystems(string wurmApiDataDirectoryFullPath, IWurmInstallDirectory installDirectory,
            IHttpWebRequests httpWebRequests, ILogger logger, IEventMarshaller publicEventMarshaller, IEventMarshaller internalEventMarshaller)
        {
            Wire(installDirectory);
            Wire(httpWebRequests);

            if (logger == null) logger = new LoggerStub();

            PublicEventInvoker publicEventInvoker = Wire(new PublicEventInvoker(publicEventMarshaller, logger));

            TaskManager taskManager = Wire(new TaskManager(logger));

            InternalEventAggregator internalEventAggregator = Wire(new InternalEventAggregator());

            InternalEventInvoker internalEventInvoker = Wire(new InternalEventInvoker(internalEventAggregator, logger, internalEventMarshaller));

            WurmPaths paths = Wire(new WurmPaths(installDirectory));

            WurmServerList serverList = Wire(new WurmServerList());

            WurmLogDefinitions logDefinitions = Wire(new WurmLogDefinitions());

            WurmConfigDirectories configDirectories = Wire(new WurmConfigDirectories(paths, internalEventAggregator, taskManager));
            WurmCharacterDirectories characterDirectories = Wire(new WurmCharacterDirectories(paths, internalEventAggregator, taskManager));
            WurmLogFiles logFiles =
                Wire(new WurmLogFiles(characterDirectories, logger, logDefinitions, internalEventAggregator,
                    internalEventInvoker, taskManager));

            WurmLogsMonitor logsMonitor =
                Wire(new WurmLogsMonitor(logFiles,
                    logger,
                    publicEventInvoker,
                    internalEventAggregator,
                    characterDirectories,
                    internalEventInvoker,
                    taskManager));
            var heuristicsDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmLogsHistory");
            WurmLogsHistory logsHistory = Wire(new WurmLogsHistory(logFiles, logger, heuristicsDataDirectory));

            WurmConfigs wurmConfigs =
                Wire(new WurmConfigs(configDirectories,
                    logger,
                    publicEventInvoker,
                    internalEventAggregator,
                    taskManager));
            WurmAutoruns autoruns = Wire(new WurmAutoruns(wurmConfigs, characterDirectories, logger));

            var wurmServerHistoryDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmServerHistory");
            WurmServerHistory wurmServerHistory =
                Wire(new WurmServerHistory(wurmServerHistoryDataDirectory, logsHistory, serverList, logger, logsMonitor, logFiles));

            var wurmServersDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmServers");
            WurmServers wurmServers =
                Wire(new WurmServers(logsHistory, logsMonitor, serverList, httpWebRequests, wurmServersDataDirectory,
                    characterDirectories, wurmServerHistory, logger));

            WurmCharacters characters =
                Wire(new WurmCharacters(characterDirectories,
                    wurmConfigs,
                    wurmServers,
                    wurmServerHistory,
                    logger,
                    taskManager));

            HttpWebRequests = httpWebRequests;
            WurmAutoruns = autoruns;
            WurmCharacters = characters;
            WurmConfigs = wurmConfigs;
            WurmLogDefinitions = logDefinitions;
            WurmLogsHistory = logsHistory;
            WurmLogsMonitor = logsMonitor;
            WurmServers = wurmServers;
            WurmLogFiles = logFiles;

            // internal systems

            WurmServerHistory = wurmServerHistory;
            WurmCharacterDirectories = characterDirectories;
            WurmConfigDirectories = configDirectories;
            InternalEventAggregator = internalEventAggregator;
        }

        public IWurmAutoruns WurmAutoruns { get; private set; }
        public IWurmCharacters WurmCharacters { get; private set; }
        public IWurmConfigs WurmConfigs { get; private set; }
        public IWurmLogDefinitions WurmLogDefinitions { get; private set; }
        public IWurmLogsHistory WurmLogsHistory { get; private set; }
        public IWurmLogsMonitor WurmLogsMonitor { get; private set; }
        public IWurmServers WurmServers { get; private set; }

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

        // internal systems

        internal IWurmServerHistory WurmServerHistory { get; private set; }
        internal IWurmCharacterDirectories WurmCharacterDirectories { get; private set; }
        internal IWurmConfigDirectories WurmConfigDirectories { get; private set; }
        internal IInternalEventAggregator InternalEventAggregator { get; private set; }
        internal IHttpWebRequests HttpWebRequests { get; private set; }
        public IWurmLogFiles WurmLogFiles { get; private set; }
    }

    internal class WurmApiTuningParams
    {
        public static TimeSpan PublicEventMarshallerDelay = TimeSpan.FromMilliseconds(500);
    }
}
