using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Attributes;
using AldurSoft.WurmAssistant3.Engine.Modules.Sample.Impl;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Modules.Sample;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Sample
{
    [WurmAssistantModule("Sample")]
    public class SampleModule : ModuleBase, ISampleModule
    {
        private readonly SampleDataContext dataContext;

        public SampleModule([NotNull] ModuleId moduleId, [NotNull] IModuleEngine moduleEngine, [NotNull] IPersistentManager persistentManager)
            : base(moduleId, moduleEngine, persistentManager)
        {
            dataContext = new SampleDataContext(persistentManager);
        }
    }
}
