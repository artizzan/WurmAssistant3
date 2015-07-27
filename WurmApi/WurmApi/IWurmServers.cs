using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
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
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="DataNotFoundException">Server with this name has not been found.</exception>
        /// <returns></returns>
        IWurmServer GetByName(ServerName name);
    }
}