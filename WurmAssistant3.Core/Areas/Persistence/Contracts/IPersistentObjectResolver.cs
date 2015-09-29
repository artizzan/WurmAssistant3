using AldursLab.PersistentObjects;

namespace AldursLab.WurmAssistant3.Core.Areas.Persistence.Contracts
{
    public interface IPersistentObjectResolver<T> where T : class, IPersistentObject
    {
        T Get(string persistentObjectId);

        void Unload(T @object);
    }
}
