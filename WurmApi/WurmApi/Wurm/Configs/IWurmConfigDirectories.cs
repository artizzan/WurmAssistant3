using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi.Wurm.Configs
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
        /// <exception cref="WurmApiException">Directory not found</exception>
        string GetGameSettingsFileFullPathForConfigName(string directoryName);
    }
}