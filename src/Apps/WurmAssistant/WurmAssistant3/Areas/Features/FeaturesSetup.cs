using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Features
{
    public static class FeaturesSetup
    {
        public static void BindFeaturesManager(IKernel kernel)
        {
            kernel.Bind<IFeaturesManager, FeaturesManager>().To<FeaturesManager>().InSingletonScope();
        }
    }
}
