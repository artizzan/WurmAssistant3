using AldursLab.PersistentObjects;

namespace AldursLab.WurmAssistant3.Core.Areas.PersistentData.Model
{
    public interface IPersistentFactory
    {
        IPersistent<T> Get<T>() where T : Entity, new();
    }
}