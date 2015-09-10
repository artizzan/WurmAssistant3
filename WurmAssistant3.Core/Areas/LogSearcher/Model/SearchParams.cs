using System;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.LogSearcher.Model
{
    public class SearchParams
    {
        public string Character { get; set; }
        public LogType LogType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        [CanBeNull]
        public string SearchPhrase { get; set; }
        public SearchTypeId SearchTypeId { get; set; }
        [CanBeNull]
        public string PmCharacter { get; set; }

        public void Validate()
        {
            if (Character == null) throw new ArgumentException("Character cannot be null");
            if (LogType == LogType.Unspecified)
                throw new ArgumentException("LogType cannot be Unspecified");
            if (SearchTypeId == SearchTypeId.Unspecified)
                throw new ArgumentException("SearchType cannot be Unspecified");
            if (Character == null) throw new ArgumentException("Character cannot be null");
            if (PmCharacter != null && LogType != LogType.Pm)
                throw new ArgumentException("If PmCharacter is set, log type must be PM");
        }
    }
}