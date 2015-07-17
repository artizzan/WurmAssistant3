using System;
using System.Collections.Generic;

using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.DataModel.LogsHistoryModel;

namespace AldurSoft.WurmApi.Impl.WurmLogsHistoryImpl.Heuristics
{
    public class MonthlyLogFileHeuristicsFactory
    {
        private readonly IWurmApiDataContext dataContext;
        private readonly IWurmLogFiles wurmLogFiles;
        private readonly MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory;

        private readonly Dictionary<CharacterName, CharacterMonthlyLogHeuristics> cache =
            new Dictionary<CharacterName, CharacterMonthlyLogHeuristics>();

        public MonthlyLogFileHeuristicsFactory(
            IWurmApiDataContext dataContext,
            IWurmLogFiles wurmLogFiles,
            MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory)
        {
            if (dataContext == null) throw new ArgumentNullException("dataContext");
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            if (monthlyHeuristicsExtractorFactory == null) throw new ArgumentNullException("monthlyHeuristicsExtractorFactory");
            this.dataContext = dataContext;
            this.wurmLogFiles = wurmLogFiles;
            this.monthlyHeuristicsExtractorFactory = monthlyHeuristicsExtractorFactory;
        }

        public virtual CharacterMonthlyLogHeuristics Create(CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");

            CharacterMonthlyLogHeuristics heuristics;
            if (!cache.TryGetValue(characterName, out heuristics))
            {
                IPersistent<WurmCharacterLogsEntity> repository =
                    dataContext.WurmCharacterLogs.Get(new EntityKey(characterName.Normalized));
                heuristics = new CharacterMonthlyLogHeuristics(
                    repository,
                    monthlyHeuristicsExtractorFactory,
                    wurmLogFiles.GetManagerForCharacter(characterName));
                cache.Add(characterName, heuristics);
            }
            return heuristics;
        }
    }
}
