using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi.DataModel.LogsHistoryModel
{
    public class WurmLogMonthlyFile
    {
        public WurmLogMonthlyFile()
        {
            DayToHeuristicsMap = new Dictionary<int, WurmLogMonthlyFileHeuristics>();
        }

        public string FileName { get; set; }
        public long LastKnownSizeInBytes { get; set; }
        public DateTime LogDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public Dictionary<int, WurmLogMonthlyFileHeuristics> DayToHeuristicsMap { get; set; }
    }
}
