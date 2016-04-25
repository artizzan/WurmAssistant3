using System;
using System.Threading;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory;
using AldursLab.WurmApi.Modules.Wurm.Servers.Jobs;
using AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class JobRunner : JobExecutor<Job, JobResult>
    {
        readonly LiveLogs liveLogs;
        readonly LogHistory logHistory;
        readonly WebFeeds webFeeds;
        readonly PersistentCollectionsLibrary persistentCollectionsLibrary;

        public JobRunner([NotNull] LiveLogs liveLogs, [NotNull] LogHistory logHistory, [NotNull] WebFeeds webFeeds,
            [NotNull] PersistentCollectionsLibrary persistentCollectionsLibrary)
        {
            if (liveLogs == null) throw new ArgumentNullException(nameof(liveLogs));
            if (logHistory == null) throw new ArgumentNullException(nameof(logHistory));
            if (webFeeds == null) throw new ArgumentNullException(nameof(webFeeds));
            if (persistentCollectionsLibrary == null) throw new ArgumentNullException(nameof(persistentCollectionsLibrary));
            this.liveLogs = liveLogs;
            this.logHistory = logHistory;
            this.webFeeds = webFeeds;
            this.persistentCollectionsLibrary = persistentCollectionsLibrary;
        }

        public override JobResult Execute(Job jobContext, JobCancellationManager jobCancellationManager)
        {
            var dateTimeJob = jobContext as CurrentWurmDateTimeJob;
            if (dateTimeJob != null)
            {
                var dateTime = TryGetCurrentTime(jobContext.ServerName);
                return new JobResult(dateTime, null);
            }
            var uptimeJob = jobContext as CurrentUptimeJob;
            if (uptimeJob != null)
            {
                var uptime = TryGetCurrentUptime(jobContext.ServerName);
                return new JobResult(null, uptime);
            }
            persistentCollectionsLibrary.SaveChanged();
            throw new InvalidOperationException("Unknown type of job: " + jobContext.GetType());
        }

        public override void IdleJob(CancellationToken cancellationToken)
        {
            liveLogs.HandleNewLiveData();
            webFeeds.UpdateWebData();

            persistentCollectionsLibrary.SaveChanged();
        }

        public override TimeSpan IdleJobTreshhold => TimeSpan.FromMinutes(5);

        WurmDateTime? TryGetCurrentTime(ServerName serverName)
        {
            var liveData = liveLogs.GetForServer(serverName);
            if (liveData.ServerDate.Stamp > DateTimeOffset.MinValue)
            {
                return AdjustedWurmDateTime(liveData.ServerDate);
            }
            var logHistoryData = logHistory.GetForServer(serverName);
            if (logHistoryData.ServerDate.Stamp > Time.Get.LocalNowOffset.AddDays(-1))
            {
                return AdjustedWurmDateTime(logHistoryData.ServerDate);
            }
            var webFeedsData = webFeeds.GetForServer(serverName);
            if (webFeedsData.ServerDate.Stamp > DateTimeOffset.MinValue)
            {
                return AdjustedWurmDateTime(webFeedsData.ServerDate);
            }
            if (logHistoryData.ServerDate.Stamp > DateTimeOffset.MinValue)
            {
                return AdjustedWurmDateTime(logHistoryData.ServerDate);
            }

            return null;
        }

        TimeSpan? TryGetCurrentUptime(ServerName serverName)
        {
            var liveData = liveLogs.GetForServer(serverName);
            if (liveData.ServerUptime.Stamp > DateTimeOffset.MinValue)
            {
                return AdjustedUptime(liveData.ServerUptime);
            }
            var logHistoryData = logHistory.GetForServer(serverName);
            if (logHistoryData.ServerUptime.Stamp > Time.Get.LocalNowOffset.AddDays(-1))
            {
                return AdjustedUptime(logHistoryData.ServerUptime);
            }
            var webFeedsData = webFeeds.GetForServer(serverName);
            if (webFeedsData.ServerUptime.Stamp > DateTimeOffset.MinValue)
            {
                return AdjustedUptime(webFeedsData.ServerUptime);
            }
            if (logHistoryData.ServerUptime.Stamp > DateTimeOffset.MinValue)
            {
                return AdjustedUptime(logHistoryData.ServerUptime);
            }

            return null;
        }

        private TimeSpan AdjustedUptime(ServerUptimeStamped uptime)
        {
            return uptime.Uptime + (Time.Get.LocalNowOffset - uptime.Stamp);
        }

        private WurmDateTime AdjustedWurmDateTime(ServerDateStamped date)
        {
            return date.WurmDateTime + (Time.Get.LocalNowOffset - date.Stamp);
        }

    }
}