using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.LogSearcher.Model;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.LogSearcher.ViewModels
{
    public class LogSearcher
    {
        readonly IWurmApi wurmApi;

        public LogSearcher([NotNull] IWurmApi wurmApi)
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
}
