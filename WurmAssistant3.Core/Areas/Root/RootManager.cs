using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Eventing;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using AldursLab.WurmAssistant3.Core.Areas.Logging.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.Root.Model;
using AldursLab.WurmAssistant3.Core.Areas.Root.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.Root.Views;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Root
{
    public static class RootManager
    {
        public static void BindCoreComponents(IKernel kernel)
        {
            var mainView = kernel.Get<MainView>();
            var threadMarshaller = new WinFormsThreadMarshaller(mainView);
            kernel.Bind<IEventMarshaller, IWurmApiEventMarshaller>().ToConstant(threadMarshaller);
            kernel.Bind<IHostEnvironment>().ToConstant(mainView);
            kernel.Bind<ITimerService>().ToConstant(mainView);
        }
    }
}
