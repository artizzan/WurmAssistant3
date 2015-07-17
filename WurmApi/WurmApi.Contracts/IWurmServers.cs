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
        /// <exception cref="WurmApiException">Server with this name is not known to WurmApi.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        IWurmServer TryGetByName(ServerName name);
    }
}