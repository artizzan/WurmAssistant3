using AldursLab.WurmAssistant3.Areas.Calendar.Modules;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Calendar
{
    public static class CalendarSetup
    {
        public static void BindCalendar(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CalendarFeature>().InSingletonScope().Named("Calendar");
            kernel.Bind<WurmSeasonsManager>().ToSelf().InSingletonScope();
        }
    }
}
