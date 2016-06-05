using AldursLab.WurmAssistant3.Areas.CraftingAssistant.Features;
using AldursLab.WurmAssistant3.Areas.CraftingAssistant.Singletons;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.CraftingAssistant
{

    public static class CraftingAssistantSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CraftingAssistantFeature>().InSingletonScope().Named("CraftingAssistant");
            kernel.Bind<CraftingAssistantForm>().ToSelf().InSingletonScope();
        }
    }
}
