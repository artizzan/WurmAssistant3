using System;
using System.Threading;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
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

        #region TryGetCurrentTime

        /// <summary>
        /// Returns current server game time or null if no data available.
        /// </summary>
        Task<WurmDateTime?> TryGetCurrentTimeAsync();

        /// <summary>
        /// Returns current server game time or null if no data available.
        /// </summary>
        WurmDateTime? TryGetCurrentTime();

        /// <summary>
        /// Returns current server game time or null if no data available.
        /// </summary>
        Task<WurmDateTime?> TryGetCurrentTimeAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns current server game time or null if no data available.
        /// </summary>
        WurmDateTime? TryGetCurrentTime(CancellationToken cancellationToken);

        #endregion

        #region TryGetCurrentUptime

        /// <summary>
        /// Returns current server uptime or null if no data available.
        /// </summary>
        Task<TimeSpan?> TryGetCurrentUptimeAsync();

        /// <summary>
        /// Returns current server uptime or null if no data available.
        /// </summary>
        TimeSpan? TryGetCurrentUptime();

        /// <summary>
        /// Returns current server uptime or null if no data available.
        /// </summary>
        Task<TimeSpan?> TryGetCurrentUptimeAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns current server uptime or null if no data available.
        /// </summary>
        TimeSpan? TryGetCurrentUptime(CancellationToken cancellationToken);

        #endregion
    }
}