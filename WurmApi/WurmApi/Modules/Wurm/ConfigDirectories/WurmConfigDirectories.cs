using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories
{
    /// <summary>
    /// Manages directory information about wurm configs
    /// </summary>
    class WurmConfigDirectories : WurmSubdirsMonitor, IWurmConfigDirectories
    {
        readonly IInternalEventAggregator eventAggregator;

        public WurmConfigDirectories(IWurmPaths wurmPaths, [NotNull] IInternalEventAggregator eventAggregator, TaskManager taskManager)
            : base(wurmPaths.ConfigsDirFullPath, taskManager, () => eventAggregator.Send(new ConfigDirectoriesChanged()))
        {
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            this.eventAggregator = eventAggregator;
        }

        public IEnumerable<string> AllConfigNames
        {
            get { return AllDirectoryNamesNormalized; }
        }

        public string GetGameSettingsFileFullPathForConfigName(string directoryName)
        {
            var dirPath = GetFullPathForDirName(directoryName);
            var configDirectoryInfo = new DirectoryInfo(dirPath);
            var file = configDirectoryInfo.GetFiles("gamesettings.txt").FirstOrDefault();
            if (file == null)
            {
                throw new DataNotFoundException(
                    string.Format(
                        "gamesettings.txt full path not found for name: {0} ; Dir monitor for: {1}",
                        directoryName,
                        this.DirectoryFullPath));
            }
            return file.FullName;
        }
    }
}
