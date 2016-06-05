using AldursLab.WurmAssistant3.Areas.CombatAssistant.Parts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CombatAssistantFeature>().InSingletonScope().Named("CombatAssistant");
        }
    }
}