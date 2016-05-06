using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.SkillStats.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.SkillStats
{
    public static class SkillStatsSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<SkillStatsFeature>().InSingletonScope().Named("SkillStats");
        }
    }
}
