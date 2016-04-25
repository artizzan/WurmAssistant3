using System;
using System.Collections.Generic;
using System.Threading;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.Jobs;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    class JobExecutor : JobExecutor<object, ServerName>
    {
        readonly ServerHistoryProviderFactory serverHistoryProviderFactory;
        readonly PersistentCollectionsLibrary persistentLibrary;
        readonly Dictionary<CharacterName, ServerHistoryProvider> historyProviders = new Dictionary<CharacterName, ServerHistoryProvider>();

        private readonly object locker = new object();

        public JobExecutor([NotNull] ServerHistoryProviderFactory serverHistoryProviderFactory,
            [NotNull] PersistentCollectionsLibrary persistentLibrary)
        {
            if (serverHistoryProviderFactory == null) throw new ArgumentNullException(nameof(serverHistoryProviderFactory));
            if (persistentLibrary == null) throw new ArgumentNullException(nameof(persistentLibrary));
            this.serverHistoryProviderFactory = serverHistoryProviderFactory;
            this.persistentLibrary = persistentLibrary;
        }

        public override ServerName Execute([NotNull] object jobContext,
            [NotNull] JobCancellationManager jobCancellationManager)
        {
            if (jobContext == null) throw new ArgumentNullException(nameof(jobContext));
            if (jobCancellationManager == null) throw new ArgumentNullException(nameof(jobCancellationManager));

            var getServerAtDateJob = jobContext as GetServerAtDateJob;
            if (getServerAtDateJob != null)
            {
                return TryGetServer(getServerAtDateJob.CharacterName, getServerAtDateJob.DateTime, jobCancellationManager);
            }
            var getCurrentServerJob = jobContext as GetCurrentServerJob;
            if (getCurrentServerJob != null)
            {
                return TryGetCurrentServer(getCurrentServerJob.CharacterName, jobCancellationManager);
            }

            ParsePendingEvents();

            persistentLibrary.SaveChanged();

            throw new InvalidOperationException("No handler specified for job type: " + jobContext.GetType());
        }

        ServerName TryGetServer(CharacterName character, DateTime exactDate, JobCancellationManager jobCancellationManager)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            return provider.TryGetAtTimestamp(exactDate, jobCancellationManager);
        }

        ServerName TryGetCurrentServer(CharacterName character, JobCancellationManager jobCancellationManager)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            return provider.TryGetCurrentServer(jobCancellationManager);
        }

        ServerHistoryProvider GetServerHistoryProvider(CharacterName character)
        {
            lock (locker)
            {
                ServerHistoryProvider provider;
                if (!historyProviders.TryGetValue(character, out provider))
                {
                    provider = serverHistoryProviderFactory.Create(character);
                    historyProviders.Add(character, provider);
                }
                return provider;
            }
        }

        public override void IdleJob(CancellationToken cancellationToken)
        {
            ParsePendingEvents();
        }

        private void ParsePendingEvents()
        {
            foreach (var serverHistoryProvider in historyProviders.Values)
            {
                serverHistoryProvider.ParsePendingLiveLogEvents();
            }

            persistentLibrary.SaveChanged();
        }

        public override TimeSpan IdleJobTreshhold => TimeSpan.FromMilliseconds(100);

        public void BeginTrackingForCharacter(CharacterName name)
        {
            // forcing server history provider to be created
            GetServerHistoryProvider(name);
        }

        /// <summary>
        /// This method checks synchronously, if ServerName is in cache and if so, retrieves the value.
        /// It is used as an optimization.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <returns></returns>
        public ServerName CheckCacheForServerInfo(CharacterName character, DateTime exactDate)
        {
            // this method can be called from arbitrary threads!
            var provider = GetServerHistoryProvider(character);
            var serverName = provider.CheckCache(exactDate);
            return serverName;
        }
    }
}