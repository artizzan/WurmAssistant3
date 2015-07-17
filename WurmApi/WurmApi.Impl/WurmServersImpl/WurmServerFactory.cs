using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi.Impl.WurmServersImpl
{
    class WurmServerFactory
    {
        private readonly LiveLogs liveLogs;
        private readonly LogHistory logHistory;
        private readonly WebFeeds webFeeds;
        private readonly IThreadGuard threadGuard;
        private readonly HashSet<ServerName> createdServers = new HashSet<ServerName>();

        public WurmServerFactory(
            LiveLogs liveLogs,
            LogHistory logHistory,
            WebFeeds webFeeds,
            IThreadGuard threadGuard)
        {
            if (liveLogs == null) throw new ArgumentNullException("liveLogs");
            if (logHistory == null) throw new ArgumentNullException("logHistory");
            if (webFeeds == null) throw new ArgumentNullException("webFeeds");
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            this.liveLogs = liveLogs;
            this.logHistory = logHistory;
            this.webFeeds = webFeeds;
            this.threadGuard = threadGuard;
        }

        public virtual WurmServer Create(WurmServerInfo wurmServerInfo)
        {
            if (createdServers.Contains(wurmServerInfo.Name))
            {
                throw new InvalidOperationException("this factory has already created Server for name " + wurmServerInfo.Name);
            }

            var server = new WurmServer(wurmServerInfo, liveLogs, logHistory, webFeeds, threadGuard);

            createdServers.Add(wurmServerInfo.Name);

            return server;
        }
    }
}