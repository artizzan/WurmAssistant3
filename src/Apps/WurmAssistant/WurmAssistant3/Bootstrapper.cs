using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AldursLab.Essentials.Eventing;
using AldursLab.Essentials.Synchronization;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Calendar;
using AldursLab.WurmAssistant3.Areas.CombatAssistant;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Config.Modules;
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
using AldursLab.WurmAssistant3.Areas.Native.Contracts;
using AldursLab.WurmAssistant3.Areas.Native.Modules;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.RevealCreatures;
using AldursLab.WurmAssistant3.Areas.SkillStats;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.Timers;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport;
using AldursLab.WurmAssistant3.Areas.WurmApi;
using AldursLab.WurmAssistant3.Utils;
using AldursLab.WurmAssistant3.Utils.IoC;
using AldursLab.WurmAssistant3.Utils.WinForms.Reusables;
using Caliburn.Micro;
using Ninject;
using Ninject.Extensions.Factory;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace AldursLab.WurmAssistant3
{
    public class Bootstrapper : BootstrapperBase
    {
        readonly IKernel kernel = new StandardKernel();
        IConsoleArgs consoleArgs;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            try
            {
                consoleArgs = new ConsoleArgs();
                kernel.Bind<IConsoleArgs>().ToConstant(consoleArgs);
                System.Windows.Forms.Application.EnableVisualStyles();

                Regex.CacheSize = 1000;

                var customKernelConfig = new KernelConfig(kernel);
                kernel.Bind<IKernelConfig>().ToConstant(customKernelConfig);

                IMessageBus messageBus = new MessageBus();
                //mainForm.MessageBus = messageBus;
                kernel.Bind<IMessageBus>().ToConstant(messageBus);

                customKernelConfig.AddPostInitializeActivations(
                    (context, reference) =>
                    {
                        if (reference.Instance is IHandle)
                        {
                            messageBus.Subscribe(reference.Instance);
                        }
                    },
                    (context, reference) =>
                    {
                        if (reference.Instance is IHandle)
                        {
                            messageBus.Unsubscribe(reference.Instance);
                        }
                    });

                kernel.Bind<ITimerFactory>().To<TimerFactory>().InSingletonScope();
                kernel.Bind<IEnvironment>().To<WpfAppEnvironment>().InSingletonScope();

                kernel.Bind<ISuperFactory>().To<SuperFactory>().InSingletonScope();
                kernel.Bind<WurmAssistantConfig, IWurmAssistantConfig>().To<WurmAssistantConfig>().InSingletonScope();

                kernel.Bind<IWurmAssistantDataDirectory>().To<WurmAssistantDataDirectory>().InSingletonScope();
                kernel.ProhibitGet<WurmAssistantDataDirectory>();

                kernel.Bind<DispatcherThreadMarshaller, IThreadMarshaller, IWurmApiEventMarshaller>()
                      .To<DispatcherThreadMarshaller>().InSingletonScope();

                kernel.Bind<ISystemTrayContextMenu>().To<TrayMenu>().InSingletonScope();
                kernel.Bind<MainForm>().ToSelf().InSingletonScope();

                kernel.Bind<ProcessStarter, IProcessStarter>().To<ProcessStarter>().InSingletonScope();
                kernel.Bind<UserNotifier, IUserNotifier>().To<UserNotifier>().InSingletonScope();
                kernel.Bind<BinDirectory, IBinDirectory>().To<BinDirectory>().InSingletonScope();
                kernel.Bind<WaVersion, IWaVersion>().To<WaVersion>().InSingletonScope();
                kernel.Bind<WaExecutionInfoProvider, IWaExecutionInfoProvider>()
                      .To<WaExecutionInfoProvider>()
                      .InSingletonScope();
                kernel.Bind<ChangelogManager, IChangelogManager>().To<ChangelogManager>().InSingletonScope();

                kernel.Bind<SendBugReportView>().ToSelf();
                kernel.Bind<ISendBugReportViewFactory>().ToFactory();

                LoggingSetup.Setup(kernel);

                PersistenceSetup.BindPersistenceSystems(kernel);

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

                var featureManager = kernel.Get<IFeaturesManager>();
                featureManager.InitFeatures();

                var mainForm = kernel.Get<MainForm>();
                mainForm.Closed += (o, args) => ShutdownCurrentApp();
                mainForm.Show();
            }
            catch (LockFailedException)
            {
                try
                {
                    AttemptToRestoreAlreadyRunningAppInstance();
                }
                catch (Exception exception)
                {
                    ShowErrorAsDialog(exception);
                }

                ShutdownCurrentApp();
            }
            catch (ConfigCancelledException)
            {
                ShutdownCurrentApp();
            }
            catch (Exception exception)
            {
                bool handled = false;

                var validator = new IrrklangDependencyValidator();

                handled = validator.HandleWhenMissingIrrklangDependency(exception);

                if (!handled)
                {
                    ShowErrorAsDialog(exception);
                }

                ShutdownCurrentApp();
            }
        }

        void AttemptToRestoreAlreadyRunningAppInstance()
        {
            if (consoleArgs != null)
            {
                INativeCalls rwc = new Win32NativeCalls();
                if (consoleArgs.WurmUnlimitedMode)
                {
                    rwc.AttemptToBringMainWindowToFront("AldursLab.WurmAssistant3", "Unlimited");
                }
                else
                {
                    rwc.AttemptToBringMainWindowToFront("AldursLab.WurmAssistant3", @"^((?!Unlimited).)*$");
                }
            }
        }

        void ShowErrorAsDialog(Exception exception)
        {
            bool restart = false;
            var btn1 = new Button()
            {
                Text = "Reset Wurm Assistant config",
                Height = 28,
                Width = 220
            };
            btn1.Click += (o, args) =>
            {
                if (TryResetConfig())
                {
                    System.Windows.Forms.MessageBox.Show("Reset complete, please restart.", "Done", MessageBoxButtons.OK);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Reset was not possible.", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            var btn2 = new Button()
            {
                Text = "Restart Wurm Assistant",
                Height = 28,
                Width = 220,
                DialogResult = DialogResult.OK
            };
            btn2.Click += (o, args) => restart = true;
            var btn3 = new Button()
            {
                Text = "Close Wurm Assistant",
                Height = 28,
                Width = 220,
                DialogResult = DialogResult.OK
            };
            var view = new UniversalTextDisplayView(btn3, btn2, btn1)
            {
                Text = "OH NO!!",
                ContentText = "Application startup was interrupted by an ugly error! "
                              + Environment.NewLine
                              + Environment.NewLine + exception.ToString()
            };

            view.ShowDialog();
            if (restart) RestartCurrentApp();
            else ShutdownCurrentApp();
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

        void ShutdownCurrentApp()
        {
            Application.Current.Shutdown();
        }

        void RestartCurrentApp()
        {
            System.Windows.Forms.Application.Restart();
            // Restart does not automatically shutdown WPF application
            ShutdownCurrentApp();
        }

        #region Kernel wirings for view resolver

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var instance = key == null ? kernel.Get(service) : kernel.Get(service, key);

            if (instance == null)
            {
                throw new InvalidOperationException("dependency missing for type " + service.FullName);
            }

            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            kernel.Inject(instance);
        }

        #endregion
    }
}
