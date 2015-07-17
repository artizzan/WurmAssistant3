namespace AldurSoft.WurmApi.Impl.WurmLogsHistoryImpl.Heuristics
{
    interface IMonthlyHeuristicsDataBuilder
    {
        void ProcessLine(string line, long lastReadLineStartPosition);

        void Complete(long finalPositionInLogFile);

        HeuristicsExtractionResult GetResult();
    }
}