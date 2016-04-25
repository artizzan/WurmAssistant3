using System;
using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    public class MonthlyFileHeuristics
    {
        public MonthlyFileHeuristics(DateTime logDate, Dictionary<int, DayInfo> heuristics, bool hasValidFilePositions)
        {
            this.heuristics = heuristics;
            LogDate = logDate;
            HasValidFilePositions = hasValidFilePositions;
        }

        private readonly Dictionary<int, DayInfo> heuristics;
        public DateTime LogDate { get; private set; }

        public DayInfo GetForDay(int day)
        {
            DayInfo result;
            if (heuristics.TryGetValue(day, out result))
            {
                return result;
            }
            else if (heuristics.Any())
            {
                result = heuristics[heuristics.Keys.Max()];
                return result;
            }
            else
            {
                throw new WurmApiException(
                    "No heuristics available for this day. This should not have happened! Abandon ship!");
            }
        }

        public bool HasValidFilePositions { get; private set; }
    }
}