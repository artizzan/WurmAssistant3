using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule.Heuristics
{
    public class MonthlyFileHeuristics
    {
        public MonthlyFileHeuristics(DateTime logDate, Dictionary<int, DayInfo> heuristics)
        {
            this.heuristics = heuristics;
            LogDate = logDate;
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
    }
}