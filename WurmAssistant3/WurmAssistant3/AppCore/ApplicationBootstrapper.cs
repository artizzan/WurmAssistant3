using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using AldurSoft.SimplePersist;
using AldurSoft.SimplePersist.Persistence.FlatFiles;
using AldurSoft.SimplePersist.Serializers.JsonNet;
using AldurSoft.WurmApi;
using AldurSoft.WurmAssistant3.Attributes;
using AldurSoft.WurmAssistant3.DataContext;
using AldurSoft.WurmAssistant3.Engine.Modules;
using AldurSoft.WurmAssistant3.Engine.Modules.Calendar;
using AldurSoft.WurmAssistant3.Engine.Repositories;
using AldurSoft.WurmAssistant3.Engine.Systems;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Modules.Calendar;
using AldurSoft.WurmAssistant3.Systems;
using AldurSoft.WurmAssistant3.SystemsCustomBind;
using AldurSoft.WurmAssistant3.ViewModels;
using AldurSoft.WurmAssistant3.ViewModels.Main;
using AldurSoft.WurmAssistant3.ViewModels.Modules;
using AldurSoft.WurmAssistant3.ViewModels.Modules.Calendar;
using AldurSoft.WurmAssistant3.Views.Main;

using Caliburn.Micro;

using Castle.Core.Internal;

using Core.AppFramework.Wpf;
using Core.AppFramework.Wpf.Attributes;

using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Infrastructure.Language;
using Ninject.Syntax;

using NLog;

using WurmAssistant3;

using Action = System.Action;
using Environment = AldurSoft.WurmAssistant3.SystemsCustomBind.Environment;
using LogManager = NLog.LogManager;

namespace AldurSoft.WurmAssistant3.AppCore
{
    public class ApplicationBootstrapper : BootstrapperBase, IEnvironmentLifecycle
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IKernel kernel = new StandardKernel();
        private readonly HashSet<ModuleId> registeredModules = new HashSet<ModuleId>();

        private readonly IEnvironment environment;
        private readonly ILoggingSystem loggingSystem;

