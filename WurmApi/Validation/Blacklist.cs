using System;
using System.Collections.Generic;
using AldursLab.WurmApi.Extensions.DotNet.Collections.Generic;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Validation
{
    class Blacklist<TKey>
    {
        readonly IWurmApiLogger logger;
        readonly string description;

        readonly int issueTreshhold;

        readonly Dictionary<TKey, int> issueCounts = new Dictionary<TKey, int>();

        readonly object locker = new object();

        public Blacklist([NotNull] IWurmApiLogger logger, [NotNull] string description, int issueTreshhold = 10)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (description == null) throw new ArgumentNullException(nameof(description));
            this.logger = logger;
            this.description = description;
            this.issueTreshhold = issueTreshhold;
        }

        public bool IsOnBlacklist(TKey key)
        {
            lock (locker)
            {
                var count = issueCounts.GetOrAdd(key, () => 0);
                return count == issueTreshhold;
            }
        }

        public void ReportIssue(TKey key)
        {
            int count;
            lock (locker)
            {
                count = issueCounts.GetOrAdd(key, () => 0);
                count++;
                issueCounts[key] = count;
            }
            if (count >= issueTreshhold)
            {
                logger.Log(LogLevel.Warn,
                    string.Format("{1} > Adding key {0} to blacklist because at least {2} issues were reported. " +
                                  "List will be reset after application restart.",
                        key,
                        description,
                        count),
                    this,
                    null);
            }
        }
    }
}
