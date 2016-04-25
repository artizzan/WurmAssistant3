namespace AldursLab.WurmApi.PersistentObjects
{
    abstract class Persistent
    {
        internal abstract string GetSerializedData(ISerializationStrategy serializationStrategy);

        internal abstract bool HasChanged { get; set; }
    }

    class Persistent<TEntity> : Persistent, IPersistent<TEntity> where TEntity : Entity, new()
    {
        public TEntity Entity { get; internal set; }

        bool hasChanged;

        public Persistent(string objectId)
        {
            Entity = new TEntity {ObjectId = objectId};
        }

        internal override string GetSerializedData(ISerializationStrategy serializationStrategy)
        {
            return serializationStrategy.Serialize(Entity);
        }

        internal override bool HasChanged
        {
            get { return hasChanged; }
            set { hasChanged = value; }
        }

        public void FlagAsChanged()
        {
            hasChanged = true;
        }
    }
}
