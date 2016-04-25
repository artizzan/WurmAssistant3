using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Provides means of obtaining wurm servers information.
    /// </summary>
    public interface IWurmServers
    {
        /// <summary>
        /// Enumerates all servers registered by WurmApi.
        /// </summary>
        IEnumerable<IWurmServer> All { get; }

        /// <summary>
        /// Gets server by its name, case insensitive.
        /// If WurmApi does not know this server, a new server will be created with a server group of Unknown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IWurmServer GetByName(ServerName name);

        /// <summary>
        /// Gets server by its name, case insensitive.
        /// If WurmApi does not know this server, a new server will be created with a server group of Unknown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IWurmServer GetByName(string name);
    }
}