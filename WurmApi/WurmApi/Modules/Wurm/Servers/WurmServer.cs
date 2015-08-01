using System;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Wurm.Servers.Jobs;
using AldurSoft.WurmApi.Modules.Wurm.Servers.WurmServersModel;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    public class WurmServer : IWurmServer
    {
        private readonly WurmServerInfo wurmServerInfo;
        readonly QueuedJobsSyncRunner<Job, JobResult> jobRunner;

        internal WurmServer(WurmServerInfo wurmServerInfo, [NotNull] QueuedJobsSyncRunner<Job, JobResult> jobRunner)
        {
            if (wurmServerInfo == null) throw new ArgumentNullException("wurmServerInfo");
            if (jobRunner == null) throw new ArgumentNullException("jobRunner");
            this.wurmServerInfo = wurmServerInfo;
            this.jobRunner = jobRunner;
        }

        public virtual ServerName ServerName
        {
            get
            {
                return wurmServerInfo.Name;
            }
        }

        public ServerGroup ServerGroup
        {
            get
            {
                return wurmServerInfo.ServerGroup;
            }
        }

        #region TryGetCurrentTime

        public async Task<WurmDateTime?> TryGetCurrentTimeAsync()
        {
            return await TryGetCurrentTimeAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public WurmDateTime? TryGetCurrentTime()
        {
            return TryGetCurrentTime(CancellationToken.None);
        }

        public async Task<WurmDateTime?> TryGetCurrentTimeAsync(CancellationToken cancellationToken)
        {
            var result =
                await
                    jobRunner.Run(new CurrentWurmDateTimeJob(ServerName), cancellationToken).ConfigureAwait(false);
            return result.WurmDateTime;
        }

        public WurmDateTime? TryGetCurrentTime(CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetCurrentTimeAsync(cancellationToken).Result);
        }

        #endregion

        private WurmDateTime AdjustedWurmDateTime(ServerDateStamped date)
        {
            return date.WurmDateTime + (Time.Get.LocalNowOffset - date.Stamp);
        }

        #region TryGetCurrentUptime

        public async Task<TimeSpan?> TryGetCurrentUptimeAsync()
        {
            return await TryGetCurrentUptimeAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public TimeSpan? TryGetCurrentUptime()
        {
            return TryGetCurrentUptime(CancellationToken.None);
        }

        public async Task<TimeSpan?> TryGetCurrentUptimeAsync(CancellationToken cancellationToken)
        {
            var result =
                await
                    jobRunner.Run(new CurrentWurmDateTimeJob(ServerName), cancellationToken).ConfigureAwait(false);
            return result.Uptime;
        }

        public TimeSpan? TryGetCurrentUptime(CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetCurrentUptimeAsync(cancellationToken).Result);
        }

        #endregion

        private TimeSpan AdjustedUptime(ServerUptimeStamped uptime)
        {
            return uptime.Uptime + (Time.Get.LocalNowOffset - uptime.Stamp);
        }
    }
}