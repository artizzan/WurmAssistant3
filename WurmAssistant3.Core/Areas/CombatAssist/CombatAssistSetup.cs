using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.CombatAssist.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssist
{
    public static class CombatAssistSetup
    {
        public static void Bind(IKernel kernel)
        {
            //kernel.Bind<IFeature>().To<CombatAssistFeature>().InSingletonScope().Named("CombatAssist");
        }
    }
}
