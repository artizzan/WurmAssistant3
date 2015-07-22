using AldurSoft.WurmApi.Wurm.CharacterServers;

namespace AldurSoft.WurmApi.Infrastructure
{
    internal interface IWurmApiInternal : IWurmApi
    {
        IWurmServerHistory WurmServerHistory { get; }
    }
}
