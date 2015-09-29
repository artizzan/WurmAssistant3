using System;
using System.Linq;
using AldursLab.Essentials.Eventing;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config;
using AldursLab.WurmAssistant3.Core.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Config.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Features;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Features.Views;
using AldursLab.WurmAssistant3.Core.Areas.Logging;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Views;
using AldursLab.WurmAssistant3.Core.Areas.LogSearcher;
using AldursLab.WurmAssistant3.Core.Areas.MainMenu;
using AldursLab.WurmAssistant3.Core.Areas.MainMenu.Views;
using AldursLab.WurmAssistant3.Core.Areas.Persistence;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi;
using AldursLab.WurmAssistant3.Core.IoC;
using AldursLab.WurmAssistant3.Core.Root.Components;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;
using Ninject;
using Ninject.Activation.Strategies;

namespace AldursLab.WurmAssistant3.Core.Root
{
    public class CoreBootstrapper : IDisposable
    {
        readonly IKernel kernel = new StandardKernel();

        readonly MainForm mainForm;
        readonly WurmAssistantDataDirectory dataDirectory;

        public CoreBootstrapper([NotNull] MainForm mainForm)
        {
            if (mainForm == null) throw new ArgumentNullException("mainForm");
            this.mainForm = mainForm;

            SetupActivationStrategyComponents();
            
            dataDirectory = new WurmAssistantDataDirectory();
            dataDirectory.Lock();
            kernel.ProhibitGet<WurmAssistantDataDirectory>();

            kernel.Bind<WurmAssistantConfig, IWurmAssistantConfig>().To<WurmAssistantConfig>().InSingletonScope();
            kernel.Bind<WurmAssistantDataDirectory, IWurmAssistantDataDirectory>().ToConstant(dataDirectory);
            kernel.Bind<MainForm>().ToConstant(mainForm);

            kernel.Bind<IThreadMarshaller, IWurmApiEventMarshaller>().ToConstant(new WinFormsThreadMarshaller(mainForm));
            kernel.Bind<IHostEnvironment>().ToConstant(mainForm);
            kernel.Bind<IUpdateLoop>().ToConstant(mainForm);
            kernel.Bind<IProcessStarter>().To<ProcessStarter>().InSingletonScope();
            kernel.Bind<IUserNotifier>().To<UserNotifier>().InSingletonScope();
            kernel.Bind<ISystemTrayContextMenu>().ToConstant(mainForm);
            kernel.Bind<IBinDirectory>().To<BinDirectory>().InSingletonScope();
            kernel.Bind<IWaVersion>().To<WaVersion>().InSingletonScope();
            kernel.Bind<IWaVersionStringProvider>().To<WaVersionStringProvider>().InSingletonScope();

            LoggingSetup.Setup(kernel);
            mainForm.SetLogView(kernel.Get<LogView>());

            PersistenceSetup.BindPersistenceSystems(kernel);
            var manager = kernel.Get<PersistenceManager>();
            manager.LoadAndStartTracking(mainForm);
            mainForm.AfterPersistentStateLoaded();
        }

        public IWurmAssistantConfig WurmAssistantConfig { get { return kernel.Get<IWurmAssistantConfig>(); } }

        void SetupActivationStrategyComponents()
        {
            var allComponents = kernel.Components.GetAll<IActivationStrategy>().ToList();
            if (allComponents.Count != 7)
            {
                throw new Exception("Unexpected count of Ninject IActivationStrategy components");
            }
            kernel.Components.RemoveAll<IActivationStrategy>();
            kernel.Components.Add<IActivationStrategy, ActivationCacheStrategy>();
            kernel.Components.Add<IActivationStrategy, PropertyInjectionStrategy>();
            kernel.Components.Add<IActivationStrategy, MethodInjectionStrategy>();
            kernel.Components.Add<IActivationStrategy, PreInitializeActionsStrategy>();
            kernel.Components.Add<IActivationStrategy, InitializableStrategy>();
            kernel.Components.Add<IActivationStrategy, StartableStrategy>();
            kernel.Components.Add<IActivationStrategy, DisposableStrategy>();
        }

        public void Bootstrap()
        {
            WurmApiSetup.TryAutodetectWurmInstallDir(kernel);
            
            // this is where 'first time' config dialog is shown, if required
            ConfigSetup.ConfigureApplication(kernel);

            ConfigSetup.BindComponents(kernel);

            WurmApiSetup.BindSingletonApi(kernel);
            WurmApiSetup.ValidateWurmGameClientConfig(kernel);

            LogSearcherSetup.BindLogSearcher(kernel);

            FeaturesSetup.BindFeaturesManager(kernel);
            MainMenuSetup.BindMenu(kernel);

            mainForm.SetMenuView(kernel.Get<MenuView>());
            mainForm.SetModulesView(kernel.Get<FeaturesView>());

            var prov = kernel.Get<IWaVersionStringProvider>();
            mainForm.Text += string.Format(" ({0})", prov.Get());

            var featureManager = kernel.Get<IFeaturesManager>();
            featureManager.InitFeatures();
        }

        public void Dispose()
        {
            try
            {
                kernel.Dispose();
            }
            finally
            {
                dataDirectory.Unlock();
            }
        }
    }
}
