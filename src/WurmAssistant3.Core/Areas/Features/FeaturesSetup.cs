using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Features.Modules;
using Ninject;
using Ninject.Extensions.Factory;

namespace AldursLab.WurmAssistant3.Core.Areas.Features
{
    public static class FeaturesSetup
    {
        public static void BindFeaturesManager(IKernel kernel)
        {
            kernel.Bind<IFeaturesManager, FeaturesManager>().To<FeaturesManager>().InSingletonScope();
        }
    }
}
