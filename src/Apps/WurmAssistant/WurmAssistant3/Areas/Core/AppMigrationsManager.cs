using System;
using System.IO;
using System.Text;
using AldursLab.Essentials.FileSystem;
using AldursLab.WurmAssistant3.Areas.Core.Data;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    [KernelBind(BindingHint.Singleton)]
    class AppMigrationsManager : IAppMigrationsManager
    {
        readonly IKernel kernel;
        readonly IWurmAssistantDataDirectory dataDirectory;
        readonly PersistentContextsManager persistentContextsManager;
        readonly ILogger logger;
        readonly ITrayPopups trayPopups;

        const int LatestVersion = 1;
        int currentVersion;

        readonly FileInfo upgradeInfoFile;
        readonly string backupDirRootPath;

        public AppMigrationsManager(
            [NotNull] IKernel kernel,
            [NotNull] IWurmAssistantDataDirectory dataDirectory,
            [NotNull] PersistentContextsManager persistentContextsManager,
            [NotNull] ILogger logger,
            [NotNull] ITrayPopups trayPopups)
        {
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (dataDirectory == null) throw new ArgumentNullException(nameof(dataDirectory));
            if (persistentContextsManager == null) throw new ArgumentNullException(nameof(persistentContextsManager));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            this.kernel = kernel;
            this.dataDirectory = dataDirectory;
            this.persistentContextsManager = persistentContextsManager;
            this.logger = logger;
            this.trayPopups = trayPopups;

            backupDirRootPath = Path.Combine(dataDirectory.DirectoryPath, "Backup");

            upgradeInfoFile = new FileInfo(Path.Combine(dataDirectory.DirectoryPath, "upgrade.dat"));
            LoadCurrentVersion();
        }

        public void RunMigrations()
        {
            if (currentVersion == 0)
            {
                trayPopups.Schedule("Upgrading some Wurm Assistant data", "Some Tricky Business");

                logger.Info($"Wurm Assistant data needs to be upgraded, performing upgrade from data version {currentVersion}");
                logger.Info($"Creating backup of the current data at {backupDirRootPath}");
                CreateDataBackup();
                logger.Info("Backup created");
                logger.Info("Beginning upgrade...");
                var migration = kernel.Get<TriggersV2Migration>();
                migration.Run();
                logger.Info("Migration completed");
                currentVersion = 1;
                persistentContextsManager.PerformAutoSave(saveAll: true);
                logger.Info("All data saved");
                SaveCurrentVersion();
                logger.Info($"Wurm Assistant is now upgraded to data version {currentVersion}");

                trayPopups.Schedule("Data upgrade completed!", "Tricky Business Done");
            }
            else
            {
                logger.Info($"Wurm Assistant data is at version {currentVersion}. No upgrade required.");
            }
        }

        void CreateDataBackup()
        {
            var backupTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var currentBackupRootPath = Path.Combine(backupDirRootPath, backupTimeStamp);
            {
                var dataDir = Path.Combine(dataDirectory.DirectoryPath, "Data");
                var backupDir = Path.Combine(currentBackupRootPath, "Data");
                DirectoryOps.CopyRecursively(dataDir, backupDir);
            }
            {
                var dataDir = Path.Combine(dataDirectory.DirectoryPath, "DataV2");
                var backupDir = Path.Combine(currentBackupRootPath, "DataV2");
                DirectoryOps.CopyRecursively(dataDir, backupDir);
            }

            trayPopups.Schedule($"Created backup of current data at {currentBackupRootPath}", "Tricky Business", 5000);
        }

        void LoadCurrentVersion()
        {
            UpgradeStatus upgradeStatus;

            if (!upgradeInfoFile.Exists)
            {
                upgradeStatus = new UpgradeStatus()
                {
                    CurrentUpgradeVersion = LatestVersion
                };
                var contents = JsonConvert.SerializeObject(upgradeStatus);
                File.WriteAllText(upgradeInfoFile.FullName, contents, Encoding.UTF8);
            }
            else
            {
                upgradeStatus = JsonConvert.DeserializeObject<UpgradeStatus>(File.ReadAllText(upgradeInfoFile.FullName, Encoding.UTF8));
            }

            currentVersion = upgradeStatus.CurrentUpgradeVersion;
        }

        void SaveCurrentVersion()
        {
            var upgradeStatus = new UpgradeStatus {CurrentUpgradeVersion = currentVersion};
            var contents = JsonConvert.SerializeObject(upgradeStatus);
            File.WriteAllText(upgradeInfoFile.FullName, contents, Encoding.UTF8);
        }

        class UpgradeStatus
        {
            public int CurrentUpgradeVersion { get; set; }
        }
    }
}
