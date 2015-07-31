using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;
using AldurSoft.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;

namespace AldurSoft.WurmApi
{
    public interface IWurmApiDataContext
    {
        IPersistentSet<WurmCharacterLogsEntity> WurmCharacterLogs { get; }
        IPersistentSet<ServerHistory> ServerHistory { get; }
        IPersistentSet<ServersData> ServersData { get; }
    }
}
