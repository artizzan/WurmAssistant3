using System;

namespace AldurSoft.WurmApi
{
    public class LogSearchParameters
    {
        /// <summary>
        /// Name of the character, whose logs are to be searched. Case insensitive. Null value is invalid.
        /// </summary>
        public CharacterName CharacterName { get; set; }
        /// <summary>
        /// General type of logs to search through. Currently, Unspecified types cannot be searched (each type has to be searched separately).
        /// </summary>
        public LogType LogType { get; set; }
        /// <summary>
        /// Lower search time boundary. Cannot be later than <see cref="DateTo"/>. Only date part is significant.
        /// </summary>
        public DateTime DateFrom { get; set; }
        /// <summary>
        /// Upper search time boundary. Cannot be earlier than <see cref="DateFrom"/>. Only date part is significant.
        /// </summary>
        public DateTime DateTo { get; set; }
        /// <summary>
        /// Set this only if searching <see cref="LogType"/> of type PM. 
        /// If set, only results matching conversations with this character will be returned.
        /// </summary>
        public CharacterName PmCharacterName { get; set; }

        public override string ToString()
        {
            return string.Format(
                "CharacterName: {0}, LogType: {1}, DateFrom: {2}, DateTo: {3}, PmCharacterName: {4}",
                CharacterName,
                LogType,
                DateFrom,
                DateTo,
                PmCharacterName);
        }

        public virtual void AssertAreValid()
        {
            if (CharacterName == null)
            {
                throw new InvalidSearchParametersException("characterName cannot be null");
            }
            if (DateTo < DateFrom)
            {
                throw new InvalidSearchParametersException("DateTo cannot be smaller than DateFrom");
            }
            if (LogType == LogType.Unspecified)
            {
                throw new InvalidSearchParametersException("Unspecified log type search is not supported");
            }
            if (LogType != LogType.Pm && PmCharacterName != null)
            {
                throw new InvalidSearchParametersException(
                    string.Format(
                        "PmCharacterName was provided in search parameters, but search log type is not PM, search params: {0}",
                        ToString()));
            }
        }
    }
}