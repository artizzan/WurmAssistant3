using System;
using System.Threading.Tasks;
using AldursLab.Essentials;
using AldurSoft.WurmApi.Modules.Wurm.Servers.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class LogHistory
    {
        readonly IWurmLogsHistory wurmLogsHistory;
        readonly IWurmCharacterDirectories wurmCharacterDirectories;
        readonly IWurmServerHistory wurmServerHistory;
        readonly LogHistorySaved logHistorySaved;
        readonly LogEntriesParser parser;

        bool scanned = false;

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

        public TimeDetails GetForServer(ServerName serverName)
        {
            EnsureScanned();

            logHistorySaved.LastScanDate = Time.Get.LocalNowOffset;

            return logHistorySaved.GetHistoricForServer(serverName);
        }

        void EnsureScanned()
        {
            if (scanned) return;

            var maxScanSince = Time.Get.LocalNowOffset.AddDays(-30);
            var lastScanSince = logHistorySaved.LastScanDate.AddDays(-1);
            var scanSince = lastScanSince < maxScanSince ? maxScanSince : lastScanSince;

            var allChars = wurmCharacterDirectories.GetAllCharacters();
            foreach (var characterName in allChars)
            {
                var searchResults = wurmLogsHistory.Scan(
                    new LogSearchParameters()
                    {
                        CharacterName = characterName,
                        DateFrom = scanSince.DateTime,
                        DateTo = Time.Get.LocalNow,
                        LogType = LogType.Event
                    });
                foreach (var searchResult in searchResults)
                {
                    var upt = parser.TryParseUptime(searchResult);
                    if (upt != null)
                    {
                        var server = wurmServerHistory.GetServer(characterName, searchResult.Timestamp);
                        if (server != null)
                        {
                            logHistorySaved.UpdateHistoric(server, upt);
                        }
                    }
                    var wdt = parser.TryParseWurmDateTime(searchResult);
                    if (wdt != null)
                    {
                        var server = wurmServerHistory.GetServer(characterName, searchResult.Timestamp);
                        if (server != null)
                        {
                            logHistorySaved.UpdateHistoric(server, wdt);
                        }
                    }
                }
            }

            scanned = true;
        }
    }
}