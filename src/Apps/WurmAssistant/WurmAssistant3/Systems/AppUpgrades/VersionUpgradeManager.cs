using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Core;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Systems.AppUpgrades
{
    /// <summary>
    /// Performs before-binding upgrades for the entire application.
    /// </summary>
    class VersionUpgradeManager
    {
        readonly IWurmAssistantDataDirectory dataDirectory;
        readonly IConsoleArgs consoleArgs;

        const int LatestVersion = 0;

        public VersionUpgradeManager([NotNull] IWurmAssistantDataDirectory dataDirectory,
            [NotNull] IConsoleArgs consoleArgs)
        {
            if (dataDirectory == null) throw new ArgumentNullException(nameof(dataDirectory));
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            this.dataDirectory = dataDirectory;
            this.consoleArgs = consoleArgs;
        }

        public void RunUpgrades()
        {
            var infoFile = new FileInfo(Path.Combine(dataDirectory.DirectoryPath, "upgrade.dat"));
            if (!infoFile.Exists)
            {
                var contents = JsonConvert.SerializeObject(new UpgradeStatus()
                {
                    CurrentUpgradeVersion = LatestVersion
                });
                File.WriteAllText(infoFile.FullName, contents, Encoding.UTF8);
            }
            else
            {
                var upgradeStatus = JsonConvert.DeserializeObject<UpgradeStatus>(File.ReadAllText(infoFile.FullName, Encoding.UTF8));
                //if (upgradeStatus.UpgradeVersion == 0)
                //{
                //    // do upgrade

                //    upgradeStatus.UpgradeVersion = LatestVersion;
                //    var contents = JsonConvert.SerializeObject(upgradeStatus);
                //    File.WriteAllText(infoFile.FullName, contents, Encoding.UTF8);
                //}
            }
        }
    }

    class UpgradeStatus
    {
        public int CurrentUpgradeVersion { get; set; }
    }
}
