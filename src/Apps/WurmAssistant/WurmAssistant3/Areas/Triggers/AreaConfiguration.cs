using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Parts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Triggers
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<TriggersFeature>().InSingletonScope().Named("Triggers");
        }
    }
}