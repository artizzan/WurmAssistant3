using System;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Servers;

namespace AldurSoft.WurmApi.Wurm.CharacterServers
{
    public interface IWurmServerHistory
    {
        Task<ServerName> TryGetServer(CharacterName character, DateTime exactDate);

        Task<ServerName> TryGetCurrentServer(CharacterName character);
    }
}