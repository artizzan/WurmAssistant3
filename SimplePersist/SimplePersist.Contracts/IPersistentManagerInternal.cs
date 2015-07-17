namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Internal manager interface.
    /// </summary>
    public interface IPersistentManagerInternal : IPersistentManager
    {
        IPersistenceStrategy PersistenceStrategy { get; }
        ISerializationStrategy SerializationStrategy { get; }
        ISimplePersistLogger Logger { get; }
    }
}