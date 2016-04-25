using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Encapsulates Wurm Online server groups.
    /// </summary>
    public interface IWurmServerGroups
    {
        /// <summary>
        /// Returns all Wurm Online server groups defined in WurmApi.
        /// </summary>
        IEnumerable<ServerGroup> AllKnown { get; }

        /// <summary>
        /// Gets server group for specific server.
        /// If server is not mapped to any server group, a server scoped group will be registered.
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        ServerGroup GetForServer(ServerName serverName);
    }
}
