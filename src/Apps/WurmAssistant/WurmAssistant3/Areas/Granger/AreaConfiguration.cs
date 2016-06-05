using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Granger.Features;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<GrangerFeature>().InSingletonScope().Named("Granger");
        }
    }
}