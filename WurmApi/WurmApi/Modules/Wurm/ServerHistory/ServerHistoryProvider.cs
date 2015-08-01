using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldurSoft.Core;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory;
using AldurSoft.WurmApi.Modules.Wurm.LogsMonitor;
using AldurSoft.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;

namespace AldurSoft.WurmApi.Modules.Wurm.ServerHistory
{
    class ServerHistoryProvider : IDisposable
    {
        readonly CharacterName characterName;
        readonly SortedServerHistory sortedServerHistory;
        readonly IPersistent<PersistentModel.ServerHistory> persistentData;
        readonly IWurmLogsMonitorInternal logsMonitor;
        readonly IWurmLogsHistory logsSearcher;
        readonly IWurmServerList wurmServerList;
        readonly ILogger logger;
        readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        ServerName currentLiveLogsServer;

        readonly ConcurrentQueue<LogEntry[]> liveEventsToParse = new ConcurrentQueue<LogEntry[]>();

        public ServerHistoryProvider(
            CharacterName characterName, IPersistent<PersistentModel.ServerHistory> persistentData,
            IWurmLogsMonitorInternal logsMonitor,
            IWurmLogsHistory logsSearcher,
            IWurmServerList wurmServerList,
            ILogger logger,
            IWurmCharacterLogFiles wurmCharacterLogFiles)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (persistentData == null) throw new ArgumentNullException("persistentData");
            if (logsMonitor == null) throw new ArgumentNullException("logsMonitor");
            if (logsSearcher == null) throw new ArgumentNullException("logsSearcher");
            if (wurmServerList == null) throw new ArgumentNullException("wurmServerList");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            this.characterName = characterName;
            this.sortedServerHistory = new SortedServerHistory(persistentData);
            this.persistentData = persistentData;
            this.logsMonitor = logsMonitor;
            this.logsSearcher = logsSearcher;
            this.wurmServerList = wurmServerList;
            this.logger = logger;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;

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

        private bool ParseForServerInfo(IEnumerable<LogEntry> logEntries, bool fromLiveLogs = false)
        {
            bool foundAny = false;
            foreach (var wurmLogEntry in logEntries)
            {
                if (Regex.IsMatch(wurmLogEntry.Content, @"^\d+ other players", RegexOptions.Compiled))
                {
                    Match match = Regex.Match(
                        wurmLogEntry.Content,
                        @"\d+ other players are online.*\. You are on (.+) \(",
                        RegexOptions.Compiled);
                    if (match.Success)
                    {
                        var serverName = new ServerName(match.Groups[1].Value.ToUpperInvariant());
                        var serverStamp = new ServerStamp() { ServerName = serverName, Timestamp = wurmLogEntry.Timestamp };
                        sortedServerHistory.Insert(serverStamp);
                        foundAny = true;
                        if (fromLiveLogs)
                        {
                            currentLiveLogsServer = serverName;
                        }
                    }
                    else
                    {
                        logger.Log(
                            LogLevel.Warn,
                            "ServerHistoryProvider found 'you are on' log line, but could not parse it. Entry: "
                            + wurmLogEntry,
                            this,
                            null);
                    }
                }
            }
            return foundAny;
        }

        public ServerName TryGetAtTimestamp(DateTime timestamp, JobCancellationManager jobCancellationManager)
        {
            ParsePendingLiveLogEvents();

            var timeNow = Time.Clock.LocalNow;
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

            var timeNow = Time.Clock.LocalNow;
            bool end = false;
            if (dateTo > timeNow)
            {
                dateTo = timeNow;
                end = true;
            }

            var results = logsSearcher.Scan(
                new LogSearchParameters()
                {
                    CharacterName = characterName,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    LogType = LogType.Event
                });

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
            if (dateTo > persistentData.Entity.SearchedTo)
            {
                persistentData.Entity.SearchedTo = dateTo;
                persistentData.FlagAsChanged();
            }
            if (dateFrom < persistentData.Entity.SearchedFrom)
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
                    CharacterName = characterName,
                    DateFrom = dateFrom,
                    DateTo = datetimeTo,
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
            return TryGetAtTimestamp(Time.Clock.LocalNow, jobCancellationManager);
        }

        public void Dispose()
        {
            logsMonitor.Unsubscribe(characterName, HandleEventLogEntries);
        }
    }
}