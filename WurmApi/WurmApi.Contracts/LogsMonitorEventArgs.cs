using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    public class LogsMonitorEventArgs : EventArgs
    {
        public LogsMonitorEventArgs(CharacterName characterName, LogType logType, IEnumerable<LogEntry> wurmLogEntries, string conversationNameNormalized = null)
        {
            if (characterName == null)
            {
                throw new ArgumentNullException("characterName");
            }
            if (wurmLogEntries == null)
            {
                throw new ArgumentNullException("wurmLogEntries");
            }
            if (conversationNameNormalized == null) conversationNameNormalized = string.Empty;

            this.CharacterName = characterName;
            this.LogType = logType;
            this.WurmLogEntries = wurmLogEntries;
            this.ConversationNameNormalized = conversationNameNormalized;
        }

        public CharacterName CharacterName { get; private set; }
        public LogType LogType { get; private set; }
        public IEnumerable<LogEntry> WurmLogEntries { get; private set; }

        /// <summary>
        /// Name of the wurm character, that this conversation is with.
        /// Only applicable to Pm logs.
        /// </summary>
        public string ConversationNameNormalized { get; private set; }
    }
}