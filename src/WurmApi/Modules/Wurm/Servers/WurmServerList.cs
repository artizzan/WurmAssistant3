using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class WurmServerList : IWurmServerList
    {
        readonly IReadOnlyCollection<WurmServerInfo> defaultDescriptions;

        public WurmServerList(IDictionary<ServerName, WurmServerInfo> serversMap)
        {
            defaultDescriptions = serversMap.Select(pair => pair.Value).ToArray();
        }

        public virtual IEnumerable<WurmServerInfo> All => defaultDescriptions;

        public bool Exists(ServerName serverName)
        {
            return defaultDescriptions.Any(info => info.ServerName == serverName);
        }
    }
}