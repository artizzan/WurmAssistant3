using System;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Describes a single in-game event as logged by the game client.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Timestamp of this log line or DateTime.Min if not parseable.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets the source of this log entry (the text inside brackets, for example chat names).
        /// Null if not available.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets the log entry full content.
        /// Null if not available.
        /// </summary>
        public string Content { get; set; }

        public override string ToString()
        {
            return string.Format("Timestamp: {0}, Source: {1}, Content: {2}", Timestamp, Source, Content);
        }
    }
}