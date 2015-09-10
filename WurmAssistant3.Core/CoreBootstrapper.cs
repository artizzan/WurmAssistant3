using System;
using System.IO;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using AldursLab.WurmAssistant3.Core.Areas.Logging;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Model;
using AldursLab.WurmAssistant3.Core.Areas.Logging.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.PersistentData;
using AldursLab.WurmAssistant3.Core.Areas.Root;
using AldursLab.WurmAssistant3.Core.Areas.Root.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.Root.Views;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Core
{
    public class CoreBootstrapper : IDisposable
    {
        readonly IKernel kernel = new StandardKernel();

        public CoreBootstrapper([NotNull] MainView mainView, [NotNull] MainViewModel mainViewModel)
        {
            if (mainView == null) throw new ArgumentNullException("mainView");
            if (mainViewModel == null) throw new ArgumentNullException("mainViewModel");
            kernel.Bind<MainView>().ToConstant(mainView);
            kernel.Bind<MainViewModel>().ToConstant(mainViewModel);
        }

        public void BootstrapCore()
        {
            RootManager.BindCoreComponents(kernel);

            ConfigManager.BindDataDirectory(kernel);
            ConfigManager.LockDataDirectory();
        }

        public void BootstrapRuntime()
        {
            LoggingManager.BindLoggingFactories(kernel);
            LoggingManager.BindLoggerAutoResolver(kernel);
            LoggingManager.EnableLoggingView(kernel);

            PersistentDataManager.BindPersistentLibrary(kernel);
            ConfigManager.BindApplicationSettings(kernel);

            WurmApiManager.AutodetectInstallDirectory(kernel);

            // validate configuration and show configuration dialogs if necessary
            ConfigManager.ConfigureApplication(kernel);

            WurmApiManager.BindSingletonApi(kernel);

            //var moduleManager = new ModuleManager(new[]
            //{
            //    new LogSearcher(kernel.Get<ILogSearcherModuleGui>()), 
            //});
            //kernel.Bind<ModuleManager>().ToConstant(moduleManager);
        }

        public void Dispose()
        {
            ConfigManager.UnlockDataDirectory();
            kernel.Dispose();
        }

        public void RunAsyncInits()
        {
            // todo
        }
    }
}
