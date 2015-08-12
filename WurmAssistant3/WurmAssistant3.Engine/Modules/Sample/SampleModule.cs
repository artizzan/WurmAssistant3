using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Attributes;
using AldursLab.WurmAssistant3.Engine.Modules.Sample.Impl;
using AldursLab.WurmAssistant3.Modules;
using AldursLab.WurmAssistant3.Modules.Sample;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Modules.Sample
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
