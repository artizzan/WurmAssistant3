using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Tests.Builders.WurmClient
{
    class WurmPlayer
    {
        DirectoryInfo DumpsDir { get; set; }
        DirectoryInfo LogsDir { get; set; }
        DirectoryInfo ScreenshotsDir { get; set; }
        FileInfo ConfigTxt { get; set; }

        public WurmPlayer(DirectoryInfo playerDir, string name, Platform targetPlatform)
        {
            Name = name;
            PlayerDir = playerDir;
            DumpsDir = playerDir.CreateSubdirectory("dumps");
            LogsDir = playerDir.CreateSubdirectory("logs");
            ScreenshotsDir = playerDir.CreateSubdirectory("screenshots");
            ConfigTxt = new FileInfo(Path.Combine(playerDir.FullName, "config.txt"));
            File.WriteAllText(ConfigTxt.FullName, string.Empty);
            Logs = new WurmLogs(LogsDir, targetPlatform);
        }

        public string Name { get; private set; }

        public WurmLogs Logs { get; private set; }

        public DirectoryInfo PlayerDir { get; }

        public WurmPlayer SetConfigName(string name)
        {
            File.WriteAllText(ConfigTxt.FullName, name + "\r\n");
            return this;
        }

        public void CreateSkillDump(DateTimeOffset stamp, params DumpEntry[] entries)
        {
            var newFileBuilder = new DumpFileBuilder(DumpsDir);
            newFileBuilder.SetStamp(stamp);
            foreach (var dumpEntry in entries)
            {
                newFileBuilder.Add(dumpEntry);
            }
            newFileBuilder.CreateFile();
        }
    }

    internal class DumpEntry
    {
        public string SkillName { get; set; }
        public float Level { get; set; }
        public int Indents { get; set; }
    }

    internal class DumpFileBuilder
    {
        readonly DirectoryInfo dirInfo;
        DateTimeOffset stamp;
        readonly List<DumpEntry> entries = new List<DumpEntry>(); 

        public DumpFileBuilder([NotNull] DirectoryInfo dirInfo)
        {
            if (dirInfo == null) throw new ArgumentNullException(nameof(dirInfo));
            this.dirInfo = dirInfo;
        }

        public void SetStamp(DateTimeOffset stamp)
        {
            this.stamp = stamp;
        }

        public void Add(DumpEntry dumpEntry)
        {
            entries.Add(dumpEntry);
        }

        public void CreateFile()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Skills dumped at " + stamp.ToString("yyyy-MM-dd"));
            sb.AppendLine("-----");
            foreach (var dumpEntry in entries)
            {
                for (int i = 0; i < dumpEntry.Indents; i++)
                {
                    sb.Append(" ");
                }
                sb.AppendLine(
                    String.Format("{0}: {1} {2} 0",
                        dumpEntry.SkillName,
                        dumpEntry.Level.ToString("0.0#####", CultureInfo.InvariantCulture),
                        dumpEntry.Level.ToString("0.0#####", CultureInfo.InvariantCulture)));
            }
            File.WriteAllText(
                Path.Combine(dirInfo.FullName, $"skills.{stamp.ToString("yyyyMMdd")}.{stamp.ToString("HHmm")}.txt"),
                sb.ToString(),
                Encoding.UTF8);
        }
    }
}