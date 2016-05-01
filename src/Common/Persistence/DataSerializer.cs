namespace AldursLab.Persistence
{
    public abstract class DataSerializer
    {
        public abstract string Serialize<T>(T @object) where T : class;

        public abstract T Deserialize<T>(string data) where T : class;
    }
}