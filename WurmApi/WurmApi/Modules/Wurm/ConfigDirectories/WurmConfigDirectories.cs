using System;
using System.IO;
using System.Linq;
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

        public WurmConfigDirectories(
            IWurmPaths wurmPaths, IPublicEventInvoker eventInvoker, [NotNull] IInternalEventAggregator eventAggregator)
            : base(wurmPaths.ConfigsDirFullPath, eventInvoker)
        {
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            this.eventAggregator = eventAggregator;
        }

        public string GetGameSettingsFileFullPathForConfigName(string directoryName)
        {
            var dirPath = GetFullPathForDirName(directoryName);
            if (dirPath == null)
            {
                throw new DataNotFoundException(
                    string.Format(
                        "Directory full path not found for name: {0} ; Dir monitor for: {1}",
                        directoryName,
                        this.DirectoryFullPath));
            }
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

        protected override void OnDirectoriesChanged()
        {
            eventAggregator.Send(new ConfigDirectoriesChanged());
        }
    }
}
