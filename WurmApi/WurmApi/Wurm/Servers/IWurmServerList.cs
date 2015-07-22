using System.Collections.Generic;

namespace AldurSoft.WurmApi.Wurm.Servers
{
    public interface IWurmServerList
    {
        IEnumerable<WurmServerInfo> All { get; }

        bool Exists(ServerName serverName);
    }
}