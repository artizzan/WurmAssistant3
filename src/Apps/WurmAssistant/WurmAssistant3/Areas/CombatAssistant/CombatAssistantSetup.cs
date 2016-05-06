using AldursLab.WurmAssistant3.Areas.CombatAssistant.Modules;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant
{
    public static class CombatAssistantSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CombatAssistantFeature>().InSingletonScope().Named("CombatAssistant");
            kernel.Bind<FeatureSettings>().ToSelf().InSingletonScope();
        }
    }
}
