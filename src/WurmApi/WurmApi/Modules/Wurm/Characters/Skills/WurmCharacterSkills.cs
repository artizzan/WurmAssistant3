using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Modules.Events.Public;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class WurmCharacterSkills : IWurmCharacterSkills, IHandle<YouAreOnEventDetectedOnLiveLogs>, IDisposable
    {
        readonly IWurmCharacter character;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IWurmLogsMonitorInternal logsMonitor;
        readonly IWurmLogsHistory logsHistory;
        readonly IWurmApiLogger logger;

        readonly SkillsMap skillsMap;
        readonly SkillDumpsManager skillDumps;

        IWurmServer currentServer;

        readonly TaskCompletionSource<DateTime> currentServerLookupFinished = new TaskCompletionSource<DateTime>();
        DateTime? scannedMinDate;

        readonly SemaphoreSlim scanJobSemaphore = new SemaphoreSlim(1,1);

        public event EventHandler<SkillsChangedEventArgs> SkillsChanged;

        readonly ConcurrentQueue<SkillInfo> changedSkills = new ConcurrentQueue<SkillInfo>();

        readonly PublicEvent onSkillsChanged;

        public WurmCharacterSkills([NotNull] IWurmCharacter character, [NotNull] IPublicEventInvoker publicEventInvoker,
            [NotNull] IWurmLogsMonitorInternal logsMonitor, [NotNull] IWurmLogsHistory logsHistory,
            [NotNull] IWurmApiLogger logger, IWurmPaths wurmPaths,
            [NotNull] IInternalEventAggregator internalEventAggregator)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            if (publicEventInvoker == null) throw new ArgumentNullException(nameof(publicEventInvoker));
            if (logsMonitor == null) throw new ArgumentNullException(nameof(logsMonitor));
            if (logsHistory == null) throw new ArgumentNullException(nameof(logsHistory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (internalEventAggregator == null) throw new ArgumentNullException(nameof(internalEventAggregator));
            this.character = character;
            this.publicEventInvoker = publicEventInvoker;
            this.logsMonitor = logsMonitor;
            this.logsHistory = logsHistory;
            this.logger = logger;

            skillsMap = new SkillsMap();
            skillDumps = new SkillDumpsManager(character, wurmPaths, logger);

            UpdateCurrentServer();

            onSkillsChanged =
                publicEventInvoker.Create(
                    InvokeOnSkillsChanged,
                    WurmApiTuningParams.PublicEventMarshallerDelay);

            internalEventAggregator.Subscribe(this);

            logsMonitor.SubscribeInternal(character.Name, LogType.Skills, EventHandler);
        }

        void InvokeOnSkillsChanged()
        {
            SkillsChanged.SafeInvoke(this, new SkillsChangedEventArgs(TakeChangedSkills()));
        }

        SkillInfo[] TakeChangedSkills()
        {
            List<SkillInfo> skills = new List<SkillInfo>();
            SkillInfo skill;
            while (changedSkills.TryDequeue(out skill))
            {
                skills.Add(skill);
            }
            return skills.OrderBy(info => info.Stamp).ToArray();
        }

        async void UpdateCurrentServer()
        {
            try
            {
                var server = await character.TryGetCurrentServerAsync().ConfigureAwait(false);
                if (server != null)
                {
                    currentServer = server;
                }
                else
                {
                    logger.Log(LogLevel.Warn, "Current server unknown for: " + character.Name.Capitalized, this, null);
                }
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "error on updating current server", this, exception);
            }
            await Task.Run(() => currentServerLookupFinished.TrySetResult(Time.Get.LocalNow));
        }

        void EventHandler(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            // note: event needs to be triggered regardless of skill being already known and up to date

            // there is no point in updating skill values, if server is not known
            var currentSvr = currentServer;
            if (currentSvr == null) return;

            SkillEntryParser parser = new SkillEntryParser(logger);
            bool anyParsed = false;
            foreach (var wurmLogEntry in logsMonitorEventArgs.WurmLogEntries)
            {
                SkillInfo skillInfo = parser.TryParseSkillInfoFromLogLine(wurmLogEntry);
                if (skillInfo != null)
                {
                    skillInfo.Server = currentSvr;
                    changedSkills.Enqueue(skillInfo);
                    skillsMap.UpdateSkill(skillInfo, currentSvr);
                    anyParsed = true;
                }
            }

            if (anyParsed) onSkillsChanged.Trigger();
        }

        public async Task<SkillInfo> TryGetCurrentSkillLevelAsync(string skillName, ServerGroup serverGroup, TimeSpan maxTimeToLookBackInLogs)
        {
            // note: semaphore(1,1) in this method ensures, that there are no races
            // be extra careful if loosening this constraint!

            try
            {
                await scanJobSemaphore.WaitAsync().ConfigureAwait(false);
                await ScanLogsHistory(maxTimeToLookBackInLogs).ConfigureAwait(false);
                var mapSkill = TryGetSkillFromMap(skillName, serverGroup);
                var dumpSkill = await TryGetSkillFromDumps(skillName, serverGroup);
                return ChooseLatestSkillOrNull(mapSkill, dumpSkill);
            }
            finally
            {
                scanJobSemaphore.Release();
            }
        }

        SkillInfo TryGetSkillFromMap(string skillName, ServerGroup serverGroup)
        {
            return skillsMap.TryGetSkill(skillName, serverGroup);
        }

        async Task<SkillInfo> TryGetSkillFromDumps(string skillName, ServerGroup serverGroup)
        {
            SkillInfo skill = null;
            var dump = await skillDumps.GetSkillDumpAsync(serverGroup).ConfigureAwait(false);
            var skillinfo = dump?.TryGetSkillLevel(skillName);
            if (skillinfo != null)
            {

                skill = new SkillInfo(skillName, skillinfo.Value, dump.Stamp, null)
                {
                    Server = await character.TryGetHistoricServerAtLogStampAsync(dump.Stamp)
                };
            }
            return skill;
        }

        SkillInfo ChooseLatestSkillOrNull(SkillInfo mapSkill, SkillInfo dumpSkill)
        {
            if (mapSkill != null && dumpSkill != null)
            {
                return mapSkill.Stamp > dumpSkill.Stamp ? mapSkill : dumpSkill;
            }
            return mapSkill ?? dumpSkill;
        }

        public SkillInfo TryGetCurrentSkillLevel(string skillName, ServerGroup serverGroup, TimeSpan maxTimeToLookBackInLogs)
        {
            return
                TaskHelper.UnwrapSingularAggegateException(
                    () => TryGetCurrentSkillLevelAsync(skillName, serverGroup, maxTimeToLookBackInLogs).Result);
        }

        private async Task ScanLogsHistory(TimeSpan maxTimeToLookBackInLogs)
        {
            DateTime maxDate = await currentServerLookupFinished.Task.ConfigureAwait(false);
            DateTime minDate = maxDate.SubtractConstrain(maxTimeToLookBackInLogs);

            // if already scanned, optimize
            if (scannedMinDate != null)
            {
                if (minDate >= scannedMinDate)
                {
                    // do not scan, if this period has already been scanned
                    return;
                }
                maxDate = scannedMinDate.Value;
            }

            var entries = await logsHistory.ScanAsync(new LogSearchParameters()
            {
                CharacterName = character.Name.Normalized,
                LogType = LogType.Skills,
                MinDate = minDate,
                MaxDate = maxDate
            }).ConfigureAwait(false);
            
            SkillEntryParser parser = new SkillEntryParser(logger);
            foreach (var wurmLogEntry in entries)
            {
                SkillInfo skillInfo = parser.TryParseSkillInfoFromLogLine(wurmLogEntry);
                if (skillInfo != null)
                {
                    var entryServer =
                        await
                            character.TryGetHistoricServerAtLogStampAsync(wurmLogEntry.Timestamp).ConfigureAwait(false);
                    if (entryServer != null)
                    {
                        skillInfo.Server = entryServer;
                        skillsMap.UpdateSkill(skillInfo, entryServer);
                    }
                    else
                    {
                        logger.Log(LogLevel.Info,
                            "Skill info rejected, server could not be identified for this entry: " + wurmLogEntry,
                            this,
                            null);
                    }
                }
            }

            scannedMinDate = minDate;
        }

        /// <summary>
        /// If skill dump is not found, returned SkillDump.IsNull will be true.
        /// </summary>
        /// <param name="serverGroupId"></param>
        /// <returns></returns>
        public async Task<SkillDump> GetLatestSkillDumpAsync(string serverGroupId)
        {
            var dump = await skillDumps.GetSkillDumpAsync(new ServerGroup(serverGroupId)).ConfigureAwait(false);
            return dump;
        }

        public void Handle(YouAreOnEventDetectedOnLiveLogs message)
        {
            if (message.CharacterName == character.Name && message.CurrentServerNameChanged)
            {
                UpdateCurrentServer();
            }
        }

        public void Dispose()
        {
            skillDumps.Dispose();
        }
    }
}
