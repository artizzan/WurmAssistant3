namespace AldurSoft.WurmApi.Modules.DataContext.DataModel.LogsHistoryModel
{
    public class WurmLogMonthlyFileHeuristics
    {
        public int DayOfMonth { get; set; }
        public long FilePositionInBytes { get; set; }
        public int LinesCount { get; set; }
    }
}
