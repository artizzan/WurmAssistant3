using System;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.FlatFiles;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Wurm.LogsMonitor;
using AldurSoft.WurmApi.Modules.Wurm.ServerHistory.Jobs;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.ServerHistory
{
    class WurmServerHistory : IWurmServerHistory
    {
        readonly QueuedJobsSyncRunner<object, ServerName> runner;

        public WurmServerHistory(
            string dataDirectoryFullPath,
            IWurmLogsHistory wurmLogsHistory,
            IWurmServerList wurmServerList,
            ILogger logger,
            IWurmLogsMonitorInternal wurmLogsMonitor,
            IWurmLogFiles wurmLogFiles)
        {
            var persistentLibrary =
                new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(dataDirectoryFullPath),
                    new PersObjErrorHandlingStrategy(logger));
            var collection = persistentLibrary.GetCollection("serverhistory");

            var providerFactory = new ServerHistoryProviderFactory(
                collection,
                wurmLogsHistory,
                wurmServerList,
                logger,
                wurmLogsMonitor,
                wurmLogFiles);

            runner = new QueuedJobsSyncRunner<object, ServerName>(new JobExecutor(providerFactory, persistentLibrary), logger);
        }

        public async Task<ServerName> GetServerAsync(CharacterName character, DateTime exactDate)
        {
            return await runner.Run(new GetServerAtDateJob(character, exactDate), CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ServerName> GetServerAsync(CharacterName character, DateTime exactDate, CancellationToken cancellationToken)
        {
            return await runner.Run(new GetServerAtDateJob(character, exactDate), cancellationToken).ConfigureAwait(false);
        }

        public ServerName GetServer(CharacterName character, DateTime exactDate)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetServerAsync(character, exactDate).Result);
        }

        public ServerName GetServer(CharacterName character, DateTime exactDate, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetServerAsync(character, exactDate, cancellationToken).Result);
        }

        public async Task<ServerName> GetCurrentServerAsync(CharacterName character)
        {
            return await runner.Run(new GetCurrentServerJob(character), CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ServerName> GetCurrentServerAsync(CharacterName character, CancellationToken cancellationToken)
        {
            return await runner.Run(new GetCurrentServerJob(character), cancellationToken).ConfigureAwait(false);
        }

        public ServerName GetCurrentServer(CharacterName character)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetCurrentServerAsync(character).Result);
        }

        public ServerName GetCurrentServer(CharacterName character, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetCurrentServerAsync(character, cancellationToken).Result);
        }
    }
}
