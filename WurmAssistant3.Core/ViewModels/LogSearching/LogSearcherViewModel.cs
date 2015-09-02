using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.ViewModels.LogSearching
{
    public class LogSearcherViewModel
    {
        readonly IWurmApi wurmApi;

        public LogSearcherViewModel([NotNull] IWurmApi wurmApi)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.wurmApi = wurmApi;
        }

        public IEnumerable<IWurmCharacter> Characters { get { return wurmApi.Characters.All; } }
        public IEnumerable<LogType> LogTypes { get { return wurmApi.LogDefinitions.AllLogTypes; } }

        public IEnumerable<SearchTypeId> SearchTypes
        {
            get { return new[] {SearchTypeId.RegexEscapedCaseIns, SearchTypeId.RegexCustom}; }
        }

        public async Task<IEnumerable<LogEntry>> Search([NotNull] LogSearchParameters searchParams,
            CancellationToken cancellationToken)
        {
            if (searchParams == null) throw new ArgumentNullException("searchParams");
            var result = await wurmApi.LogsHistory.ScanAsync(searchParams, cancellationToken);

            return result;
        }
    }

    public enum SearchTypeId
    {
        Unspecified = 0,
        RegexEscapedCaseIns, 
        RegexCustom
    }

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
