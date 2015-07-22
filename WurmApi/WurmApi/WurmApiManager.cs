using System;
using System.Collections.Generic;

using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Networking;
using AldurSoft.WurmApi.Networking.HttpWebRequestsModule;
using AldurSoft.WurmApi.Persistence;
using AldurSoft.WurmApi.Persistence.WurmApiDataContextModule;
using AldurSoft.WurmApi.Validation;
using AldurSoft.WurmApi.Validation.ThreadGuardModule;
using AldurSoft.WurmApi.Wurm.Autoruns;
using AldurSoft.WurmApi.Wurm.Autoruns.WurmAutorunsModule;
using AldurSoft.WurmApi.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Wurm.CharacterDirectories.WurmCharacterDirectoriesModule;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Characters.WurmCharactersModule;
using AldurSoft.WurmApi.Wurm.CharacterServers;
using AldurSoft.WurmApi.Wurm.CharacterServers.WurmServerHistoryModule;
using AldurSoft.WurmApi.Wurm.Configs;
using AldurSoft.WurmApi.Wurm.Configs.WurmConfigDirectoriesModule;
using AldurSoft.WurmApi.Wurm.Configs.WurmConfigsModule;
using AldurSoft.WurmApi.Wurm.GameClients;
using AldurSoft.WurmApi.Wurm.GameClients.WurmGameClientsModule;
using AldurSoft.WurmApi.Wurm.Logs;
using AldurSoft.WurmApi.Wurm.Logs.Monitoring;
using AldurSoft.WurmApi.Wurm.Logs.Monitoring.WurmLogsMonitorModule;
using AldurSoft.WurmApi.Wurm.Logs.Searching;
using AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogDefinitionsModule;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogFilesModule;
using AldurSoft.WurmApi.Wurm.Paths;
using AldurSoft.WurmApi.Wurm.Paths.WurmInstallDirectoryModule;
using AldurSoft.WurmApi.Wurm.Paths.WurmPathsModule;
using AldurSoft.WurmApi.Wurm.Servers;
using AldurSoft.WurmApi.Wurm.Servers.WurmServersModule;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Core implementation of the WurmApi. This is all, that is needed to consume entire Api.
    /// </summary>
    /// <remarks>
    /// WurmApi, at bare minimum, requires path to a directory, where it can store its state. 
    /// This directory should not be shared with anything else. You can use <see cref="DefaultWurmApiConfig"/>,
    /// or override the implementation
    /// 
    /// As soon as API is constructed, it is ready for use.
    /// 
    /// WurmApi has to be continously updated, by calling Update() method. Recommended value is roughly half of a second.
    /// WurmApi is not thread safe and all interaction with WurmApi objects should be done on same thread, that constructed it. Attempting to call any method from other threads will result in an exception.
    /// Furthermore, thread that runs WurmApi should have a Dispatcher, so that it can properly dispatch async results back to correct thread.
    /// The simpliest way to achieve above is to create Api on main UI thread and call it with a Timer synchronized to UI thread
    /// (for example WPF DispatcherTimer or WinForms Timer control).
    /// 
    /// WurmApi offers many async methods, for operations that could otherwise block calling thread for long periods of time.
    /// When handling async methods, do not use Thread.Wait() or Task.Wait(). Doing so may hang your application, 
    /// as Api relies internally on dispatching results back to the same thread that called them. 
    /// 
    /// API keeps some state between sessions. Should there ever be a need to reset it, 
    /// the best practice is to restart the process (app) and before WurmApi is constructed,
    /// wipe its data directory of all contents. Alternatively WurmApi can just be disposed and recreated,
    /// but any active objects obtained from old (disposed instance of) WurmApi will no longer function.
    /// </remarks>
    public sealed class WurmApiManager : IWurmApi, IWurmApiController, IWurmApiInternal, IDisposable
    {
        readonly List<IRequireRefresh> requireRefreshes = new List<IRequireRefresh>();
        readonly List<IDisposable> disposables = new List<IDisposable>();

        private readonly IThreadGuard threadGuard;

        public WurmApiManager(
            WurmApiDataDirectory dataDirectory,
            WurmInstallDirectory installDirectory,
            ILogger wurmApiLogger)
        {
            threadGuard = new ThreadGuard();
            IHttpWebRequests httpRequests = new HttpWebRequests();
            ConstructSystems(dataDirectory.FullPath, installDirectory, httpRequests, wurmApiLogger);
        }

        /// <summary>
        /// Constructor for integration testing.
        /// </summary>
        internal WurmApiManager(
            string dataDir,
            IWurmInstallDirectory installDirectory,
            IHttpWebRequests httpWebRequests,
            ILogger logger,
            bool disableThreadGuard = true)
        {
            if (disableThreadGuard)
            {
                threadGuard = new ThreadGuardStub();
            }
            ConstructSystems(dataDir, installDirectory, httpWebRequests, logger);
        }

        private void ConstructSystems(
            string wurmApiDataDirectoryFullPath,
            IWurmInstallDirectory installDirectory,
            IHttpWebRequests httpWebRequests,
            ILogger logger)
        {
            Wire(installDirectory);
            Wire(httpWebRequests);

            IWurmApiDataContext dataContext =
                Wire(new WurmApiDataContext(wurmApiDataDirectoryFullPath, Wire(new SimplePersistLoggerAdapter(logger))));

            IWurmPaths paths = Wire(new WurmPaths(installDirectory));
            IWurmGameClients clients = Wire(new WurmGameClients());

            IWurmServerList serverList = Wire(new WurmServerList());

            IWurmLogDefinitions logDefinitions = Wire(new WurmLogDefinitions());

            IWurmConfigDirectories configDirectories = Wire(new WurmConfigDirectories(paths));
            IWurmCharacterDirectories characterDirectories = Wire(new WurmCharacterDirectories(paths));
            IWurmLogFiles logFiles = Wire(new WurmLogFiles(characterDirectories, logger, logDefinitions));

            IWurmLogsMonitor logsMonitor = Wire(new WurmLogsMonitor(logFiles, logger, threadGuard));
            IWurmLogsHistory logsHistory = Wire(new WurmLogsHistory(dataContext, logFiles, logger, threadGuard));

            IWurmConfigs wurmConfigs = Wire(new WurmConfigs(clients, configDirectories, logger, threadGuard));
            IWurmAutoruns autoruns = Wire(new WurmAutoruns(wurmConfigs, characterDirectories, threadGuard));

            IWurmServerHistory wurmServerHistory =
                Wire(new WurmServerHistory(dataContext, logsHistory, serverList, logger, logsMonitor, logFiles));

            IWurmServers wurmServers =
                Wire(
                    new WurmServers(
                        logsHistory,
                        logsMonitor,
                        serverList,
                        httpWebRequests,
                        dataContext,
                        characterDirectories,
                        wurmServerHistory,
                        logger, 
                        threadGuard));

            IWurmCharacters characters =
                Wire(new WurmCharacters(characterDirectories, wurmConfigs, wurmServers, wurmServerHistory, threadGuard));

            WurmAutoruns = autoruns;
            WurmCharacters = characters;
            WurmConfigs = wurmConfigs;
            WurmGameClients = clients;
            WurmLogDefinitions = logDefinitions;
            WurmLogsHistory = logsHistory;
            WurmLogsMonitor = logsMonitor;
            WurmServers = wurmServers;

            // internal systems

            WurmServerHistory = wurmServerHistory;
        }

        public IWurmAutoruns WurmAutoruns { get; private set; }
        public IWurmCharacters WurmCharacters { get; private set; }
        public IWurmConfigs WurmConfigs { get; private set; }
        public IWurmGameClients WurmGameClients { get; private set; }
        public IWurmLogDefinitions WurmLogDefinitions { get; private set; }
        public IWurmLogsHistory WurmLogsHistory { get; private set; }
        public IWurmLogsMonitor WurmLogsMonitor { get; private set; }
        public IWurmServers WurmServers { get; private set; }

        /// <summary>
        /// Updates engine state, allowing it to catch up on all real-time changes since last update and trigger all events.
        /// </summary>
        public void Update()
        {
            threadGuard.ValidateCurrentThread();
            foreach (var requireRefresh in requireRefreshes)
            {
                requireRefresh.Refresh();
            }
        }

        public void Dispose()
        {
            threadGuard.ValidateCurrentThread();
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~WurmApiManager()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }

        private TSystem Wire<TSystem>(TSystem system)
        {
            var requiresRefresh = system as IRequireRefresh;
            if (requiresRefresh != null)
            {
                if (requireRefreshes.Contains(requiresRefresh))
                {
                    throw new InvalidOperationException(
                        "attempted to wire same object twice for IRequireRefresh, obj type: " + system.GetType());
                }
                requireRefreshes.Add(requiresRefresh);
            }
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

        public IWurmServerHistory WurmServerHistory { get; private set; }
    }
}
