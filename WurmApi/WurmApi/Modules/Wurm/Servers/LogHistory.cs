using System;
using System.Threading.Tasks;
using AldurSoft.Core;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class LogHistory
    {
        private readonly IWurmLogsHistory wurmLogsHistory;
        private readonly IWurmCharacterDirectories wurmCharacterDirectories;
        private readonly IWurmServerHistory wurmServerHistory;
        private readonly LogHistorySaved logHistorySaved;
        private readonly LogEntriesParser parser;

        private bool scanned = false;

        public LogHistory(
            IWurmLogsHistory wurmLogsHistory,
            IWurmCharacterDirectories wurmCharacterDirectories,
            IWurmServerHistory wurmServerHistory,
            LogHistorySaved logHistorySaved,
            LogEntriesParser parser)
        {
            if (wurmLogsHistory == null) throw new ArgumentNullException("wurmLogsHistory");
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logHistorySaved == null) throw new ArgumentNullException("logHistorySaved");
            if (parser == null) throw new ArgumentNullException("parser");
            this.wurmLogsHistory = wurmLogsHistory;
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.wurmServerHistory = wurmServerHistory;
            this.logHistorySaved = logHistorySaved;
            this.parser = parser;
        }

        private Task scanTask;

        public async Task<TimeDetails> GetForServer(ServerName serverName)
        {
            if (!scanned)
            {
                if (scanTask != null)
                {
                    await scanTask;
                }
                else
                {
                    try
                    {
                        scanTask = Scan();
                        await scanTask;
                    }
                    finally
                    {
                        scanTask = null;
                    }
                }
            }

            return logHistorySaved.GetHistoricForServer(serverName);
        }

        private async Task Scan()
        {
            var maxScanSince = Time.Clock.LocalNowOffset.AddDays(-30);
            var lastScanSince = logHistorySaved.LastScanDate.AddDays(-1);
            var scanSince = lastScanSince < maxScanSince ? maxScanSince : lastScanSince;

            var allChars = wurmCharacterDirectories.GetAllCharacters();
            foreach (var characterName in allChars)
            {
                var searchResults = await wurmLogsHistory.Scan(
                    new LogSearchParameters()
                    {
                        CharacterName = characterName,
                        DateFrom = scanSince.DateTime,
                        DateTo = Time.Clock.LocalNow,
                        LogType = LogType.Event
                    });
                foreach (var searchResult in searchResults)
                {
                    var upt = parser.TryParseUptime(searchResult);
                    if (upt != null)
                    {
                        var server = await wurmServerHistory.GetServer(characterName, searchResult.Timestamp);
                        if (server != null)
                        {
                            logHistorySaved.UpdateHistoric(server, upt);
                        }
                        
                    }
                    var wdt = parser.TryParseWurmDateTime(searchResult);
                    if (wdt != null)
                    {
                        var server = await wurmServerHistory.GetServer(characterName, searchResult.Timestamp);
                        if (server != null)
                        {
                            logHistorySaved.UpdateHistoric(server, wdt);
                        }
                    }
                }
            }

            scanned = true;
            logHistorySaved.LastScanDate = Time.Clock.LocalNowOffset;
        }
    }
}