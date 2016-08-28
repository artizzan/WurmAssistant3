using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.DataBackups
{
    public class DataBackup
    {
        public DataBackup([NotNull] string rootDirPath)
        {
            if (rootDirPath == null) throw new ArgumentNullException(nameof(rootDirPath));
            RootDirPath = rootDirPath;

            var rootDirPathInfo = new DirectoryInfo(RootDirPath);
            
            BackupName = rootDirPathInfo.Name;

            IsComplete = File.Exists(Path.Combine(RootDirPath, "completed.dat"));
            Timestamp = ParseDate(BackupName);

            HasDataDir = Directory.Exists(Path.Combine(RootDirPath, "Data"));
            HasDataV2Dir = Directory.Exists(Path.Combine(RootDirPath, "DataV2"));
            HasSoundBankDir = Directory.Exists(Path.Combine(RootDirPath, "SoundBank"));
            HasVersionFile = File.Exists(Path.Combine(RootDirPath, "upgrade.dat"));
        }

        public string RootDirPath { get; private set; }

        public string BackupName { get; private set; }

        public bool IsComplete { get; private set; }

        public DateTime Timestamp { get; private set; }

        public bool HasDataDir { get; private set; }
        public bool HasDataV2Dir { get; private set; }
        public bool HasSoundBankDir { get; private set; }
        public bool HasVersionFile { get; private set; }

        DateTime ParseDate(string folderName)
        {
            var result = Regex.Match(folderName, @"(\d\d\d\d)(\d\d)(\d\d)(\d\d)(\d\d)(\d\d)");
            return new DateTime(
                int.Parse(result.Groups[1].Value),
                int.Parse(result.Groups[2].Value),
                int.Parse(result.Groups[3].Value),
                int.Parse(result.Groups[4].Value),
                int.Parse(result.Groups[5].Value),
                int.Parse(result.Groups[6].Value));
        }

        public static string GenerateDirNameBasedOnTimestamp(DateTime timestamp)
        {
            return timestamp.ToString("yyyyMMddHHmmss");
        }
    }
}