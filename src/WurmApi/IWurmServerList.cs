using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Provides information about all servers registered in WurmApi.
    /// </summary>
    public interface IWurmServerList
    {
        /// <summary>
        /// List of all servers currently registered in WurmApi.
        /// </summary>
        IEnumerable<WurmServerInfo> All { get; }

        /// <summary>
        /// Checks if given server name is registered.
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        bool Exists(ServerName serverName);
    }
}