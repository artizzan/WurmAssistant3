namespace AldursLab.WurmApi.PersistentObjects
{
    interface IPersistentCollectionsLibrary
    {
        IPersistentCollection GetCollection(string collectionId);
        IPersistentCollection DefaultCollection { get; }
    }
}