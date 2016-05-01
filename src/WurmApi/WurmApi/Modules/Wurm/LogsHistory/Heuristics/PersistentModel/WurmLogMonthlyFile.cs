using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel
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
        public bool HasValidBytePositions { get; set; }
    }
}
