namespace AldursLab.PersistentObjects.Persistence
{
    public class RawDataObject
    {
        public string CollectionId { get; }
        public string Key { get; }
        public string Content { get; }

        public RawDataObject(string collectionId, string key, string content)
        {
            CollectionId = collectionId;
            Key = key;
            Content = content;
        }
    }
}