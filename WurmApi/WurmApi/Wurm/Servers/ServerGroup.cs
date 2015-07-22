namespace AldurSoft.WurmApi.Wurm.Servers
{
    public abstract class ServerGroup
    {
        public abstract ServerGroupId ServerGroupId { get; }
    }

    public enum ServerGroupId
    {
        Unknown = 0,
        Freedom = 1,
        Epic = 2,
        Challenge = 3
    }
}
