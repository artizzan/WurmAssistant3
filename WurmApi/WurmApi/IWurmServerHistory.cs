using System;
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
        Task<ServerName> GetServer(CharacterName character, DateTime exactDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        Task<ServerName> GetCurrentServer(CharacterName character);
    }
}