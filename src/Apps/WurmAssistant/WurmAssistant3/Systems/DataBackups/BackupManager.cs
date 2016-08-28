using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AldursLab.Essentials.FileSystem;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.DataBackups
{
    [KernelBind(BindingHint.Singleton)]
    public class BackupManager
    {
        readonly IWurmAssistantDataDirectory dataDirectory;
        readonly ILogger logger;
        readonly string backupDirRootPath;

        bool dataBackupRestorationIsLocked = false;

        public BackupManager([NotNull] IWurmAssistantDataDirectory dataDirectory, [NotNull] ILogger logger)
        {
            if (dataDirectory == null) throw new ArgumentNullException(nameof(dataDirectory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.dataDirectory = dataDirectory;
            this.logger = logger;
            backupDirRootPath = Path.Combine(dataDirectory.DirectoryPath, "Backup");
        }

        public string BackupDirRootPath => backupDirRootPath;

        public void CreateTodaysDataBackupIfNotExists()
        {
            var todayBackup = GetBackups().FirstOrDefault(backup => backup.Timestamp.Date == DateTime.Now.Date);
            if (todayBackup == null)
            {
                CreateDataBackup();
            }
        }

        public DataBackup CreateDataBackup()
        {
            var backupTimeStamp = DataBackup.GenerateDirNameBasedOnTimestamp(DateTime.Now);
            var currentBackupRootPath = Path.Combine(backupDirRootPath, backupTimeStamp);

            Directory.CreateDirectory(currentBackupRootPath);

            {
                var upgradeVersionFile = Path.Combine(dataDirectory.DirectoryPath, "upgrade.dat");
                var backupVersionFile = Path.Combine(currentBackupRootPath, "upgrade.dat");
                File.Copy(upgradeVersionFile, backupVersionFile);
            }
            {
                var dataDir = Path.Combine(dataDirectory.DirectoryPath, "Data");
                var backupDir = Path.Combine(currentBackupRootPath, "Data");
                DirectoryOps.CopyRecursively(dataDir, backupDir);
            }
            {
                var dataV2Dir = Path.Combine(dataDirectory.DirectoryPath, "DataV2");
                var backupV2Dir = Path.Combine(currentBackupRootPath, "DataV2");
                DirectoryOps.CopyRecursively(dataV2Dir, backupV2Dir);
            }
            {
                var soundBankDir = Path.Combine(dataDirectory.DirectoryPath, "SoundBank");
                var backupSoundBankDir = Path.Combine(currentBackupRootPath, "SoundBank");
                DirectoryOps.CopyRecursively(soundBankDir, backupSoundBankDir);
            }

            var confirmFile = Path.Combine(currentBackupRootPath, "completed.dat");
            File.WriteAllText(confirmFile, $"This backup has been fully completed at {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");

            var backup = new DataBackup(currentBackupRootPath);

            logger.Info($"Created data backup named {backup.BackupName}");

            return backup;
        }

        public void TrimOldDataBackups(TimeSpan treshholdTime)
        {
            // keep no less than 5 last backups, but delete old backups above that limit
            var oldBackups =
                GetBackups()
                    .OrderByDescending(backup => backup.Timestamp)
                    .Skip(5)
                    .Where(backup => backup.Timestamp < DateTime.Now - treshholdTime)
                    .ToArray();

            foreach (var dataBackup in oldBackups)
            {
                Directory.Delete(dataBackup.RootDirPath, true);
                logger.Info($"Deleted old backup named: {dataBackup.BackupName}");
            }
        }

        public IEnumerable<DataBackup> GetBackups()
        {
            var allBackups = new DirectoryInfo(backupDirRootPath).GetDirectories().Select(info => new DataBackup(info.FullName));
            return allBackups.ToArray();
        }

        public void LockRestoreDataBackupForThisSession()
        {
            dataBackupRestorationIsLocked = true;
            logger.Info("Data backup system has been locked.");
        }

        public void RestoreDataBackup(string backupName)
        {
            if (dataBackupRestorationIsLocked)
            {
                throw new InvalidOperationException("Backup restoration has been locked for this session, " +
                                                    "likely because some features have already been initialized. " + 
                                                    "Please run this command at an earlier stage in application lifecycle.");
            }

            var backup = GetBackups().FirstOrDefault(dataBackup => dataBackup.BackupName == backupName);
            if (backup == null)
            {
                throw new InvalidOperationException($"Backup named {backupName} does not exist.");
            }

            if (!backup.IsComplete)
            {
                if (
                    MessageBox.Show(
                        $"Backup named {backupName} does not appear to have been fully completed. " +
                        $"This might happen if an error interrupted it's creation. " +
                        $"Would you like to try it anyway?",
                        "Backup restoration issue",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Asterisk,
                        MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                {
                    throw new InvalidOperationException(
                        $"Cancelling restoration of backup {backupName} due to user request.");
                }
            }

            List<string> restoredFolders = new List<string>();

            try
            {
                if (backup.HasVersionFile)
                {
                    var upgradeVersionFile = Path.Combine(dataDirectory.DirectoryPath, "upgrade.dat");
                    var backupVersionFile = Path.Combine(backup.RootDirPath, "upgrade.dat");

                    var tempBackupPath = upgradeVersionFile + ".old";
                    if (File.Exists(tempBackupPath))
                    {
                        File.Delete(tempBackupPath);
                    }

                    File.Move(upgradeVersionFile, tempBackupPath);
                    try
                    {
                        File.Copy(backupVersionFile, upgradeVersionFile);
                    }
                    catch (Exception exception)
                    {
                        // This is important file, minimize potential issues by rolling it back.
                        try
                        {
                            File.Move(tempBackupPath, upgradeVersionFile);
                            throw;
                        }
                        catch (Exception innerException)
                        {
                            throw new AggregateException(exception, innerException);
                        }
                    }
                    File.Delete(tempBackupPath);
                }

                if (backup.HasDataDir)
                {
                    var dataDir = Path.Combine(dataDirectory.DirectoryPath, "Data");
                    var backupDir = Path.Combine(backup.RootDirPath, "Data");
                    Directory.Delete(dataDir, true);
                    DirectoryOps.CopyRecursively(backupDir, dataDir);

                    restoredFolders.Add("Data");
                }

                if (backup.HasDataV2Dir)
                {
                    var dataV2Dir = Path.Combine(dataDirectory.DirectoryPath, "DataV2");
                    var backupV2Dir = Path.Combine(backup.RootDirPath, "DataV2");
                    Directory.Delete(dataV2Dir, true);
                    DirectoryOps.CopyRecursively(backupV2Dir, dataV2Dir);

                    restoredFolders.Add("DataV2");
                }

                if (backup.HasSoundBankDir)
                {
                    var soundBankDir = Path.Combine(dataDirectory.DirectoryPath, "SoundBank");
                    var backupSoundBankDir = Path.Combine(backup.RootDirPath, "SoundBank");
                    Directory.Delete(soundBankDir, true);
                    DirectoryOps.CopyRecursively(backupSoundBankDir, soundBankDir);

                    restoredFolders.Add("SoundBank");
                }
                var message = $"Wurm Assistant data backup has been successfully restored from snapshot: {backup.BackupName}. " +
                              $"Restored folders: {string.Join(", ", restoredFolders)}";
                logger.Info(message);
                MessageBox.Show(message, "Backup restoration success", MessageBoxButton.OK);
            }
            catch (Exception)
            {
                var message =
                    $"There was a problem during restoration of Wurm Assistant data from backup snapshot: {backup.BackupName}. " +
                    $"Data may not be complete or may be corrupted. See logs for details." +
                    $"Restored folders: {string.Join(", ", restoredFolders)}";
                logger.Error(message);
                MessageBox.Show(message, "Backup restoration issue", MessageBoxButton.OK);

                throw;
            }
        }
    }
}