        public ApplicationBootstrapper()
        {
            EnvironmentStatus = EnvironmentStatus.Initializing;
            Initialize();
            environment = new Environment();
            loggingSystem = new LoggingSystem(environment);
            loggingSystem.EnableGlobalLogging();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                Logger.Info("Startup");
                base.OnStartup(sender, e);

                EnvironmentStatus = EnvironmentStatus.Binding;
                Logger.Info("Begin depedency binding");
                Bind();
                Logger.Info("Depedencies bound");

                EnvironmentStatus = EnvironmentStatus.Configuring;
                Logger.Info("Begin config");
                if (!DoConfig())
                {
                    // do not run the app.
                    return;
                }
                Logger.Info("Configured");

                EnvironmentStatus = EnvironmentStatus.Starting;
                Logger.Info("Begin run");
                Run();
                EnvironmentStatus = EnvironmentStatus.Running;
                Logger.Info("Running");
            }
            catch (Exception exception)
            {
                Logger.Fatal("OnStartup error.", exception);
                throw;
            }
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            EnvironmentStatus = EnvironmentStatus.Crashing;
            Logger.Fatal(e.Exception);
            OnEnvironmentClosing();
            base.OnUnhandledException(sender, e);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            EnvironmentStatus = EnvironmentStatus.Stopping;
            Logger.Info("Wurm Assistant begin shutdown");
            OnEnvironmentClosing();
            Logger.Info("Wurm Assistant begin cleanup");
            kernel.Dispose();
            Logger.Info("Wurm Assistant cleanup complete.");
            base.OnExit(sender, e);
        }

        #region Bindings

        private void Bind()
        {
            // explicit bindings

            kernel.Bind<IEnvironmentLifecycle>().ToConstant(this).InSingletonScope();
            kernel.Bind<IEnvironment>().ToConstant(environment).InSingletonScope();
            kernel.Bind<ILoggingSystem>().ToConstant(loggingSystem).InSingletonScope();

            kernel.Bind<IWindowManager, IExtendedWindowManager>().To<ExtendedWindowManager>().InSingletonScope();

            BindApplicationDataContext();

            // convention bindings

            // All system are intended to be scoped lifetimes. 
            // Since entire app is the scope here, they can just be singletons.
            BindSystems();

            // Only explicitly flagged view models should be singletons.
            BindViewModels();

            // All modules should be singletons.
            BindModules();

            // There should only be one WurmApi
            BindWurmApi();

            // binding current WurmApi modules
            // todo: convention as well?
            kernel.Bind(
                syntax =>
                syntax.FromThisAssembly()
                    .SelectAllClasses()
                    .InheritedFrom<ModuleToolControlViewModel>()
                    .Where(type => type.IsPublic)
                    .BindAllBaseClasses()
                    .Configure((onSyntax, type) => onSyntax.Named(type.Name)));

        }

        private void BindWurmApi()
        {
            kernel.Bind<IWurmApi, IWurmApiController>().ToMethod(
                context =>
                    {
                        var env = context.Kernel.Get<IEnvironment>();
                        var settings = context.Kernel.Get<IWurmAssistantSettings>();
                        var installDir = settings.Entity.WurmClientConfig.WurmClientInstallDirOverride;
                        WurmApiManager wurmApi =
                            new WurmApiManager(
                                new WurmApiDataDirectory(
                                    Path.Combine(env.FullPathToCurrentDataDirectory, "WurmApi"),
                                    createIfNotExists: true),
                                new WurmGameClientInstallDirectory(installDir),
                                new WurmApiLogger("AldurSoft.WurmApi"));
                        return wurmApi;
                    }).InSingletonScope();
        }

        private void BindModules()
        {
            kernel.Bind(
                syntax =>
                    {
                        syntax.FromAssemblyContaining<ModuleBase>()
                            .SelectAllClasses()
                            .InNamespaceOf<ModuleBase>()
                            .Where(type => type.IsPublic)
                            .BindDefaultInterface()
                            .Configure(
                                (onSyntax, type) =>
                                    {
                                        var descriptor = type.GetAttribute<WurmAssistantModuleAttribute>();
                                        if (descriptor == null)
                                        {
                                            throw new ArgumentException(
                                                string.Format(
                                                    "Module {0} is missing required {1}",
                                                    type.FullName,
                                                    typeof(WurmAssistantModuleAttribute).FullName));
                                        }
                                        var moduleId = new ModuleId(descriptor.ModuleId);
                                        if (registeredModules.Contains(moduleId))
                                        {
                                            throw new InvalidOperationException(
                                                string.Format(
                                                    "Module with id {0} is already registered.",
                                                    descriptor.ModuleId));
                                        }
                                        registeredModules.Add(moduleId);

                                        onSyntax.InSingletonScope()
                                            .WithConstructorArgument(
                                                typeof(IPersistentManager),
                                                stx =>
                                                    {
                                                        var env = stx.Kernel.Get<IEnvironment>();
                                                        return CreatePersistentManager(
                                                            stx,
                                                            env.GetFullPathToModulePersistentManagerDirectory(moduleId),
                                                            moduleId.ToString());
                                                    }).WithConstructorArgument(moduleId);
                                    });
                    });
        }

        private void BindApplicationDataContext()
        {
            kernel.Bind<IWurmAssistantDataContext>()
                .To<WurmAssistantDataContext>()
                .InSingletonScope()
                .WithConstructorArgument(
                    typeof(IPersistentManager),
                    context => CreatePersistentManager(context, "WurmAssistantData", "WurmAssistantDataContext"));
        }

        private void BindSystems()
        {
            // Limit to public types, as public types are intended to be subsystems,
            // while internal classes are intended as local implementation detail only.
            // Note: according to docs, SelectAllClasses should not grab internal classes,
            // but it does if type is in same assembly.

            // Bind from the .Engine project
            kernel.Bind(
                syntax =>
                syntax.FromAssemblyContaining<ModuleEngine>()
                    .SelectAllClasses()
                    .InNamespaceOf<ModuleEngine>()
                    .Where(type => type.IsPublic)
                    .BindDefaultInterface()
                    .Configure(onSyntax => onSyntax.InSingletonScope()));

            // Bind from this project
            kernel.Bind(
                syntax =>
                syntax.FromAssemblyContaining<TimerFactory>()
                    .SelectAllClasses()
                    .InNamespaceOf<TimerFactory>()
                    .Where(type => type.IsPublic)
                    .BindDefaultInterface()
                    .Configure(onSyntax => onSyntax.InSingletonScope()));
        }

        private void BindViewModels()
        {
            kernel.Bind(
                syntax =>
                syntax.FromAssemblyContaining<ApplicationWindowViewModel>()
                    .SelectAllClasses()
                    .InNamespaceOf<ApplicationWindowViewModel>()
                    .Where(type => type.IsPublic)
                    .BindToSelf()
                    .Configure(
                        (onSyntax, type) =>
                            {
                                if (type.HasAttribute<GlobalViewModelAttribute>())
                                {
                                    onSyntax.InSingletonScope();
                                }
                                else
                                {
                                    onSyntax.InTransientScope();
                                }
                            }));
        }

        private PersistentManager CreatePersistentManager(
            IContext context,
            string dataDirRelativePath,
            string loggingCategory)
        {
            var appEnv = context.Kernel.Get<IEnvironment>();
            var folder = Path.Combine(appEnv.FullPathToCurrentDataDirectory, dataDirRelativePath);
            var manager = new PersistentManager(
                new JsonSerializationStrategy(),
                new FlatFilesPersistenceStrategy(folder),
                new SimplePersistLogger(loggingCategory));
            return manager;
        }

        #endregion bindings

        #region Config

        private bool DoConfig()
        {
            var windowManager = kernel.Get<IExtendedWindowManager>();
            var applicationSettings = kernel.Get<IWurmAssistantSettings>();

            // choose and validate wurm game client install directory
            var installDirChooser = kernel.Get<WurmClientInstallDirChooserViewModel>();
            if (!installDirChooser.ValidateWurmDir())
            {
                installDirChooser.DescriptonTextBlock =
                    "Wurm game client install directory does not appear to be valid. Please choose it manually.";
                bool? applied = false;
                DisableDefaultAppClosingPolicy(
                    () =>
                    {
                        applied = windowManager.ShowDialog(
                            installDirChooser,
                            null,
                            new Dictionary<string, object>() { { "Title", "Wurm Assistant 3 Configuration" } });
                        return true;
                    });

                if (applied != true)
                {
                    Application.Shutdown();
                    return false;
                }
                else
                {
                    applicationSettings.Entity.WurmClientConfig.WurmClientInstallDirOverride =
                        installDirChooser.InstallDirFullPath;
                    applicationSettings.Save();
                }
            }

            // validate wurm game client configuration
            var wurmApiConfigurator = kernel.Get<IWurmApiConfigurator>();
            var verifyResult = wurmApiConfigurator.VerifyGameClientConfig();
            if (verifyResult.AnyIssues && !applicationSettings.Entity.WurmClientConfig.DoNotAskToSyncWurmClients)
            {
                // show gui to prompt for closing all game clients,
                return DisableDefaultAppClosingPolicy(
                () =>
                {
                    var configurer = kernel.Get<WurmClientConfigViewModel>();
                    configurer.RegularDisplayMode = true;
                    var dialogResult = windowManager.ShowDialog(
                        configurer,
                        null,
                        new Dictionary<string, object>() { { "Title", "Wurm Assistant 3 Configuration" } });
                    if (dialogResult != true)
                    {
                        Application.Shutdown();
                        return false;
                    }
                    switch (configurer.Result)
                    {
                        case WurmClientConfigViewModel.ResultValue.SyncConfigsAndContinue:
                            break;
                        case WurmClientConfigViewModel.ResultValue.IgnoreAndContinue:
                            break;
                        case WurmClientConfigViewModel.ResultValue.IgnoreAndShutdown:
                            Application.Shutdown();
                            break;
                    }
                    return true;
                });
            }

            return true;
        }

        private bool DisableDefaultAppClosingPolicy(Func<bool> action)
        {
            try
            {
                Application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                return action();
            }
            finally
            {
                try
                {
                    if (EnvironmentStatus != EnvironmentStatus.Stopping
                        || EnvironmentStatus != EnvironmentStatus.Crashing)
                    {
                        Application.ShutdownMode = ShutdownMode.OnLastWindowClose;
                    }
                }
                catch (InvalidOperationException exception)
                {
                    // happens if application is currently shutting down
                    Logger.Warn("Exception during DisableDefaultAppClosingPolicy", exception as Exception);
                }
            }
        }

        #endregion Config

        private void Run()
        {
            var props = new WindowSettingsBuilder()
            {
                WindowTitle = string.Format("Wurm Assistant {0}", Assembly.GetExecutingAssembly().GetName().Version)
            };
            DisplayRootViewFor<ApplicationWindowViewModel>(props.ToDictionary());
        }

        #region DI container implementation for Caliburn VM binder

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
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
                throw new ArgumentNullException("service");
            }

            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            kernel.Inject(instance);
        }

        #endregion

        #region IEnvironmentLifecycle implementation

        public EnvironmentStatus EnvironmentStatus { get; private set; }

        public event EventHandler EnvironmentClosing;

        public void RestartEnvironment()
        {
            System.Windows.Forms.Application.Restart();
        }

        private void OnEnvironmentClosing()
        {
            EventHandler handler = EnvironmentClosing;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
