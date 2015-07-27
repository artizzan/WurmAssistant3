using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    public interface IWurmServerList
    {
        IEnumerable<WurmServerInfo> All { get; }

        bool Exists(ServerName serverName);
    }
}