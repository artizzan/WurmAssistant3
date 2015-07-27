using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.LogsHistoryModel;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.ServerHistoryModel;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi
{
    public interface IWurmApiDataContext
    {
        IPersistentSet<WurmCharacterLogsEntity> WurmCharacterLogs { get; }
        IPersistentSet<ServerHistory> ServerHistory { get; }
        IPersistentSet<ServersData> ServersData { get; }
    }
}
