using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Modules;
using AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.Custom;
using AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.JunkSale;
using AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.Meditation;
using AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.MeditPath;
using AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.Prayer;
using AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.Sermon;
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
            kernel.Bind<PrayerTimer>().ToSelf();
            kernel.Bind<MeditationTimer>().ToSelf();
            kernel.Bind<CustomTimer>().ToSelf();
            kernel.Bind<JunkSaleTimer>().ToSelf();
            kernel.Bind<MeditPathTimer>().ToSelf();
            kernel.Bind<SermonTimer>().ToSelf();
        }
    }
}
