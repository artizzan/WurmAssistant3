using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Describes a single in-game event as logged by the game client.
    /// </summary>
    public class LogEntry
    {
        public LogEntry(DateTime timestamp, [NotNull] string source, [NotNull] string content,
            [CanBeNull] string pmConversationRecipient = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (content == null) throw new ArgumentNullException(nameof(content));
            Timestamp = timestamp;
            Source = source;
            Content = content;
            PmConversationRecipient = pmConversationRecipient != null
                ? new CharacterName(pmConversationRecipient)
                : CharacterName.Empty;
        }

        /// <summary>
        /// Timestamp of this log line or DateTime.Min if not parseable.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets the source of this log entry (the text inside brackets, for example chat names).
        /// String.Empty if not available.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Gets the log entry full content.
        /// String.Empty if not available.
        /// </summary>
        public string Content { get; private set; }

        public override string ToString()
        {
            return string.Format("Timestamp: {0}, Source: {1}, Content: {2}", Timestamp, Source, Content);
        }

        /// <summary>
        /// Contains name of the PM recipient of a PM conversation, if LogEntry describes PM Logs
        /// </summary>
        public CharacterName PmConversationRecipient { get; private set; }
    }
}