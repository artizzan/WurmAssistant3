using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.DataModel.LogsHistoryModel;
using AldurSoft.WurmApi.DataModel.ServerHistoryModel;
using AldurSoft.WurmApi.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi
{
    public interface IWurmApiDataContext
    {
        IPersistentSet<WurmCharacterLogsEntity> WurmCharacterLogs { get; }
        IPersistentSet<ServerHistory> ServerHistory { get; }
        IPersistentSet<ServersData> ServersData { get; }
    }
}
