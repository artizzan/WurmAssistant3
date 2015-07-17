namespace AldurSoft.WurmApi.Impl.WurmLogsHistoryImpl.Heuristics
{
    public struct DayInfo
    {
        private readonly long startPositionInBytes;
        private readonly int linesLength;

        public DayInfo(long startPositionInBytes, int linesLength)
        {
            this.startPositionInBytes = startPositionInBytes;
            this.linesLength = linesLength;
        }

        public long StartPositionInBytes
        {
            get { return startPositionInBytes; }
        }

        public int LinesLength
        {
            get { return linesLength; }
        }
    }
}