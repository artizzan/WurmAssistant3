namespace AldurSoft.WurmApi
{
    internal interface IWurmApiInternal : IWurmApi
    {
        IWurmServerHistory WurmServerHistory { get; }
    }
}
