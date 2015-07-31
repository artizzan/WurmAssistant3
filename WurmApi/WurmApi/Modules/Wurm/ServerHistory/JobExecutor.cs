using System;
using System.Collections.Generic;
using System.Threading;
using AldursLab.PersistentObjects;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory;
using AldurSoft.WurmApi.Modules.Wurm.ServerHistory.Jobs;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.ServerHistory
{
    class JobExecutor : JobExecutor<object, ServerName>
    {
        readonly ServerHistoryProviderFactory serverHistoryProviderFactory;
        readonly PersistentCollectionsLibrary persistentLibrary;
        readonly Dictionary<CharacterName, ServerHistoryProvider> historyProviders = new Dictionary<CharacterName, ServerHistoryProvider>();

        public JobExecutor([NotNull] ServerHistoryProviderFactory serverHistoryProviderFactory,
            [NotNull] PersistentCollectionsLibrary persistentLibrary)
        {
            if (serverHistoryProviderFactory == null) throw new ArgumentNullException("serverHistoryProviderFactory");
            if (persistentLibrary == null) throw new ArgumentNullException("persistentLibrary");
            this.serverHistoryProviderFactory = serverHistoryProviderFactory;
            this.persistentLibrary = persistentLibrary;
        }

        public override ServerName Execute([NotNull] object jobContext,
            [NotNull] JobCancellationManager jobCancellationManager)
        {
            if (jobContext == null) throw new ArgumentNullException("jobContext");
            if (jobCancellationManager == null) throw new ArgumentNullException("jobCancellationManager");

            var getServerAtDateJob = jobContext as GetServerAtDateJob;
            if (getServerAtDateJob != null)
            {
                return GetServer(getServerAtDateJob.CharacterName, getServerAtDateJob.DateTime, jobCancellationManager);
            }
            var getCurrentServerJob = jobContext as GetCurrentServerJob;
            if (getCurrentServerJob != null)
            {
                return GetCurrentServer(getCurrentServerJob.CharacterName, jobCancellationManager);
            }

            persistentLibrary.SaveChanged();

            throw new InvalidOperationException("No handler specified for job type: " + jobContext.GetType());
        }

        ServerName GetServer(CharacterName character, DateTime exactDate, JobCancellationManager jobCancellationManager)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            var result = provider.TryGetAtTimestamp(exactDate, jobCancellationManager);
            if (result == null)
            {
                throw new DataNotFoundException(
                    string.Format("Server not found for timestamp {0} and character name {1}", exactDate, character));
            }
            return result;
        }

        ServerName GetCurrentServer(CharacterName character, JobCancellationManager jobCancellationManager)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            var result = provider.TryGetCurrentServer(jobCancellationManager);
            if (result == null)
            {
                throw new DataNotFoundException(
                    string.Format("Current server not found for character name {0}", character));
            }
            return result;
        }

        ServerHistoryProvider GetServerHistoryProvider(CharacterName character)
        {
            ServerHistoryProvider provider;
            if (!historyProviders.TryGetValue(character, out provider))
            {
                provider = serverHistoryProviderFactory.Create(character);
                historyProviders.Add(character, provider);
            }
            return provider;
        }

        public override void IdleJob(CancellationToken cancellationToken)
        {
            foreach (var serverHistoryProvider in historyProviders.Values)
            {
                serverHistoryProvider.ParsePendingLiveLogEvents();
            }

            persistentLibrary.SaveChanged();
        }

        public override TimeSpan IdleJobTreshhold { get { return TimeSpan.FromSeconds(5); } }
    }
}