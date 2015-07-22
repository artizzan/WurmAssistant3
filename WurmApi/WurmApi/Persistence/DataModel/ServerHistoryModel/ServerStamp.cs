using System;
using AldurSoft.WurmApi.Wurm.Servers;

namespace AldurSoft.WurmApi.Persistence.DataModel.ServerHistoryModel
{
    public class ServerStamp
    {
        public DateTime Timestamp { get; set; }
        public ServerName ServerName { get; set; }
    }
}