using System;
using System.Collections.Generic;
using System.IO;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Networking;
using AldurSoft.WurmApi.Modules.Wurm.Autoruns;
using AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Characters;
using AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Configs;
using AldurSoft.WurmApi.Modules.Wurm.InstallDirectory;
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
    public sealed class WurmApiManager : IWurmApi, IDisposable
    {
        readonly List<IRequireRefresh> requireRefreshes = new List<IRequireRefresh>();
        readonly List<IDisposable> disposables = new List<IDisposable>();

        public WurmApiManager(
            WurmApiDataDirectory dataDirectory,
            WurmInstallDirectory installDirectory,
            ILogger wurmApiLogger,
            IEventMarshaller eventMarshaller = null)
        {
            if (eventMarshaller == null) eventMarshaller = new ThreadPoolMarshaller(wurmApiLogger);
            IHttpWebRequests httpRequests = new HttpWebRequests();
            ConstructSystems(dataDirectory.FullPath, installDirectory, httpRequests, wurmApiLogger, eventMarshaller);
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
            ConstructSystems(dataDir, installDirectory, httpWebRequests, logger, new ThreadPoolMarshaller(logger));
        }

        void ConstructSystems(string wurmApiDataDirectoryFullPath, IWurmInstallDirectory installDirectory,
            IHttpWebRequests httpWebRequests, ILogger logger, IEventMarshaller publicEventMarshaller)
        {
            Wire(installDirectory);
            Wire(httpWebRequests);

            ThreadPoolMarshaller internalEventMarshaller = new ThreadPoolMarshaller(logger);

            InternalEventAggregator internalEventAggregator = new InternalEventAggregator();
            PublicEventInvoker publicEventInvoker = new PublicEventInvoker(publicEventMarshaller, logger);
            InternalEventInvoker internalEventInvoker = new InternalEventInvoker(internalEventAggregator, logger, internalEventMarshaller);

            WurmPaths paths = Wire(new WurmPaths(installDirectory));

            WurmServerList serverList = Wire(new WurmServerList());

            WurmLogDefinitions logDefinitions = Wire(new WurmLogDefinitions());

            WurmConfigDirectories configDirectories = Wire(new WurmConfigDirectories(paths, internalEventAggregator));
            WurmCharacterDirectories characterDirectories = Wire(new WurmCharacterDirectories(paths, internalEventAggregator));
            WurmLogFiles logFiles =
                Wire(new WurmLogFiles(characterDirectories, logger, logDefinitions, internalEventAggregator,
                    internalEventInvoker));

            WurmLogsMonitor logsMonitor =
                Wire(new WurmLogsMonitor(logFiles, logger, publicEventInvoker, internalEventAggregator,
                    characterDirectories, internalEventInvoker));
            var heuristicsDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmLogsHistory");
            WurmLogsHistory logsHistory = Wire(new WurmLogsHistory(logFiles, logger, heuristicsDataDirectory));

            WurmConfigs wurmConfigs = Wire(new WurmConfigs(configDirectories, logger, publicEventInvoker, internalEventAggregator));
            WurmAutoruns autoruns = Wire(new WurmAutoruns(wurmConfigs, characterDirectories, logger));

            var wurmServerHistoryDataDirectory = Path.Combine(wurmApiDataDirectoryFullPath, "WurmServerHistory");
            WurmServerHistory wurmServerHistory =
                Wire(new WurmServerHistory(wurmServerHistoryDataDirectory, logsHistory, serverList, logger, logsMonitor, logFiles));

            WurmServers wurmServers =
                Wire(new WurmServers(logsHistory, logsMonitor, serverList, httpWebRequests, "WurmServers",
                    characterDirectories, wurmServerHistory, logger));

            WurmCharacters characters =
                Wire(new WurmCharacters(characterDirectories, wurmConfigs, wurmServers, wurmServerHistory, logger));

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
