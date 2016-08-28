using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AldursLab.WurmAssistant3.Systems.DataBackups;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main.ViewModels
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class DataBackupsViewModel : Screen
    {
        readonly BackupManager backupManager;
        readonly DataBackupUserSettings userSettings;

        readonly ObservableCollection<DataBackup> dataBackups = new ObservableCollection<DataBackup>();
        string backupSystemStatus;

        public DataBackupsViewModel([NotNull] BackupManager backupManager, [NotNull] DataBackupUserSettings userSettings)
        {
            if (backupManager == null) throw new ArgumentNullException(nameof(backupManager));
            if (userSettings == null) throw new ArgumentNullException(nameof(userSettings));
            this.backupManager = backupManager;
            this.userSettings = userSettings;

            var backups = backupManager.GetBackups().OrderByDescending(backup => backup.Timestamp);
            foreach (var dataBackup in backups)
            {
                dataBackups.Add(dataBackup);
            }

            RefreshBackupSystemStatus();
        }

        public ObservableCollection<DataBackup> DataBackups => dataBackups;

        public string BackupSystemStatus
        {
            get { return backupSystemStatus; }
            set
            {
                if (value == backupSystemStatus) return;
                backupSystemStatus = value;
                NotifyOfPropertyChange();
            }
        }

        public string Title => "Data Backups Manager";

        [UsedImplicitly]
        public void ScheduleRestore([NotNull] DataBackup dataBackup)
        {
            if (dataBackup == null) throw new ArgumentNullException(nameof(dataBackup));

            var dialogResult = MessageBox.Show(
                "Selected backup will be restored after Wurm Assistant is restarted. Current data will be overwritten.",
                "Confirmation needed", 
                MessageBoxButton.OKCancel);

            if (dialogResult == MessageBoxResult.Cancel || dialogResult == MessageBoxResult.None)
            {
                return;
            }

            userSettings.RestoreBackupRequested = true;
            userSettings.RestoreBackupName = dataBackup.BackupName;
            userSettings.Save();
            RefreshBackupSystemStatus();
        }

        [UsedImplicitly]
        public void ScheduleBackup()
        {
            userSettings.CreateNewBackupRequested = true;
            userSettings.Save();
            RefreshBackupSystemStatus();
        }

        [UsedImplicitly]
        public void CancelAllPendingOperations()
        {
            userSettings.RestoreBackupRequested = false;
            userSettings.CreateNewBackupRequested = false;
            userSettings.RestoreBackupName = string.Empty;
            userSettings.Save();
            RefreshBackupSystemStatus();
        }

        void RefreshBackupSystemStatus()
        {
            string status = string.Empty;
            if (userSettings.CreateNewBackupRequested)
            {
                status +=
                    "A new backup will be created on next launch of Wurm Assistant, for the current mode (Wurm Online or Wurm Unlimited).";
                status += Environment.NewLine;
            }
            if (userSettings.RestoreBackupRequested && userSettings.CreateNewBackupRequested)
            {
                status += "Afterwards... " + Environment.NewLine;
            }
            if (userSettings.RestoreBackupRequested)
            {
                status +=
                    $"Wurm Assistant data for current mode will be restored from the snapshot named {userSettings.RestoreBackupName}.";
            }
            if (!userSettings.RestoreBackupRequested && !userSettings.CreateNewBackupRequested)
            {
                status += "No operation is currently scheduled.";
            }

            status += Environment.NewLine + $"Backups older than {userSettings.BackupRetentionTreshhold.Days} days are automatically cleared.";

            BackupSystemStatus = status;
        }
    }
}
