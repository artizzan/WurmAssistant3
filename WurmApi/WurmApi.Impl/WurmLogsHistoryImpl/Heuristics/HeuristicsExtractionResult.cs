using System;
using System.Collections.Generic;

using AldurSoft.WurmApi.DataModel.LogsHistoryModel;

namespace AldurSoft.WurmApi.Impl.WurmLogsHistoryImpl.Heuristics
{
    public class HeuristicsExtractionResult
    {
        public DateTime LogDate { get; set; }
        public Dictionary<int, WurmLogMonthlyFileHeuristics> Heuristics { get; set; }
    }
}