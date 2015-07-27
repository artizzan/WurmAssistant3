namespace AldurSoft.WurmApi.Infrastructure
{
    /// <summary>
    /// Object must be periodically refreshed to keep its internal data in sync.
    /// </summary>
    internal interface IRequireRefresh
    {
        void Refresh();
    }
}
