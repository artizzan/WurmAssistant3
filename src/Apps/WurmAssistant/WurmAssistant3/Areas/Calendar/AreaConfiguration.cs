using AldursLab.WurmAssistant3.Areas.Calendar.Features;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Calendar
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CalendarFeature>().InSingletonScope().Named("Calendar");
        }
    }
}