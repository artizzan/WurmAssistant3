using System;
using System.Threading;
using System.Threading.Tasks;

namespace AldursLab.WurmApi
{
    public interface IWurmServerHistory
    {
        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <returns></returns>
        Task<ServerName> TryGetServerAsync(CharacterName character, DateTime exactDate);

        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ServerName> TryGetServerAsync(CharacterName character, DateTime exactDate, CancellationToken cancellationToken);

        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <returns></returns>
        ServerName TryGetServer(CharacterName character, DateTime exactDate);

        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ServerName TryGetServer(CharacterName character, DateTime exactDate, CancellationToken cancellationToken);

        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        Task<ServerName> TryGetCurrentServerAsync(CharacterName character);

        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ServerName> TryGetCurrentServerAsync(CharacterName character, CancellationToken cancellationToken);

        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        ServerName TryGetCurrentServer(CharacterName character);

        /// <summary>
        /// Returns null if not found.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ServerName TryGetCurrentServer(CharacterName character, CancellationToken cancellationToken);

        /// <summary>
        /// Informs WurmServerHistory, that it should start tracking live logs for potential current server changes.
        /// </summary>
        /// <param name="name"></param>
        void BeginTracking(CharacterName name);
    }
}