using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    public interface IWurmLogDefinitions
    {
        IEnumerable<LogDefinition> AllDefinitions { get; }

        IEnumerable<LogType> AllLogTypes { get; }

        LogType TryGetTypeFromLogFileName(string logFileName);
    }
}