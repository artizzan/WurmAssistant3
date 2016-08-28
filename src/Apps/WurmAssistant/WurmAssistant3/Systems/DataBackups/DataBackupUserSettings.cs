using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Core;

namespace AldursLab.WurmAssistant3.Systems.DataBackups
{
    [KernelBind(BindingHint.Singleton)]
    public class DataBackupUserSettings
    {
        readonly FileInfo settingsFile;

        public DataBackupUserSettings(IWurmAssistantDataDirectory dataDirectory)
        {
            settingsFile = new FileInfo(Path.Combine(dataDirectory.DirectoryPath, "backupSettings.dat"));
            Load();
        }

        public bool RestoreBackupRequested { get; set; }
        public string RestoreBackupName { get; set; }
        public bool CreateNewBackupRequested { get; set; }
        public TimeSpan BackupRetentionTreshhold => TimeSpan.FromDays(7);

        void Load()
        {
            if (settingsFile.Exists)
            {
                var lines = File.ReadAllLines(settingsFile.FullName, Encoding.UTF8);
                var line1 = lines.FirstOrDefault();
                var line2 = lines.Skip(1).FirstOrDefault();
                var line3 = lines.Skip(2).FirstOrDefault();

                if (!string.IsNullOrEmpty(line1))
                {
                    bool flag;
                    if (bool.TryParse(line1, out flag))
                    {
                        RestoreBackupRequested = flag;
                    }
                }

                if (!string.IsNullOrEmpty(line2))
                {
                    RestoreBackupName = line2;
                }

                if (!string.IsNullOrEmpty(line3))
                {
                    bool flag;
                    if (bool.TryParse(line3, out flag))
                    {
                        CreateNewBackupRequested = flag;
                    }
                }

            }
        }


        public void Save()
        {
            List<string> lines = new List<string>();
            lines.Add(RestoreBackupRequested.ToString());
            lines.Add(RestoreBackupName ?? string.Empty);
            lines.Add(CreateNewBackupRequested.ToString());
            File.WriteAllLines(settingsFile.FullName, lines, Encoding.UTF8);
        }
    }
}
