namespace AldursLab.WurmApi.PersistentObjects
{
    interface IPersistent<out TEntity> where TEntity : Entity, new()
    {
        TEntity Entity { get; }
        void FlagAsChanged();
    }
}