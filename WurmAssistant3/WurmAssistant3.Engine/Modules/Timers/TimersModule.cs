using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Attributes;
using AldurSoft.WurmAssistant3.Engine.Modules.Timers.Impl;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Modules.Timers;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Timers
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
