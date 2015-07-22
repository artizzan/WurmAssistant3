using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Persistence.DataModel.LogsHistoryModel;
using AldurSoft.WurmApi.Persistence.DataModel.ServerHistoryModel;
using AldurSoft.WurmApi.Persistence.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi.Persistence
{
    public interface IWurmApiDataContext
    {
        IPersistentSet<WurmCharacterLogsEntity> WurmCharacterLogs { get; }
        IPersistentSet<ServerHistory> ServerHistory { get; }
        IPersistentSet<ServersData> ServersData { get; }
    }
}
