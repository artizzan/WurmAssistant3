using AldursLab.WurmAssistant3.Areas.CraftingAssistant.Features;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.CraftingAssistant
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CraftingAssistantFeature>().InSingletonScope().Named("CraftingAssistant");
        }
    }
}