using System;
using System.Collections.Generic;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.Servers.Jobs;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class WurmServerFactory
    {
        readonly QueuedJobsSyncRunner<Job, JobResult> jobRunner;
        private readonly HashSet<ServerName> createdServers = new HashSet<ServerName>();

        public WurmServerFactory([NotNull] QueuedJobsSyncRunner<Job, JobResult> jobRunner)
        {
            if (jobRunner == null) throw new ArgumentNullException(nameof(jobRunner));
            this.jobRunner = jobRunner;
        }

        public WurmServer Create(WurmServerInfo wurmServerInfo)
        {
            if (createdServers.Contains(wurmServerInfo.ServerName))
            {
                throw new InvalidOperationException("this factory has already created Server for name " + wurmServerInfo.ServerName);
            }

            var server = new WurmServer(wurmServerInfo, jobRunner);

            createdServers.Add(wurmServerInfo.ServerName);

            return server;
        }
    }
}