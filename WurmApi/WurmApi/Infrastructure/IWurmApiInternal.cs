namespace AldurSoft.WurmApi.Infrastructure
{
    internal interface IWurmApiInternal : IWurmApi
    {
        IWurmServerHistory WurmServerHistory { get; }
    }
}
