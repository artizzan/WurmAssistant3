using System;
using System.Threading;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    public interface IWurmServerHistory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        Task<ServerName> GetServerAsync(CharacterName character, DateTime exactDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        Task<ServerName> GetServerAsync(CharacterName character, DateTime exactDate, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        ServerName GetServer(CharacterName character, DateTime exactDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="exactDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        ServerName GetServer(CharacterName character, DateTime exactDate, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        Task<ServerName> GetCurrentServerAsync(CharacterName character);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        Task<ServerName> GetCurrentServerAsync(CharacterName character, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        ServerName GetCurrentServer(CharacterName character);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        ServerName GetCurrentServer(CharacterName character, CancellationToken cancellationToken);
    }
}