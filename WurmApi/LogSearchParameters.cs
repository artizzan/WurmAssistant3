using System;

namespace AldursLab.WurmApi
{
    public class LogSearchParameters
    {
        /// <summary>
        /// Name of the character, whose logs are to be searched. Case insensitive. Null value is invalid.
        /// Names are case insensitive.
        /// </summary>
        public string CharacterName { get; set; }
        /// <summary>
        /// General type of logs to search through. Currently, Unspecified types cannot be searched (each type has to be searched separately).
        /// </summary>
        public LogType LogType { get; set; }
        /// <summary>
        /// Lower search time boundary. Cannot be later than <see cref="MaxDate"/>. Only date part is significant.
        /// </summary>
        public DateTime MinDate { get; set; }
        /// <summary>
        /// Upper search time boundary. Cannot be earlier than <see cref="MinDate"/>. Only date part is significant.
        /// </summary>
        public DateTime MaxDate { get; set; }
        /// <summary>
        /// Set this only if searching <see cref="LogType"/> of type PM. 
        /// If set, only results matching conversations with this character will be returned.
        /// Names are case insensitive.
        /// </summary>
        public string PmRecipientName { get; set; }
        /// <summary>
        /// Defines the order, in which result LogEntry are returned.
        /// Default order is Ascending (oldest first).
        /// </summary>
        public ScanResultOrdering ScanResultOrdering { get; set; }

        public override string ToString()
        {
            return string.Format(
                "CharacterName: {0}, LogType: {1}, DateFrom: {2}, DateTo: {3}, PmCharacterName: {4}",
                CharacterName,
                LogType,
                MinDate,
                MaxDate,
                PmRecipientName);
        }

        public virtual void AssertAreValid()
        {
            if (CharacterName == null)
            {
                throw new InvalidSearchParametersException("characterName cannot be null");
            }
            if (MaxDate < MinDate)
            {
                throw new InvalidSearchParametersException("DateTo cannot be smaller than DateFrom");
            }
            if (LogType == LogType.Unspecified)
            {
                throw new InvalidSearchParametersException("Unspecified log type search is not supported");
            }
            if (LogType != LogType.Pm && PmRecipientName != null)
            {
                throw new InvalidSearchParametersException(
                    string.Format(
                        "PmCharacterName was provided in search parameters, but search log type is not PM, search params: {0}",
                        ToString()));
            }
        }
    }

    /// <summary>
    /// Defines order of the result LogEntries.
    /// </summary>
    public enum ScanResultOrdering
    {
        /// <summary>
        /// First result entry is the oldest one.
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// First result entry is the newest one.
        /// </summary>
        Descending
    }
}