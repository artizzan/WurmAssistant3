using AldurSoft.SimplePersist;
using AldurSoft.SimplePersist.Persistence.FlatFiles;
using AldurSoft.SimplePersist.Serializers.JsonNet;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.LogsHistoryModel;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.ServerHistoryModel;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.DataContext
{
    public class WurmApiDataContext : IWurmApiDataContext
    {
        public WurmApiDataContext(string dataStoreDirectoryFullPath, ISimplePersistLogger logger)
        {
            PersistentManager persistenceManager = new PersistentManager(
                new JsonSerializationStrategy(),
                new FlatFilesPersistenceStrategy(dataStoreDirectoryFullPath),
                logger);

            WurmCharacterLogs =
                persistenceManager.GetPersistentCollection<WurmCharacterLogsEntity>(
                    new EntityName("WurmCharacterLogsEntity"));

            ServerHistory =
                persistenceManager.GetPersistentCollection<ServerHistory>(new EntityName("ServerHistory"));

            ServersData = persistenceManager.GetPersistentCollection<ServersData>(new EntityName("ServersData"));
        }

        public IPersistentSet<WurmCharacterLogsEntity> WurmCharacterLogs { get; private set; }
        public IPersistentSet<ServerHistory> ServerHistory { get; private set; }
        public IPersistentSet<ServersData> ServersData { get; private set; }
    }
}
