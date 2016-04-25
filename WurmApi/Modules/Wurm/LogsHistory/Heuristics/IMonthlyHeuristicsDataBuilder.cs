namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    interface IMonthlyHeuristicsDataBuilder
    {
        void ProcessLine(string line, long lastReadLineStartPosition);

        void Complete(long finalPositionInLogFile);

        HeuristicsExtractionResult GetResult();
    }
}