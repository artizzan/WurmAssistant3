namespace AldurSoft.Core.AppFramework.Repositories
{
    interface IRepository
    {
    }

    interface IRepository<TEntity, in TKey> : IRepository where TEntity : class
    {
        TEntity GetById(TKey id);

        void Add(TEntity entity);

        void Remove(TEntity entity);

        void Update(TEntity entity);
    }
}