using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Attributes;
using AldursLab.WurmAssistant3.Engine.Modules.Triggers.Impl;
using AldursLab.WurmAssistant3.Modules;
using AldursLab.WurmAssistant3.Modules.Triggers;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Modules.Triggers
{
    [WurmAssistantModule("Triggers")]
    public class TriggersModule : ModuleBase, ITriggersModule
    {
        private readonly TriggersDataContext dataContext;

        public TriggersModule(
            ModuleId moduleId,
            IModuleEngine moduleEngine,
            [NotNull] IPersistentManager persistentManager)
            : base(moduleId, moduleEngine, persistentManager)
        {
            this.dataContext = new TriggersDataContext(persistentManager);
        }
    }
}
