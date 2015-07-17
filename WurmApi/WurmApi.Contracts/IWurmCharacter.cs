using System;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means of obtaining wurm character-specific details.
    /// </summary>
    public interface IWurmCharacter
    {
        /// <summary>
        /// Returns exact server, that the player was on at given stamp.
        /// </summary>
        /// <exception cref="WurmApiException">Server could not be obtained.</exception>
        Task<IWurmServer> GetHistoricServerAtLogStamp(DateTime stamp);

        /// <summary>
        /// Returns exact server, that the player is currently on.
        /// </summary>
        /// <exception cref="WurmApiException">Server could not be obtained.</exception>
        Task<IWurmServer> GetCurrentServer();

        /// <summary>
        /// Game name of this character.
        /// </summary>
        CharacterName Name { get; }

        /// <summary>
        /// Current config for this character.
        /// </summary>
        IWurmConfig CurrentConfig { get; }
    }
}