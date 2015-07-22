using System;
using System.Collections.Generic;
using AldurSoft.WurmApi.Persistence.DataModel.LogsHistoryModel;

namespace AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule.Heuristics
{
    public class HeuristicsExtractionResult
    {
        public DateTime LogDate { get; set; }
        public Dictionary<int, WurmLogMonthlyFileHeuristics> Heuristics { get; set; }
    }
}