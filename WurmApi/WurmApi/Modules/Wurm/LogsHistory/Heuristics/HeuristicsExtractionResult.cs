using System;
using System.Collections.Generic;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    public class HeuristicsExtractionResult
    {
        public DateTime LogDate { get; set; }
        public Dictionary<int, WurmLogMonthlyFileHeuristics> Heuristics { get; set; }
    }
}