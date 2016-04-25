using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class SkillDumpsManager : IDisposable
    {
        readonly IWurmCharacter character;
        readonly IWurmApiLogger logger;
        readonly DirectoryInfo skillDumpsDirectory;
        readonly Dictionary<ServerGroup, SkillDump> latestSkillDumps = new Dictionary<ServerGroup, SkillDump>();

        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1,1);

        static readonly TimeSpan MaxDaysBack = TimeSpan.FromDays(336);

        DateTimeOffset lastSpottedNewFile = DateTimeOffset.MinValue;
        DateTimeOffset lastRebuild = DateTimeOffset.MinValue;
        readonly FileSystemWatcher skillDumpFilesMonitor;

        public SkillDumpsManager([NotNull] IWurmCharacter character, [NotNull] IWurmPaths wurmPaths, IWurmApiLogger logger)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            if (wurmPaths == null) throw new ArgumentNullException(nameof(wurmPaths));
            this.character = character;
            this.logger = logger;

            skillDumpsDirectory = new DirectoryInfo(wurmPaths.GetSkillDumpsFullPathForCharacter(character.Name));
            skillDumpsDirectory.Create();

            skillDumpFilesMonitor = new FileSystemWatcher()
            { 
                Path = skillDumpsDirectory.FullName,
                IncludeSubdirectories = false
            };
            skillDumpFilesMonitor.Created += SkillDumpFilesMonitorOnChanged;
            skillDumpFilesMonitor.Changed += SkillDumpFilesMonitorOnChanged;
            skillDumpFilesMonitor.Renamed += SkillDumpFilesMonitorOnChanged;
            skillDumpFilesMonitor.Deleted += SkillDumpFilesMonitorOnChanged;
            skillDumpFilesMonitor.EnableRaisingEvents = true;
        }

        void SkillDumpFilesMonitorOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            lastSpottedNewFile = Time.Get.LocalNowOffset;
        }

        /// <summary>
        /// If skill dump is not found, empty skill dump is returned.
        /// </summary>
        /// <param name="serverGroup"></param>
        /// <returns></returns>
        public async Task<SkillDump> GetSkillDumpAsync(ServerGroup serverGroup)
        {
            SkillDump dump;
            try
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                if (HasNewSkillDumpAppeared())
                {
                    latestSkillDumps.Clear();
                }
                if (!latestSkillDumps.TryGetValue(serverGroup, out dump))
                {
                    await FindLatestDumpForServerGroup(serverGroup).ConfigureAwait(false);
                    latestSkillDumps.TryGetValue(serverGroup, out dump);
                }
            }
            finally
            {
                semaphore.Release();
            }

            return dump;
        }

        bool HasNewSkillDumpAppeared()
        {
            // grace period to avoid reading incomplete file
            return lastSpottedNewFile < Time.Get.LocalNowOffset - TimeSpan.FromSeconds(10)
                && lastRebuild <= lastSpottedNewFile;
        }

        async Task FindLatestDumpForServerGroup(ServerGroup serverGroup)
        {
            var beginDate = Time.Get.LocalNowOffset;

            var maxBackDate = Time.Get.LocalNow - MaxDaysBack;
            SkillDumpInfo[] dumps = new SkillDumpInfo[0];
            if (skillDumpsDirectory.Exists)
            {
                dumps = skillDumpsDirectory.GetFiles().Select(ConvertFileInfoToSkillDumpInfo)
                                           .Where(info => info != null && info.Stamp > maxBackDate)
                                           .OrderByDescending(info => info.Stamp)
                                           .ToArray();
            }

            SkillDump foundDump = null;
            foreach (var dumpInfo in dumps)
            {
                var server = await character.TryGetHistoricServerAtLogStampAsync(dumpInfo.Stamp).ConfigureAwait(false);
                if (server != null)
                {
                    if (server.ServerGroup == serverGroup)
                    {
                        foundDump = new RealSkillDump(serverGroup, dumpInfo, logger);
                        break;
                    }
                }
                else
                {
                    logger.Log(LogLevel.Info,
                        "Could not identify server for skill dump: " + dumpInfo.FileInfo.FullName,
                        this,
                        null);
                }
            }

            if (foundDump != null)
            {
                latestSkillDumps[serverGroup] = foundDump;
            }
            else
            {
                // if nothing found, place a stub to prevent another file search
                latestSkillDumps[serverGroup] = new StubSkillDump(serverGroup);
            }

            lastRebuild = beginDate;
        }

        SkillDumpInfo ConvertFileInfoToSkillDumpInfo(FileInfo fileInfo)
        {
            var stamp = TryParseDumpStamp(fileInfo);
            if (stamp == null)
            {
                return null;
            }
            return new SkillDumpInfo()
            {
                FileInfo = fileInfo,
                Stamp = stamp.Value
            };
        }

        DateTime? TryParseDumpStamp(FileInfo info)
        {
            try
            {

                var match = Regex.Match(info.Name,
                    @"skills\.(\d\d\d\d)(\d\d)(\d\d)\.(\d\d)(\d\d).*\.txt",
                    RegexOptions.Compiled | RegexOptions.CultureInvariant);
                if (match.Success)
                {
                    var year = int.Parse(match.Groups[1].Value);
                    var month = int.Parse(match.Groups[2].Value);
                    var day = int.Parse(match.Groups[3].Value);
                    var hour = int.Parse(match.Groups[4].Value);
                    var minute = int.Parse(match.Groups[5].Value);
                    return new DateTime(year, month, day, hour, minute, 0);
                }
                else
                {
                    logger.Log(LogLevel.Error,
                        "match failed during parsing skill dump file date, file name: " + info.FullName,
                        this,
                        null);
                    return null;
                }
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error,
                    "exception during parsing skill dump file date, file name: " + info.FullName,
                    this,
                    exception);
                return null;
            }
        }

        public void Dispose()
        {
            skillDumpFilesMonitor.EnableRaisingEvents = false;
            skillDumpFilesMonitor.Dispose();
        }
    }
}