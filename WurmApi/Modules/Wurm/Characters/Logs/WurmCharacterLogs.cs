using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Logs
{
    class WurmCharacterLogs : IWurmCharacterLogs
    {
        readonly IWurmCharacter character;
        readonly IWurmLogsHistory logsHistory;
        readonly IWurmApiLogger logger;

        public WurmCharacterLogs(
            [NotNull] IWurmCharacter character, 
            [NotNull] IWurmServerGroups serverGroups,
            [NotNull] IWurmLogsHistory logsHistory,
            [NotNull] IWurmServers wurmServers, 
            [NotNull] IWurmApiLogger logger)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            if (serverGroups == null) throw new ArgumentNullException(nameof(serverGroups));
            if (logsHistory == null) throw new ArgumentNullException(nameof(logsHistory));
            if (wurmServers == null) throw new ArgumentNullException(nameof(wurmServers));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.character = character;
            this.logsHistory = logsHistory;
            this.logger = logger;
        }

        public async Task<IList<LogEntry>> ScanLogsAsync(DateTime minDate, DateTime maxDate, LogType logType,
            ScanResultOrdering scanResultOrdering)
        {
            return await logsHistory.ScanAsync(new LogSearchParameters()
            {
                CharacterName = character.Name.Normalized,
                LogType = logType,
                MinDate = minDate,
                MaxDate = maxDate,
                ScanResultOrdering = scanResultOrdering
            }).ConfigureAwait(false);
        }

        public async Task<IList<LogEntry>> ScanLogsServerGroupRestrictedAsync(DateTime minDate, DateTime maxDate, LogType logType,
            ServerGroup serverGroup, ScanResultOrdering scanResultOrdering)
        {
            var results = await logsHistory.ScanAsync(new LogSearchParameters()
            {
                CharacterName = character.Name.Normalized,
                LogType = logType,
                MinDate = minDate,
                MaxDate = maxDate,
                ScanResultOrdering = scanResultOrdering
            }).ConfigureAwait(false);

            List<LogEntry> filteredEntries = new List<LogEntry>();
            foreach (var logEntry in results)
            {
                var serverForEntry = await TryGetServerAtStamp(logEntry.Timestamp).ConfigureAwait(false);
                if (serverForEntry != null && serverForEntry.ServerGroup == serverGroup)
                {
                    filteredEntries.Add(logEntry);
                }
            }
            return filteredEntries;
        }

        async Task<IWurmServer> TryGetServerAtStamp(DateTime dateTime)
        {
            var result = await character.TryGetHistoricServerAtLogStampAsync(dateTime).ConfigureAwait(false);
            if (result == null)
            {
                logger.Log(LogLevel.Info,
                    string.Format("Server could not be identified for character {0} at stamp {1}",
                        character.Name.Capitalized,
                        dateTime),
                    this,
                    null);
                return null;
            }
            return result;
        }
    }
}