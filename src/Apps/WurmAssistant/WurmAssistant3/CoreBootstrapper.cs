using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using AldursLab.Essentials.Eventing;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Calendar;
using AldursLab.WurmAssistant3.Areas.CombatAssistant;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Config.Modules;
using AldursLab.WurmAssistant3.Areas.Core.Components.Obsolete;
using AldursLab.WurmAssistant3.Areas.Core.Components.Singletons;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Views;
using AldursLab.WurmAssistant3.Areas.CraftingAssistant;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Granger;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Views;
using AldursLab.WurmAssistant3.Areas.LogSearcher;
using AldursLab.WurmAssistant3.Areas.MainMenu;
using AldursLab.WurmAssistant3.Areas.MainMenu.Views;
using AldursLab.WurmAssistant3.Areas.Native;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.RevealCreatures;
using AldursLab.WurmAssistant3.Areas.SkillStats;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.Timers;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport;
using AldursLab.WurmAssistant3.Areas.WurmApi;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;
using Ninject;
using Ninject.Activation.Strategies;
using Ninject.Extensions.Factory;

namespace AldursLab.WurmAssistant3
{
    public class CoreBootstrapper : IDisposable
    {
        readonly IKernel kernel = new StandardKernel();

        readonly MainForm mainForm;

        public CoreBootstrapper([NotNull] MainForm mainForm)
        {
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            this.mainForm = mainForm;
        }

        public void PreBootstrap()
        {
            SetupActivationStrategyComponents();

            Regex.CacheSize = 1000;

            kernel.Bind<ITimerFactory>().To<TimerFactory>().InSingletonScope();

            kernel.Bind<IEventBus>().To<EventBus>().InSingletonScope();
            //todo: this hack only until MainForm is replaced.
            mainForm.EventBus = kernel.Get<IEventBus>();
            kernel.Bind<ConsoleArgsManager>().ToSelf().InSingletonScope();
            kernel.Bind<ISuperFactory>().To<SuperFactory>().InSingletonScope();
            kernel.Bind<WurmAssistantConfig, IWurmAssistantConfig>().To<WurmAssistantConfig>().InSingletonScope();

            kernel.Bind<IWurmAssistantDataDirectory>().To<WurmAssistantDataDirectory>().InSingletonScope();
            kernel.ProhibitGet<WurmAssistantDataDirectory>();

            kernel.Bind<DispatcherThreadMarshaller, IThreadMarshaller, IWurmApiEventMarshaller>()
                  .To<DispatcherThreadMarshaller>().InSingletonScope();
            
            kernel.Bind<MainForm, IHostEnvironment, ISystemTrayContextMenu>().ToConstant(mainForm);

            kernel.Bind<ProcessStarter, IProcessStarter>().To<ProcessStarter>().InSingletonScope();
            kernel.Bind<UserNotifier, IUserNotifier>().To<UserNotifier>().InSingletonScope();
            kernel.Bind<BinDirectory, IBinDirectory>().To<BinDirectory>().InSingletonScope();
            kernel.Bind<WaVersion, IWaVersion>().To<WaVersion>().InSingletonScope();
            kernel.Bind<WaExecutionInfoProvider, IWaExecutionInfoProvider>().To<WaExecutionInfoProvider>().InSingletonScope();
            kernel.Bind<ChangelogManager, IChangelogManager>().To<ChangelogManager>().InSingletonScope();

            kernel.Bind<SendBugReportView>().ToSelf();
            kernel.Bind<ISendBugReportViewFactory>().ToFactory();

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
            NativeSetup.Bind(kernel);
            WurmApiSetup.TryAutodetectWurmInstallDir(kernel);
            
            // this is where 'first time' config dialog is shown, if required
            ConfigSetup.ConfigureApplication(kernel);

            ConfigSetup.BindComponents(kernel);

            WurmApiSetup.BindSingletonApi(kernel);
            WurmApiSetup.ValidateWurmGameClientConfig(kernel);

            TrayPopupsSetup.BindTrayPopups(kernel);
            SoundManagerSetup.Bind(kernel);

            Wa2DataImportSetup.Bind(kernel);

            LogSearcherSetup.BindLogSearcher(kernel);
            CalendarSetup.BindCalendar(kernel);
            TimersSetup.BindTimers(kernel);
            TriggersSetup.BindTriggers(kernel);
            GrangerSetup.BindGranger(kernel);
            CraftingAssistantSetup.Bind(kernel);
            RevealCreaturesSetup.Bind(kernel);
            SkillStatsSetup.Bind(kernel);
            CombatAssistantSetup.Bind(kernel);

            FeaturesSetup.BindFeaturesManager(kernel);
            MainMenuSetup.BindMenu(kernel);

            mainForm.SetMenuView(kernel.Get<MenuView>());
            mainForm.SetFeaturesManager(kernel.Get<IFeaturesManager>());

            var prov = kernel.Get<IWaExecutionInfoProvider>();
            mainForm.Text += string.Format(" ({0})", prov.Get());

            var featureManager = kernel.Get<IFeaturesManager>();
            featureManager.InitFeatures();

            ShowChangelog();
        }

        void ShowChangelog()
        {
            var changelogManager = kernel.Get<IChangelogManager>();
            var userNotify = kernel.Get<IUserNotifier>();
            var logger = kernel.Get<ILogger>();
            try
            {
                var changes = changelogManager.GetNewChanges();
                if (!string.IsNullOrWhiteSpace(changes))
                {
                    changelogManager.ShowChanges(changes);
                    changelogManager.UpdateLastChangeDate();
                }
            }
            catch (Exception exception)
            {
                logger.Warn(exception, "Error at parsing or opening changelog");
                userNotify.NotifyWithMessageBox("Error opening changelog, see logs for details.", NotifyKind.Warning);
            }
        }

        public void Dispose()
        {
            try
            {
                kernel.Dispose();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        public ILogger GetCoreLogger()
        {
            return kernel.Get<ILogger>();
        }

        public bool TryResetConfig()
        {
            var settings = kernel.TryGet<WurmAssistantConfig>();
            if (settings != null)
            {
                settings.ReSetupRequested = true;
                return true;
            }
            return false;
        }
    }
}
