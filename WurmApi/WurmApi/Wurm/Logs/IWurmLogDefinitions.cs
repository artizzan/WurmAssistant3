using System.Collections.Generic;

namespace AldurSoft.WurmApi.Wurm.Logs
{
    public interface IWurmLogDefinitions
    {
        IEnumerable<LogDefinition> AllDefinitions { get; }

        IEnumerable<LogType> AllLogTypes { get; }

        LogType TryGetTypeFromLogFileName(string filename);
    }
}