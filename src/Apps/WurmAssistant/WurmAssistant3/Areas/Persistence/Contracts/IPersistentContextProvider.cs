using AldursLab.Persistence;
using AldursLab.Persistence.Simple;

namespace AldursLab.WurmAssistant3.Areas.Persistence.Contracts
{
    public  interface IPersistentContextProvider
    {
        IPersistentContext GetPersistentContext(string contextId, PersistentContextOptions options);
        IPersistentContext GetPersistentContext(string contextId);
    }
}