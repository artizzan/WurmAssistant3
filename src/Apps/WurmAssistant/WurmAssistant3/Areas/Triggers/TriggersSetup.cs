using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Contracts.ActionQueueParsing;
using AldursLab.WurmAssistant3.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules.ActionQueueParsing;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Triggers
{
    public static class TriggersSetup
    {
        public static void BindTriggers(IKernel kernel)
        {
            kernel.Bind<ConditionsManager, IActionQueueConditions>().To<ConditionsManager>().InSingletonScope();
            kernel.Bind<IFeature>().To<TriggersFeature>().InSingletonScope().Named("Triggers");
            kernel.Bind<TriggerManager>().ToSelf();
            kernel.Bind<ActiveTriggers>().ToSelf();
        }
    }
}
