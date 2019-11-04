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
            const string gameSettingsTxt = "gamesettings.txt";

            var dirPath = GetFullPathForDirName(directoryName);
            var configDirectoryInfo = new DirectoryInfo(dirPath);
            var file = new FileInfo(Path.Combine(configDirectoryInfo.FullName, gameSettingsTxt));
            if (!file.Exists)
            {
                throw new DataNotFoundException(
                    $"{gameSettingsTxt} not found at path {file.FullName} ; "
                    + $"Config for: {directoryName} ; Dir monitor for: {DirectoryFullPath}");
            }
            return file.FullName;
        }
    }
}
