using System;
using System.IO;
using System.Text;

namespace AldursLab.WurmApi.Tests.Integration.Builders.WurmClient
{
    class WurmLogs
    {
        readonly DirectoryInfo logsDir;
        readonly Platform targetPlatform;

        public LogSaveMode LogSaveMode { get; set; }

        public WurmLogs(DirectoryInfo logsDir, Platform targetPlatform)
        {
            this.logsDir = logsDir;
            this.targetPlatform = targetPlatform;
            LogSaveMode = LogSaveMode.Daily;
        }

        //note: refactor this before adding more

        public WurmLogs CreateCustomLogFile(string name, string content = null)
        {
            if (content == null) content = string.Empty;
            if (targetPlatform != Platform.Windows)
            {
                content = content.Replace("\r\n", "\n");
            }
            File.WriteAllText(Path.Combine(logsDir.FullName, name), content, Encoding.UTF8);
            return this;
        }

        public WurmLogs CreateEventLogFile()
        {
            var fileName = CreateCurrentEventLogFileName();
            CreateCustomLogFile(fileName, "Logging started " + Time.Get.LocalNow.ToString("yyyy-MM-dd") + "\r\n");
            return this;
        }

        public WurmLogs CreateSkillsLogFile()
        {
            var fileName = CreateCurrentSkillsLogFileName();
            CreateCustomLogFile(fileName, "Logging started " + Time.Get.LocalNow.ToString("yyyy-MM-dd") + "\r\n");
            return this;
        }

        public WurmLogs WriteEventLog(string content)
        {
            var fileName = CreateCurrentEventLogFileName();
            WriteLogLine(fileName, content, null, Time.Get.LocalNow);
            return this;
        }

        public WurmLogs WriteSkillLog(string skillName, float value, float by = 0f)
        {
            var fileName = CreateCurrentSkillsLogFileName();
            WriteLogLine(fileName, string.Format("{2} increased by {0} to {1}", by, value, skillName), null, Time.Get.LocalNow);
            return this;
        }

        void WriteLogLine(string fileName, string content, string source, DateTime now)
        {
            var file = GetLogFileInfo(fileName);
            if (!file.Exists)
            {
                CreateCustomLogFile(file.Name, "Logging started " + now.ToString("yyyy-MM-dd") + "\r\n");
            }
            string line =
                $"[{now.ToString("HH:mm:ss")}] {(source != null ? "<" + source + "> " : string.Empty)}{content}";
            File.AppendAllLines(file.FullName, new[]{ line });
        }

        FileInfo GetLogFileInfo(string fileName)
        {
            return new FileInfo(Path.Combine(logsDir.FullName, fileName));
        }

        string CreateCurrentEventLogFileName()
        {
            var now = Time.Get.LocalNow;
            var datePart = LogSaveMode == LogSaveMode.Daily ? now.ToString("yyyy-MM-dd") : now.ToString("yyyy-mm");
            return $"_Event.{datePart}.txt";
        }

        string CreateCurrentSkillsLogFileName()
        {
            var now = Time.Get.LocalNow;
            var datePart = LogSaveMode == LogSaveMode.Daily ? now.ToString("yyyy-MM-dd") : now.ToString("yyyy-mm");
            return $"_Skills.{datePart}.txt";
        }
    }
}