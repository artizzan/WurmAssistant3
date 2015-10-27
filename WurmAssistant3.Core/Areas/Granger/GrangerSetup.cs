using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.Advisor.Default;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger
{
    public static class GrangerSetup
    {
        public static void BindGranger(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<GrangerFeature>().InSingletonScope().Named("Granger");
            kernel.Bind<GrangerSettings>().ToSelf().InSingletonScope();
            kernel.Bind<DefaultBreedingEvaluatorOptions>().ToSelf().InSingletonScope();
        }
    }
}
