﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Systems;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace AldursLab.WurmAssistant3.SystemsCustomBind
{
    public class LoggingSystem : ILoggingSystem
    {
        readonly LoggingConfiguration config;

        readonly LoggingRule globalrule;

        public LoggingSystem([NotNull] IEnvironment environment)
        {
            if (environment == null) throw new ArgumentNullException("environment");
            config = new LoggingConfiguration();
            LogManager.Configuration = config;

            // rolling custom archival because NLog options failed me.
            var currentLogDir = Path.Combine(environment.FullPathToRootDataDirectory, "Logs");
            if (!Directory.Exists(currentLogDir)) Directory.CreateDirectory(currentLogDir);

            var archiveLogDir = Path.Combine(currentLogDir, "Archive");
            if (!Directory.Exists(archiveLogDir)) Directory.CreateDirectory(archiveLogDir);

            MoveOldLogToArchive(currentLogDir, archiveLogDir);
            TrimOlgLogFiles(archiveLogDir, maxMegabytes: 50);

            var currentFileName = Time.Clock.LocalNow.ToString("yyy-MM-dd_HH-mm-ss-ffffff") + ".txt";
            FullPathToCurrentLogFile = currentFileName;
            var fileTarget = new FileTarget
            {
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                KeepFileOpen = true,
                FileName = Path.Combine(currentLogDir, currentFileName),
            };

            globalrule = new LoggingRule("*", LogLevel.Debug, fileTarget);
        }

        public string FullPathToCurrentLogFile { get; private set; }

        private void TrimOlgLogFiles(string archiveLogDir, int maxMegabytes)
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

        private void MoveOldLogToArchive(string currentLogDir, string archiveLogDir)
        {
            var currentFiles = new DirectoryInfo(currentLogDir).GetFiles();
            foreach (var file in currentFiles)
            {
                var newPath = Path.Combine(archiveLogDir, file.Name);
                var uniqueNewPath = GetNewUniqueName(newPath);
                file.MoveTo(uniqueNewPath);
            }
        }

        private string GetNewUniqueName(string newPath, int counter = 1)
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

        public void EnableGlobalLogging()
        {
            config.LoggingRules.Add(globalrule);
            Apply();
        }

        private void Apply()
        {
            LogManager.Configuration = config;
        }
    }
}
