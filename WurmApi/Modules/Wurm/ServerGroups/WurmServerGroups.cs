using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmApi.Modules.Wurm.ServerGroups
{
    class WurmServerGroups : IWurmServerGroups
    {
        readonly Dictionary<ServerName, ServerGroup> serverNameToServerGroupMap = new Dictionary<ServerName, ServerGroup>();
        readonly object locker = new object();

        public WurmServerGroups(IDictionary<ServerName, WurmServerInfo> definedServerGroups)
        {
            foreach (var initialServerGroup in definedServerGroups)
            {
                serverNameToServerGroupMap.Add(initialServerGroup.Key, initialServerGroup.Value.ServerGroup);
            }
        }

        public IEnumerable<ServerGroup> AllKnown
        {
            get
            {
                lock (locker)
                {
                    return serverNameToServerGroupMap.Values.Distinct().ToArray();
                }
            }
        }

        public ServerGroup GetForServer(ServerName serverName)
        {
            lock (locker)
            {
                ServerGroup sg;
                if (!serverNameToServerGroupMap.TryGetValue(serverName, out sg))
                {
                    sg = ServerGroup.CreateServerScoped(serverName);
                    serverNameToServerGroupMap.Add(serverName, sg);
                }
                return sg;
            }
        }
    }
}