using AldursLab.Persistence.Simple;

namespace AldursLab.WurmAssistant3.Areas.Persistence.Contracts
{
    public  interface IPersistentContextProvider
    {
        PersistentContext GetPersistentContext(string contextId, PersistentContextOptions options);
    }
}