using System;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Wurm.DateAndTime;

namespace AldurSoft.WurmApi.Wurm.Servers
{
    /// <summary>
    /// Provides means of obtaining wurm server-specific information.
    /// </summary>
    public interface IWurmServer
    {
        /// <summary>
        /// This server name.
        /// </summary>
        ServerName ServerName { get; }

        /// <summary>
        /// Server group for this server.
        /// </summary>
        ServerGroup ServerGroup { get; }

        /// <summary>
        /// Returns current server game time or null if no data available.
        /// </summary>
        Task<WurmDateTime?> TryGetCurrentTime();

        /// <summary>
        /// Returns current server uptime or null if no data available.
        /// </summary>
        Task<TimeSpan?> TryGetCurrentUptime();
    }
}