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
                result = TryGetMatchResult(TryMatch1(logEntry), logEntry);
                if (result == null)
                {
                    result = TryGetMatchResult(TryMatch2(logEntry), logEntry);
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

        private static Match TryMatch1(LogEntry logEntry)
        {
            return Regex.Match(
                    logEntry.Content,
                    @"\d+ other players are online.*\. You are on (.+) \(",
                    RegexOptions.Compiled);
        }

        private static Match TryMatch2(LogEntry logEntry)
        {
            return Regex.Match(
                    logEntry.Content,
                    @"No other players are online on (.+) \(",
                    RegexOptions.Compiled);
        }

        private static ServerStamp TryGetMatchResult(Match match, LogEntry logEntry)
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
