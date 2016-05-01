using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.CraftingAssistant.Modules;
using AldursLab.WurmAssistant3.Core.Areas.CraftingAssistant.Views;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.CraftingAssistant
{

    public static class CraftingAssistantSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CraftingAssistantFeature>().InSingletonScope().Named("CraftingAssistant");
            kernel.Bind<CraftingAssistantView>().ToSelf().InSingletonScope();
        }
    }
}
