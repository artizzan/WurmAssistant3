using AldursLab.WurmAssistant3.Modules;

namespace AldursLab.WurmAssistant3.Systems
{
    public interface IModuleEngine
    {
        void RegisterModule(IModule module);
    }
}
