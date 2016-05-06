using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.RevealCreatures.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.RevealCreatures
{
    public static class RevealCreaturesSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<RevealCreaturesFeature>().InSingletonScope().Named("RevealCreatures");
        }
    }
}
