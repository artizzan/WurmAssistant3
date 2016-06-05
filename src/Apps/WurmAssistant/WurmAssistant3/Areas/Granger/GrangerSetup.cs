using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Granger.Features;
using AldursLab.WurmAssistant3.Areas.Granger.Singletons;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public static class GrangerSetup
    {
        public static void BindGranger(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<GrangerFeature>().InSingletonScope().Named("Granger");
            kernel.Bind<GrangerSettings>().ToSelf().InSingletonScope();
            kernel.Bind<DefaultBreedingEvaluatorOptions>().ToSelf().InSingletonScope();
            kernel.Bind<GrangerSimpleDb>().ToSelf().InSingletonScope();
        }
    }
}
