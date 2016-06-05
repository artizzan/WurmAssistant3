using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Features;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Timers
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<TimersFeature>().InSingletonScope().Named("Timers");
        }
    }
}