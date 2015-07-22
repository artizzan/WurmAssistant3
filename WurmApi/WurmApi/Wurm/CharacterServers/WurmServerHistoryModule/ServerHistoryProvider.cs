using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldurSoft.Core;
using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Persistence.DataModel.ServerHistoryModel;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Logs;
using AldurSoft.WurmApi.Wurm.Logs.Monitoring;
using AldurSoft.WurmApi.Wurm.Logs.Searching;
using AldurSoft.WurmApi.Wurm.Servers;

namespace AldurSoft.WurmApi.Wurm.CharacterServers.WurmServerHistoryModule
{
    public class ServerHistoryProvider : IDisposable
    {
        private readonly CharacterName characterName;
        private readonly SortedServerHistory sortedServerHistory;
        private readonly IPersistent<ServerHistory> serverHistoryRepository;
        private readonly IWurmLogsMonitor logsMonitor;
        private readonly IWurmLogsHistory logsSearcher;
        private readonly IWurmServerList wurmServerList;
        private readonly ILogger logger;
        private readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        private ServerName currentLiveLogsServer;

        public ServerHistoryProvider(
            CharacterName characterName,
            IPersistent<ServerHistory> serverHistoryRepository,
            IWurmLogsMonitor logsMonitor,
            IWurmLogsHistory logsSearcher,
            IWurmServerList wurmServerList,
            ILogger logger,
            IWurmCharacterLogFiles wurmCharacterLogFiles)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (serverHistoryRepository == null) throw new ArgumentNullException("serverHistoryRepository");
            if (logsMonitor == null) throw new ArgumentNullException("logsMonitor");
            if (logsSearcher == null) throw new ArgumentNullException("logsSearcher");
            if (wurmServerList == null) throw new ArgumentNullException("wurmServerList");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            this.characterName = characterName;
            this.sortedServerHistory = new SortedServerHistory(serverHistoryRepository);
            this.serverHistoryRepository = serverHistoryRepository;
            this.logsMonitor = logsMonitor;
            this.logsSearcher = logsSearcher;
            this.wurmServerList = wurmServerList;
            this.logger = logger;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;

            logsMonitor.Subscribe(characterName, LogType.Event, HandleEventLogEntries);
        }

        public void HandleEventLogEntries(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            if (logsMonitorEventArgs.CharacterName == characterName)
            {
                if (logsMonitorEventArgs.LogType == LogType.Event)
                {
                    ParseForServerInfo(logsMonitorEventArgs.WurmLogEntries, true);
                }
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
                        @"\d+ other players are online\. You are on (.+) \(",
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
                            this);
                    }
                }
            }
            return foundAny;
        }

        public virtual async Task<ServerName> TryGetAtTimestamp(DateTime timestamp)
        {
            var timeNow = Time.Clock.LocalNow;
            var time12MonthsAgo = timeNow.AddMonths(-12);

            // a cheap hack to reset history if its very outdated
            if (serverHistoryRepository.Entity.SearchedFrom < time12MonthsAgo
                && serverHistoryRepository.Entity.SearchedTo < time12MonthsAgo)
            {
                serverHistoryRepository.Entity.Reset();
            }

            if (serverHistoryRepository.Entity.AnySearchCompleted 
                && timestamp >= serverHistoryRepository.Entity.SearchedFrom
                && timestamp <= serverHistoryRepository.Entity.SearchedTo)
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

            if (serverHistoryRepository.Entity.AnySearchCompleted)
            {
                if (timestamp > serverHistoryRepository.Entity.SearchedTo)
                {
                    found = await SearchLogsForwards(serverHistoryRepository.Entity.SearchedTo);
                }

                if (!found || timestamp < serverHistoryRepository.Entity.SearchedFrom)
                {
                    await SearchLogsBackwards(serverHistoryRepository.Entity.SearchedFrom);
                }
            }
            else
            {
                found = await SearchLogsForwards(timestamp);
                if (!found)
                {
                    await SearchLogsBackwards(timestamp);
                }
                serverHistoryRepository.Entity.AnySearchCompleted = true;
            }
           
            serverHistoryRepository.SaveIfSetChanged();

            return sortedServerHistory.TryGetServerAtStamp(timestamp);
        }

        /// <summary>
        /// True if any data found
        /// </summary>
        private async Task<bool> SearchLogsForwards(DateTime datetimeFrom)
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

            var results = await logsSearcher.Scan(
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

            return await SearchLogsForwards(dateTo);
        }

        private void UpdateEntity(DateTime dateFrom, DateTime dateTo)
        {
            if (dateTo > serverHistoryRepository.Entity.SearchedTo)
            {
                serverHistoryRepository.Entity.SearchedTo = dateTo;
                serverHistoryRepository.SetChanged();
            }
            if (dateFrom < serverHistoryRepository.Entity.SearchedFrom)
            {
                serverHistoryRepository.Entity.SearchedFrom = dateFrom;
                serverHistoryRepository.SetChanged();
            }
        }

        /// <summary>
        /// True if any data found
        /// </summary>
        private async Task<bool> SearchLogsBackwards(DateTime datetimeTo)
        {
            var dateFrom = datetimeTo - TimeSpan.FromDays(30);

            if (datetimeTo < wurmCharacterLogFiles.OldestLogFileDate)
            {
                return false;
            }

            var results = await logsSearcher.Scan(
                new LogSearchParameters()
                {
                    CharacterName = characterName,
                    DateFrom = dateFrom,
                    DateTo = datetimeTo,
                    LogType = LogType.Event
                });

            UpdateEntity(dateFrom, datetimeTo);

            var found = ParseForServerInfo(results);
            if (found)
            {
                return true;
            }

            return await SearchLogsBackwards(dateFrom);
        }

        public virtual async Task<ServerName> TryGetCurrentServer()
        {
            if (currentLiveLogsServer != null)
            {
                return currentLiveLogsServer;
            }
            return await TryGetAtTimestamp(Time.Clock.LocalNow);
        }

        public void Dispose()
        {
            logsMonitor.Unsubscribe(HandleEventLogEntries);
        }
    }
}