using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AldursLab.Essentials;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Model
{
    public static class LoggingConfig
    {
        static LoggingConfiguration _config;

        public static void Setup(string logOutputDirFullPath)
        {
            if (logOutputDirFullPath == null)
                throw new ArgumentNullException("logOutputDirFullPath");
            
            _config = new LoggingConfiguration();

            SetupReadableLogging(logOutputDirFullPath);
            SetupVerboseLogging(logOutputDirFullPath);

            ApplyLoggingConfig();
        }

        private static void SetupReadableLogging(string logOutputDirFullPath)
        {
            logOutputDirFullPath = Path.Combine(logOutputDirFullPath, "Readable");

            var currentLogDir = Path.Combine(logOutputDirFullPath);
            if (!Directory.Exists(currentLogDir))
                Directory.CreateDirectory(currentLogDir);

            var archiveLogDir = Path.Combine(currentLogDir, "Archive");
            if (!Directory.Exists(archiveLogDir))
                Directory.CreateDirectory(archiveLogDir);

            MoveOldLogToArchive(currentLogDir, archiveLogDir);
            TrimOlgLogFiles(archiveLogDir, maxMegabytes: 50);

            var currentFileName = Time.Get.LocalNow.ToString("yyy-MM-dd_HH-mm-ss-ffffff") + ".txt";
            CurrentReadableLogFileFullPath = currentFileName;
            var fileTarget = new FileTarget
            {
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                KeepFileOpen = true,
                FileName = Path.Combine(currentLogDir, currentFileName),
                Layout = "${date:universalTime=true} > ${level} > ${message}${onexception:inner= > ${exception:format=Message:maxInnerExceptionLevel=1:innerFormat=Message}}"
            };

            var globalrule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            _config.LoggingRules.Add(globalrule);
        }

        private static void SetupVerboseLogging(string logOutputDirFullPath)
        {
            logOutputDirFullPath = Path.Combine(logOutputDirFullPath, "Verbose");

            var currentLogDir = Path.Combine(logOutputDirFullPath);
            if (!Directory.Exists(currentLogDir))
                Directory.CreateDirectory(currentLogDir);

            var archiveLogDir = Path.Combine(currentLogDir, "Archive");
            if (!Directory.Exists(archiveLogDir))
                Directory.CreateDirectory(archiveLogDir);

            MoveOldLogToArchive(currentLogDir, archiveLogDir);
            TrimOlgLogFiles(archiveLogDir, maxMegabytes: 50);

            var currentFileName = Time.Get.LocalNow.ToString("yyy-MM-dd_HH-mm-ss-ffffff") + ".txt";
            CurrentVerboseLogFileFullPath = currentFileName;
            var fileTarget = new FileTarget
            {
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                KeepFileOpen = true,
                FileName = Path.Combine(currentLogDir, currentFileName),
                Layout = "${date:universalTime=true}|${level}|${message}${onexception:inner=|${exception:format=ToString}}"
            };

            var globalrule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            _config.LoggingRules.Add(globalrule);
        }

        private static void ApplyLoggingConfig()
        {
            LogManager.Configuration = _config;
        }

        public static string CurrentReadableLogFileFullPath { get; private set; }
        public static string CurrentVerboseLogFileFullPath { get; private set; }

        private static void TrimOlgLogFiles(string archiveLogDir, int maxMegabytes)
        {
            var files = new DirectoryInfo(archiveLogDir).GetFiles();
            if (files.Any())
            {
                var totalSize = files.Sum(info => info.Length);
                if (totalSize > ((long)maxMegabytes) * 1024L * 1024L)
                {
                    var fileToDelete = files.Single(file => file.CreationTime == files.Min(info => info.CreationTime));
                    fileToDelete.Delete();
                    TrimOlgLogFiles(archiveLogDir, maxMegabytes);
                }
            }
        }

        private static void MoveOldLogToArchive(string currentLogDir, string archiveLogDir)
        {
            var currentFiles = new DirectoryInfo(currentLogDir).GetFiles();
            foreach (var file in currentFiles)
            {
                var newPath = Path.Combine(archiveLogDir, file.Name);
                var uniqueNewPath = GetNewUniqueName(newPath);
                file.MoveTo(uniqueNewPath);
            }
        }

        private static string GetNewUniqueName(string newPath, int counter = 1)
        {
            var file = new FileInfo(newPath);
            if (file.Exists)
            {
                var dirPath = file.DirectoryName;
                var rawFileName = Path.GetFileNameWithoutExtension(file.Name);
                if (rawFileName.EndsWith("(" + (counter - 1) + ")"))
                {
                    rawFileName = rawFileName.Substring(0, rawFileName.Length - 3);
                }
                var newFileName = rawFileName + "(" + counter + ")" + file.Extension;
                Debug.Assert(dirPath != null, "dirPath != null");
                newPath = Path.Combine(dirPath, newFileName);
                counter++;
                return GetNewUniqueName(newPath, counter);
            }
            return newPath;
        }
    }
}
