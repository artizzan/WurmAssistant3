using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Services;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Modules;
using AldursLab.WurmAssistant3.Areas.Main.Services;
using AldursLab.WurmAssistant3.Areas.Main.ViewModels;
using AldursLab.WurmAssistant3.Areas.Native.Contracts;
using AldursLab.WurmAssistant3.Areas.Native.Services;
using AldursLab.WurmAssistant3.Areas.Persistence.Components;
using AldursLab.WurmAssistant3.Systems.AppUpgrades;
using AldursLab.WurmAssistant3.Systems.ConventionBinding;
using AldursLab.WurmAssistant3.Systems.Plugins;
using AldursLab.WurmAssistant3.Utils;
using AldursLab.WurmAssistant3.Utils.WinForms.Reusables;
using Caliburn.Micro;
using Ninject;
using Action = System.Action;

namespace AldursLab.WurmAssistant3
{
    public class Bootstrapper : BootstrapperBase
    {
        readonly IKernel kernel = new StandardKernel();

        IConsoleArgs consoleArgs;
        WurmAssistantDataDirectory dataDirectory;
        PluginManager pluginManager;

        public Bootstrapper()
        {
            HandleExceptions(() =>
            {
                consoleArgs = new ConsoleArgs();
                kernel.Bind<IConsoleArgs>().ToConstant(consoleArgs);

                dataDirectory = new WurmAssistantDataDirectory(consoleArgs);
                kernel.Bind<IWurmAssistantDataDirectory, WurmAssistantDataDirectory>()
                      .ToConstant(dataDirectory);

                var pluginsDir = new DirectoryInfo(Path.Combine(dataDirectory.DirectoryPath, "Plugins"));
                pluginManager = new PluginManager(pluginsDir);
                pluginManager.EnablePlugins();
                kernel.Bind<PluginManager>().ToConstant(pluginManager);

                Initialize();
            });
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            HandleExceptions(() =>
            {
                kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();

                VersionUpgradeManager upgradeManager = new VersionUpgradeManager(dataDirectory, consoleArgs);
                upgradeManager.RunUpgrades();

                System.Windows.Forms.Application.EnableVisualStyles();
                Regex.CacheSize = 1000;

                var kernelConfig = KernelConfig.EnableFor(kernel);
                kernel.Bind<IKernelConfig>().ToConstant(kernelConfig);

                IMessageBus messageBus = new MessageBus();
                kernel.Bind<IMessageBus>().ToConstant(messageBus);

                kernelConfig.AddPostInitializeActivations(
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

                var conventionBindingManager = new ConventionBindingManager(
                    kernel,
                    new[] { this.GetType().Assembly }.Concat(pluginManager.PluginAssemblies).ToArray());
                conventionBindingManager.BindAssembliesByConvention();

                var persistentDataManager = kernel.Get<PersistenceEnabler>();
                persistentDataManager.SetupPersistenceActivation();

                var logger = GetLogger();

                var featureManager = kernel.Get<IFeaturesManager>();
                featureManager.InitFeaturesAsync();

                DisplayRootViewFor<MainViewModel>();

                var appManager = kernel.Get<AppRuntimeManager>();
                appManager.ExecuteAfterStartupSteps();

                foreach (var dllLoadError in pluginManager.DllLoadErrors)
                {
                    logger.Error(dllLoadError.Exception, "Failed to load plugin DLL: " + dllLoadError.DllFileName);
                }
            });
        }

        void HandleExceptions(Action action)
        {
            try
            {
                action();
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

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return
                base.SelectAssemblies()
                    .Concat(new[] {this.GetType().Assembly})
                    .Concat(pluginManager.PluginAssemblies)
                    .Distinct()
                    .ToArray();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            GetLogger().Error(e.Exception, "Something went very wrong!");
            // Errors should not crash the application.
            e.Handled = true;
            base.OnUnhandledException(sender, e);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            kernel.Dispose();
        }

        ILogger GetLogger()
        {
            ILogger logger;
            try
            {
                logger = kernel.Get<ILogger>();
            }
            catch (Exception exception)
            {
                logger = new Logger(new LogMessageDumpStub(), this.GetType().FullName);
                logger.Error(exception, "Unable to get ILogger from IKernel. Creating logger explicitly.");
            }

            return logger;
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

            var resetConfigButton = new Button()
            {
                Text = "Reset Wurm Assistant config",
                Height = 28,
                Width = 220
            };
            resetConfigButton.Click += (o, args) =>
            {
                if (TryResetConfig())
                {
                    System.Windows.Forms.MessageBox.Show("Reset complete, please restart.", "Done", MessageBoxButtons.OK);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Config is unavailable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var restartAppButton = new Button()
            {
                Text = "Restart Wurm Assistant",
                Height = 28,
                Width = 220,
                DialogResult = DialogResult.OK
            };
            restartAppButton.Click += (o, args) => restart = true;

            var closeAppButton = new Button()
            {
                Text = "Close Wurm Assistant",
                Height = 28,
                Width = 220,
                DialogResult = DialogResult.OK
            };

            var view = new UniversalTextDisplayView(closeAppButton, restartAppButton, resetConfigButton)
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

        bool TryResetConfig()
        {
            var settings = kernel.TryGet<IWurmAssistantConfig>();
            if (settings != null)
            {
                settings.WurmApiResetRequested = true;
                return true;
            }
            return false;
        }

        void ShutdownCurrentApp()
        {
            System.Windows.Application.Current.Shutdown();
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
