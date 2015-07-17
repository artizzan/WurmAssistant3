using System;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    public interface IWurmServerHistory
    {
        Task<ServerName> TryGetServer(CharacterName character, DateTime exactDate);

        Task<ServerName> TryGetCurrentServer(CharacterName character);
    }
}