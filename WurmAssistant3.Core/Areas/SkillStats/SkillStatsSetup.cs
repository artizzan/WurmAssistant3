using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SkillStats.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.SkillStats
{
    public static class SkillStatsSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<SkillStatsFeature>().InSingletonScope().Named("SkillStats");
        }
    }
}
