using System;
using System.Collections.Generic;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    class MonthlyLogFilesHeuristics
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
                throw new ArgumentNullException(nameof(heuristicsPersistentCollection));
            if (wurmLogFiles == null) throw new ArgumentNullException(nameof(wurmLogFiles));
            if (monthlyHeuristicsExtractorFactory == null) throw new ArgumentNullException(nameof(monthlyHeuristicsExtractorFactory));

            this.heuristicsPersistentCollection = heuristicsPersistentCollection;
            this.wurmLogFiles = wurmLogFiles;
            this.monthlyHeuristicsExtractorFactory = monthlyHeuristicsExtractorFactory;
        }

        public virtual CharacterMonthlyLogHeuristics GetForCharacter(CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));

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
