using AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssistant
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
