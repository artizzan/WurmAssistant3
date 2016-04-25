using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    class ServerHistoryProvider : IDisposable
    {
        readonly CharacterName characterName;
        readonly SortedServerHistory sortedServerHistory;
        readonly IPersistent<PersistentModel.ServerHistory> persistentData;
        readonly IWurmLogsMonitorInternal logsMonitor;
        readonly IWurmLogsHistory logsSearcher;
        readonly IWurmServerList wurmServerList;
        readonly IWurmApiLogger logger;
        readonly IWurmCharacterLogFiles wurmCharacterLogFiles;
        readonly IInternalEventAggregator eventAggregator;

        ServerName currentLiveLogsServer;

        readonly ConcurrentQueue<LogEntry[]> liveEventsToParse = new ConcurrentQueue<LogEntry[]>();

        static readonly DateTime NoPreviousSearchBoundary = new DateTime(1901, 1, 1);

        public ServerHistoryProvider(
            [NotNull] CharacterName characterName, 
            [NotNull] IPersistent<PersistentModel.ServerHistory> persistentData,
            [NotNull] IWurmLogsMonitorInternal logsMonitor,
            [NotNull] IWurmLogsHistory logsSearcher,
            [NotNull] IWurmServerList wurmServerList,
            [NotNull] IWurmApiLogger logger,
            [NotNull] IWurmCharacterLogFiles wurmCharacterLogFiles, 
            [NotNull] IInternalEventAggregator eventAggregator)
        {
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));
            if (persistentData == null) throw new ArgumentNullException(nameof(persistentData));
            if (logsMonitor == null) throw new ArgumentNullException(nameof(logsMonitor));
            if (logsSearcher == null) throw new ArgumentNullException(nameof(logsSearcher));
            if (wurmServerList == null) throw new ArgumentNullException(nameof(wurmServerList));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException(nameof(wurmCharacterLogFiles));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            this.characterName = characterName;
            sortedServerHistory = new SortedServerHistory(persistentData);
            this.persistentData = persistentData;
            this.logsMonitor = logsMonitor;
            this.logsSearcher = logsSearcher;
            this.wurmServerList = wurmServerList;
            this.logger = logger;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;
            this.eventAggregator = eventAggregator;

            eventAggregator.Subscribe(this);
            logsMonitor.SubscribeInternal(characterName, LogType.Event, HandleEventLogEntries);
        }

        #region Not Thread Safe

        private void HandleEventLogEntries(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            // this handler may run on arbitrary threads!

            if (logsMonitorEventArgs.CharacterName == characterName)
            {
                if (logsMonitorEventArgs.LogType == LogType.Event)
                {
                    liveEventsToParse.Enqueue(logsMonitorEventArgs.WurmLogEntries.ToArray());
                }
            }
        }

        #endregion

        public void ParsePendingLiveLogEvents()
        {
            LogEntry[] logEntries;
            while (liveEventsToParse.TryDequeue(out logEntries))
            {
                ParseForServerInfo(logEntries, true);
            }
        }

        bool ParseForServerInfo(IEnumerable<LogEntry> logEntries, bool fromLiveLogs = false)
        {
            bool foundAny = false;
            foreach (var wurmLogEntry in logEntries)
            {
                var serverStamp = wurmLogEntry.TryGetServerFromLogEntry(logger, characterName);
                if (serverStamp != null)
                {
                    ServerName previousServerName = null;
                    if (fromLiveLogs)
                    {
                        previousServerName = currentLiveLogsServer
                                             ?? sortedServerHistory.TryGetServerAtStamp(Time.Get.LocalNow);
                    }

                    sortedServerHistory.Insert(serverStamp);
                    foundAny = true;

                    if (fromLiveLogs)
                    {
                        currentLiveLogsServer = serverStamp.ServerName;

                        eventAggregator.Send(new YouAreOnEventDetectedOnLiveLogs(currentLiveLogsServer,
                            characterName,
                            previousServerName != currentLiveLogsServer));
                    }
                }
            }
            return foundAny;
        }

        public ServerName TryGetAtTimestamp(DateTime timestamp, JobCancellationManager jobCancellationManager)
        {
            ParsePendingLiveLogEvents();

            var timeNow = Time.Get.LocalNow;
            var time12MonthsAgo = timeNow.AddMonths(-12);

            // a cheap hack to reset history if its very outdated
            if (persistentData.Entity.SearchedFrom < time12MonthsAgo
                && persistentData.Entity.SearchedTo < time12MonthsAgo)
            {
                persistentData.Entity.Reset();
            }

            if (persistentData.Entity.AnySearchCompleted 
                && timestamp >= persistentData.Entity.SearchedFrom
                && timestamp <= persistentData.Entity.SearchedTo)
            {
                // within already scanned period
                ServerName serverName = sortedServerHistory.TryGetServerAtStamp(timestamp);
                if (serverName != null)
                {
                    // we have data, return
                    return serverName;
                }
                // no data, we should continue
            }

            jobCancellationManager.ThrowIfCancelled();

            bool found = false;

            if (persistentData.Entity.AnySearchCompleted)
            {
                if (timestamp > persistentData.Entity.SearchedTo)
                {
                    found = SearchLogsForwards(persistentData.Entity.SearchedTo, jobCancellationManager);
                }

                if (!found || timestamp < persistentData.Entity.SearchedFrom)
                {
                    SearchLogsBackwards(persistentData.Entity.SearchedFrom, jobCancellationManager);
                }
            }
            else
            {
                found = SearchLogsForwards(timestamp, jobCancellationManager);
                if (!found)
                {
                    SearchLogsBackwards(timestamp, jobCancellationManager);
                }
                persistentData.Entity.AnySearchCompleted = true;
            }
           
            persistentData.FlagAsChanged();

            return sortedServerHistory.TryGetServerAtStamp(timestamp);
        }

        /// <summary>
        /// True if any data found
        /// </summary>
        private bool SearchLogsForwards(DateTime datetimeFrom, JobCancellationManager jobCancellationManager)
        {
            var dateFrom = datetimeFrom;
            var dateTo = datetimeFrom + TimeSpan.FromDays(30);

            var timeNow = Time.Get.LocalNow;
            bool end = false;
            if (dateTo > timeNow)
            {
                dateTo = timeNow;
                end = true;
            }

            var results = logsSearcher.Scan(
                new LogSearchParameters()
                {
                    CharacterName = characterName.Normalized,
                    MinDate = dateFrom,
                    MaxDate = dateTo,
                    LogType = LogType.Event
                }, jobCancellationManager.GetLinkedToken());

            UpdateEntity(dateFrom, dateTo);

            var found = ParseForServerInfo(results);

            if (found)
            {
                return true;
            }
            if (end)
            {
                return false;
            }

            return SearchLogsForwards(dateTo, jobCancellationManager);
        }

        private void UpdateEntity(DateTime dateFrom, DateTime dateTo)
        {
            if (persistentData.Entity.SearchedTo < NoPreviousSearchBoundary || dateTo > persistentData.Entity.SearchedTo)
            {
                persistentData.Entity.SearchedTo = dateTo;
                persistentData.FlagAsChanged();
            }
            if (persistentData.Entity.SearchedFrom < NoPreviousSearchBoundary || dateFrom < persistentData.Entity.SearchedFrom)
            {
                persistentData.Entity.SearchedFrom = dateFrom;
                persistentData.FlagAsChanged();
            }
        }

        /// <summary>
        /// True if any data found
        /// </summary>
        private bool SearchLogsBackwards(DateTime datetimeTo, JobCancellationManager jobCancellationManager)
        {
            var dateFrom = datetimeTo - TimeSpan.FromDays(30);

            if (datetimeTo < wurmCharacterLogFiles.OldestLogFileDate)
            {
                return false;
            }

            var results = logsSearcher.Scan(
                new LogSearchParameters()
                {
                    CharacterName = characterName.Normalized,
                    MinDate = dateFrom,
                    MaxDate = datetimeTo,
                    LogType = LogType.Event
                }, jobCancellationManager.GetLinkedToken());

            UpdateEntity(dateFrom, datetimeTo);

            var found = ParseForServerInfo(results);
            if (found)
            {
                return true;
            }

            return SearchLogsBackwards(dateFrom, jobCancellationManager);
        }

        public ServerName TryGetCurrentServer(JobCancellationManager jobCancellationManager)
        {
            ParsePendingLiveLogEvents();

            if (currentLiveLogsServer != null)
            {
                return currentLiveLogsServer;
            }
            return TryGetAtTimestamp(Time.Get.LocalNow, jobCancellationManager);
        }

        public void Dispose()
        {
            logsMonitor.Unsubscribe(characterName.Normalized, HandleEventLogEntries);
        }

        public ServerName CheckCache(DateTime exactDate)
        {
            return sortedServerHistory.TryGetServerAtStamp(exactDate);
        }
    }
}