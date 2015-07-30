namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel
{
    public class WurmLogMonthlyFileHeuristics
    {
        public int DayOfMonth { get; set; }
        public long FilePositionInBytes { get; set; }
        public int LinesCount { get; set; }
    }
}
