using System;
using System.Collections.Generic;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    public class HeuristicsExtractionResult
    {
        public DateTime LogDate { get; set; }
        public Dictionary<int, WurmLogMonthlyFileHeuristics> Heuristics { get; set; }

        public bool HasValidBytePositions { get; set; }
    }
}