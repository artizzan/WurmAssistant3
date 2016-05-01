using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers
{
    public static class TimersSetup
    {
        public static void BindTimers(IKernel kernel)
        {
            kernel.Bind<TimerTypes>().ToSelf().InSingletonScope();
            kernel.Bind<TimerDefinitions>().ToSelf().InSingletonScope();
            kernel.Bind<IFeature>().To<TimersFeature>().InSingletonScope().Named("Timers");
            kernel.Bind<TimerInstances>().ToSelf().InSingletonScope();
        }
    }
}
