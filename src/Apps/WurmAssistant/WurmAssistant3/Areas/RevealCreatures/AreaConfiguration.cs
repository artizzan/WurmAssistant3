using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.RevealCreatures.Features;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.RevealCreatures
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<RevealCreaturesFeature>().InSingletonScope().Named("RevealCreatures");
        }
    }
}