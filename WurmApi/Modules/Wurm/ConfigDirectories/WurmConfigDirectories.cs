using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.ConfigDirectories
{
    /// <summary>
    /// Manages directory information about wurm configs
    /// </summary>
    class WurmConfigDirectories : WurmSubdirsMonitor, IWurmConfigDirectories
    {
        readonly IInternalEventAggregator eventAggregator;

        public WurmConfigDirectories(IWurmPaths wurmPaths, [NotNull] IInternalEventAggregator eventAggregator,
            TaskManager taskManager, IWurmApiLogger logger)
            : base(
                wurmPaths.ConfigsDirFullPath,
                taskManager,
                () => eventAggregator.Send(new ConfigDirectoriesChanged()),
                logger,
                ValidateDirectory,
                wurmPaths)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            this.eventAggregator = eventAggregator;
        }

        static void ValidateDirectory(string directoryFullPath, IWurmPaths wurmPaths)
        {
            // no validation required here
        }

        public IEnumerable<string> AllConfigNames => AllDirectoryNamesNormalized;

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
                        DirectoryFullPath));
            }
            return file.FullName;
        }
    }
}
