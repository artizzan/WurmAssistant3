using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    public class MonthlyLogFilesHeuristics
    {
        readonly IPersistentCollection heuristicsPersistentCollection;
        readonly IWurmLogFiles wurmLogFiles;
        readonly MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory;

        readonly Dictionary<CharacterName, CharacterMonthlyLogHeuristics> cache =
            new Dictionary<CharacterName, CharacterMonthlyLogHeuristics>();

        public MonthlyLogFilesHeuristics([NotNull] IPersistentCollection heuristicsPersistentCollection,
            IWurmLogFiles wurmLogFiles,
            MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory)
        {
            if (heuristicsPersistentCollection == null)
                throw new ArgumentNullException("heuristicsPersistentCollection");
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            if (monthlyHeuristicsExtractorFactory == null) throw new ArgumentNullException("monthlyHeuristicsExtractorFactory");

            this.heuristicsPersistentCollection = heuristicsPersistentCollection;
            this.wurmLogFiles = wurmLogFiles;
            this.monthlyHeuristicsExtractorFactory = monthlyHeuristicsExtractorFactory;
        }

        public virtual CharacterMonthlyLogHeuristics GetForCharacter(CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");

            CharacterMonthlyLogHeuristics heuristics;
            if (!cache.TryGetValue(characterName, out heuristics))
            {
                IPersistent<WurmCharacterLogsEntity> persistentData =
                    heuristicsPersistentCollection.GetObject<WurmCharacterLogsEntity>(characterName.Normalized);
                heuristics = new CharacterMonthlyLogHeuristics(
                    persistentData,
                    monthlyHeuristicsExtractorFactory,
                    wurmLogFiles.GetForCharacter(characterName));
                cache.Add(characterName, heuristics);
            }
            return heuristics;
        }
    }
}
