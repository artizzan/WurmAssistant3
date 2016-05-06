using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Timers
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
