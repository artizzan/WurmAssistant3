namespace AldursLab.PersistentObjects.Serialization
{
    public interface ISerializationStrategy
    {
        void PopulateFromSerialized<T>(T @object, string data) where T : class;

        string Serialize<T>(T @object) where T : class;
    }
}