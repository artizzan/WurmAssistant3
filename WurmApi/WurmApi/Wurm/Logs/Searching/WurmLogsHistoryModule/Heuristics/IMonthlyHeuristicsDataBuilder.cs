namespace AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule.Heuristics
{
    interface IMonthlyHeuristicsDataBuilder
    {
        void ProcessLine(string line, long lastReadLineStartPosition);

        void Complete(long finalPositionInLogFile);

        HeuristicsExtractionResult GetResult();
    }
}