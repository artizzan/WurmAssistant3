using System.Text.RegularExpressions;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Contains extensions enabling parsing common information from log entries.
    /// </summary>
    public static class LogEntryParsingHelper
    {
        /// <summary>
        /// Attempts to parse server name from a log entry.
        /// Null if parsing failed.
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="logger">Optional, will log parsing errors.</param>
        /// <param name="sourceCharacterName">Optional, will be appended to log message.</param>
        /// <returns></returns>
        public static ServerStamp TryGetServerFromLogEntry(this LogEntry logEntry, IWurmApiLogger logger = null, CharacterName sourceCharacterName = null)
        {
            ServerStamp result = null;
            // attempt some faster matchers first, before trying actual parse
            if (Regex.IsMatch(logEntry.Content, @"other players are online", RegexOptions.Compiled))
            {
                result = TryMatch1(logEntry);
                if (result == null)
                {
                    result = TryMatch2(logEntry);
                }
                if (result == null)
                {
                    result = TryMatch3(logEntry);
                }
                if (result == null)
                {
                    logger?.Log(
                        LogLevel.Warn,
                        string.Format(
                            "ServerHistoryProvider found 'other players are online' log line, but could not parse it. Character: {0} Entry: {1}",
                            sourceCharacterName,
                            logEntry),
                        "ServerParsingHelper",
                        null);
                }
            }
            return result;
        }

        static ServerStamp TryMatch1(LogEntry logEntry)
        {
            return TryGetMatchResult(Regex.Match(
                    logEntry.Content,
                    @"\d+ other players are online.*\. You are on (.+) \(",
                    RegexOptions.Compiled), logEntry);
        }

        static ServerStamp TryMatch2(LogEntry logEntry)
        {
            return TryGetMatchResult(Regex.Match(
                    logEntry.Content,
                    @"No other players are online on (.+) \(",
                    RegexOptions.Compiled), logEntry);
        }

        static ServerStamp TryMatch3(LogEntry logEntry)
        {
            // [20:43:08] No other players are online taking tutorials (242 totally in Wurm).
            // [22:29:29] 1 other players are online on this server (857 totally in Wurm).
            var match = Regex.Match(
                    logEntry.Content,
                    @"(?!taking tutorials \()||(?!players are online on this)",
                    RegexOptions.Compiled);

            if (match.Success)
            {
                return new ServerStamp() {ServerName = new ServerName(Constants.ServerNames.GoldenValley), Timestamp = logEntry.Timestamp};
            }

            return null;
        }

        static ServerStamp TryGetMatchResult(Match match, LogEntry logEntry)
        {
            ServerStamp result = null;
            if (match.Success)
            {
                var serverName = new ServerName(match.Groups[1].Value.ToUpperInvariant());
                result = new ServerStamp() { ServerName = serverName, Timestamp = logEntry.Timestamp };
            }
            return result;
        }
    }
}
