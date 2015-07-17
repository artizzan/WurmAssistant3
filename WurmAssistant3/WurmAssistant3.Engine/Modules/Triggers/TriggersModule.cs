using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Attributes;
using AldurSoft.WurmAssistant3.Engine.Modules.Triggers.Impl;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Modules.Triggers;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Triggers
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
