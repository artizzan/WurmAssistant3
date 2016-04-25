namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    public struct DayInfo
    {
        public DayInfo(long startPositionInBytes, int linesLength, int totalLinesSinceBeginFile)
        {
            StartPositionInBytes = startPositionInBytes;
            LinesLength = linesLength;
            TotalLinesSinceBeginFile = totalLinesSinceBeginFile;
        }

        public long StartPositionInBytes { get; }

        public int LinesLength { get; }

        public int TotalLinesSinceBeginFile { get; }
    }
}