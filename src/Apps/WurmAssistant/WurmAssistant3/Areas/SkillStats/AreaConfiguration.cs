using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.SkillStats.Features;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.SkillStats
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<SkillStatsFeature>().InSingletonScope().Named("SkillStats");
        }
    }
}