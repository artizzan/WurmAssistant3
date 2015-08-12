using AldursLab.WurmAssistant3.Attributes;
using AldursLab.WurmAssistant3.Engine.Modules.Timers.Impl;
using AldursLab.WurmAssistant3.Modules;
using AldursLab.WurmAssistant3.Modules.Timers;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Modules.Timers
{
    [WurmAssistantModule("Timers")]
    public class TimersModule : ModuleBase, ITimersModule
    {
        private readonly TimersDataContext dataContext;

        public TimersModule(
            ModuleId moduleId,
            IModuleEngine moduleEngine,
            IPersistentManager persistentManager)
            : base(moduleId, moduleEngine, persistentManager)
        {
            this.dataContext = new TimersDataContext(persistentManager);
        }
    }
}
