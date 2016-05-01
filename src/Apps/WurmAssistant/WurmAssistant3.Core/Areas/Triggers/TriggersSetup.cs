using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Contracts.ActionQueueParsing;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.ActionQueueParsing;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers
{
    public static class TriggersSetup
    {
        public static void BindTriggers(IKernel kernel)
        {
            kernel.Bind<ConditionsManager, IActionQueueConditions>().To<ConditionsManager>().InSingletonScope();
            kernel.Bind<IFeature>().To<TriggersFeature>().InSingletonScope().Named("Triggers");
        }
    }
}
