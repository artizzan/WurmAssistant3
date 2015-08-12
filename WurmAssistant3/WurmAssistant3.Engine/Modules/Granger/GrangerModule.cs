using AldursLab.WurmAssistant3.Attributes;
using AldursLab.WurmAssistant3.Engine.Modules.Granger.Impl;
using AldursLab.WurmAssistant3.Modules;
using AldursLab.WurmAssistant3.Modules.Granger;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Modules.Granger
{
    [WurmAssistantModule("Granger")]
    public class GrangerModule : ModuleBase, IGrangerModule
    {
        private readonly GrangerDataContext dataContext;

        public GrangerModule(
            ModuleId moduleId,
            IModuleEngine moduleEngine,
            IPersistentManager persistentManager)
            : base(moduleId, moduleEngine, persistentManager)
        {
            this.dataContext = new GrangerDataContext(persistentManager);
        }
    }
}
