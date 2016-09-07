using System;
using System.IO;
using System.Text;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer.Migrations;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations;
using AldursLab.WurmAssistant3.Systems.DataBackups;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Systems.AppMigrations
{
    [KernelBind(BindingHint.Singleton)]
    class AppMigrationsManager : IAppMigrationsManager
    {
        readonly IKernel kernel;
        readonly PersistentContextsManager persistentContextsManager;
        readonly ILogger logger;
        readonly ITrayPopups trayPopups;

        const int LatestVersion = 2;
        int currentVersion;

        readonly FileInfo upgradeInfoFile;

        readonly BackupManager backupManager;

        public AppMigrationsManager(
            [NotNull] IKernel kernel,
            [NotNull] IWurmAssistantDataDirectory dataDirectory,
            [NotNull] PersistentContextsManager persistentContextsManager,
            [NotNull] ILogger logger,
            [NotNull] ITrayPopups trayPopups,
            [NotNull] BackupManager backupManager)
        {
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (dataDirectory == null) throw new ArgumentNullException(nameof(dataDirectory));
            if (persistentContextsManager == null) throw new ArgumentNullException(nameof(persistentContextsManager));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (backupManager == null) throw new ArgumentNullException(nameof(backupManager));
            this.kernel = kernel;
            this.persistentContextsManager = persistentContextsManager;
            this.logger = logger;
            this.trayPopups = trayPopups;
            this.backupManager = backupManager;

            upgradeInfoFile = new FileInfo(Path.Combine(dataDirectory.DirectoryPath, "upgrade.dat"));
            LoadCurrentVersion();
        }

        public void RunMigrations()
        {
            if (currentVersion == 0)
            {
                CreateBackup();

                logger.Info("Beginning upgrade...");
                var migration = kernel.Get<TriggersV2Migration>();
                migration.Run();
                logger.Info("Migration completed");
                currentVersion = 1;
                persistentContextsManager.PerformAutoSave(saveAll: true);
                logger.Info("All data saved");
                SaveCurrentVersion();
                NotifyDataMigrationSuccess();
            }
            if (currentVersion == 1)
            {
                logger.Info("Beginning upgrade...");
                var migration = kernel.Get<GrangerHorseColorsMigration>();
                migration.Run();
                logger.Info("Migration completed");
                currentVersion = 2;
                SaveCurrentVersion();
                NotifyDataMigrationSuccess();
            }
            else
            {
                logger.Info($"Wurm Assistant data is at version {currentVersion}. No upgrade required.");
            }
        }

        void NotifyDataMigrationSuccess()
        {
            logger.Info($"Wurm Assistant is now upgraded to data version {currentVersion}");
            trayPopups.Schedule("Data upgrade completed!", "Tricky Business Done");
        }

        void CreateBackup()
        {
            trayPopups.Schedule("Upgrading some Wurm Assistant data", "Some Tricky Business");

            logger.Info($"Wurm Assistant data needs to be upgraded, performing upgrade from data version {currentVersion}");
            logger.Info($"Creating backup of the current data at {backupManager.BackupDirRootPath}");

            var backup = backupManager.CreateDataBackup();
            trayPopups.Schedule($"Created backup of current data at {backup.RootDirPath}", "Tricky Business", 5000);

            logger.Info("Backup created");
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
