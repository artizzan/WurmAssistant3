using System;
using System.Collections.Generic;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Wurm.Servers.Jobs;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class WurmServerFactory
    {
        readonly QueuedJobsSyncRunner<Job, JobResult> jobRunner;
        private readonly HashSet<ServerName> createdServers = new HashSet<ServerName>();

        public WurmServerFactory([NotNull] QueuedJobsSyncRunner<Job, JobResult> jobRunner)
        {
            if (jobRunner == null) throw new ArgumentNullException("jobRunner");
            this.jobRunner = jobRunner;
        }

        public virtual WurmServer Create(WurmServerInfo wurmServerInfo)
        {
            if (createdServers.Contains(wurmServerInfo.Name))
            {
                throw new InvalidOperationException("this factory has already created Server for name " + wurmServerInfo.Name);
            }

            var server = new WurmServer(wurmServerInfo, jobRunner);

            createdServers.Add(wurmServerInfo.Name);

            return server;
        }
    }
}