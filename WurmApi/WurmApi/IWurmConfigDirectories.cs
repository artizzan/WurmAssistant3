using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    public interface IWurmConfigDirectories
    {
        IEnumerable<string> AllDirectoryNamesNormalized { get; }

        IEnumerable<string> AllDirectoriesFullPaths { get; }

        event EventHandler DirectoriesChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        string GetGameSettingsFileFullPathForConfigName(string directoryName);
    }
}