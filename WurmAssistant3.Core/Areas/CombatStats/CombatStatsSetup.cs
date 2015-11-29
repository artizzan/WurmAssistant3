using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats
{
    public static class CombatStatsSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CombatStatsFeature>().InSingletonScope().Named("CombatAssist");
        }
    }
}
