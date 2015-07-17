using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    public interface IWurmLogDefinitions
    {
        IEnumerable<LogDefinition> AllDefinitions { get; }

        IEnumerable<LogType> AllLogTypes { get; }

        LogType TryGetTypeFromLogFileName(string filename);
    }
}