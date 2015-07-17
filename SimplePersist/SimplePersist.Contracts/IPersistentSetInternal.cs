namespace AldurSoft.SimplePersist
{
    public interface IPersistentSetInternal<out TEntity> : IPersistentSet<TEntity>
        where TEntity : class, new()
    {
        IPersistentManagerInternal PersistentManager { get; }
    }
}