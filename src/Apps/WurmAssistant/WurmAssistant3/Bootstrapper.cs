using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Main;
using AldursLab.WurmAssistant3.Areas.Main.ViewModels;
using AldursLab.WurmAssistant3.Areas.Native;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
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
                System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                System.Windows.Forms.Application.EnableVisualStyles();
                Regex.CacheSize = 1000;

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

                var appMigrationsManager = kernel.Get<IAppMigrationsManager>();
                appMigrationsManager.RunMigrations();

                var featureManager = kernel.Get<IFeaturesManager>();
                featureManager.InitFeaturesAsync();

                DisplayRootViewFor<MainViewModel>();

                var appManager = kernel.Get<AppRuntimeManager>();
                appManager.ExecuteAfterStartupSteps();

                foreach (var dllLoadError in pluginManager.DllLoadErrors)
                {
                    logger.Error(dllLoadError.Exception, "Failed to load plugin DLL: " + dllLoadError.DllFileName);
                }

                System.Windows.Forms.Application.ThreadException += ApplicationOnThreadException;
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
            LogError(e.Exception);
            // Errors should not crash the application.
            e.Handled = true;
            base.OnUnhandledException(sender, e);
        }

        void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs threadExceptionEventArgs)
        {
            LogError(threadExceptionEventArgs.Exception);
        }

        void LogError(Exception e)
        {
            GetLogger().Error(e, "Something went wrong!");
            GetTrayPopups().Schedule("Wurm Assistant error: " + e.Message + Environment.NewLine + "Check logs for details", "Error!");
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

        ITrayPopups GetTrayPopups()
        {
            ITrayPopups tp;
            try
            {
                tp = kernel.Get<ITrayPopups>();
                return tp;
            }
            catch (Exception exception)
            {
                var logger = GetLogger();
                logger.Error(exception, "Unable to get ITrayPopups from IKernel. Skipping.");
            }

            return new TrayPopupsStub();
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
