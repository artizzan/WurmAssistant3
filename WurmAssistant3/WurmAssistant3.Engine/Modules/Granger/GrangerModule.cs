using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Attributes;
using AldurSoft.WurmAssistant3.Engine.Modules.Granger.Impl;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Modules.Granger;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Granger
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
