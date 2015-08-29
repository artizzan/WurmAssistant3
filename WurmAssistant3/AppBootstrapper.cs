using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using AldursLab.WurmAssistant3.Core.Logging;
using AldursLab.WurmAssistant3.Infrastructure;
using AldursLab.WurmAssistant3.Model;
using AldursLab.WurmAssistant3.ViewModels;
using Caliburn.Micro;
using Ninject;
using Environment = AldursLab.WurmAssistant3.Infrastructure.Environment;
using ILogger = AldursLab.WurmAssistant3.Core.Logging.ILogger;

namespace AldursLab.WurmAssistant3
{
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly IKernel kernel = new StandardKernel();
        CoreBootstrapper coreBootstrapper;
        ILogger globalLogger;

        WindowManager windowManager;
        readonly Assembly[] viewViewModelAssemblies;

        readonly Environment environment = new Environment();

        SharedDataDirectory sharedDataDirectory;

        public AppBootstrapper()
        {
            viewViewModelAssemblies = CaliburnConventionsHelper.MapConventionsForAssemblies(
                new[]
                {
                    // views are in this assembly and in Windows-specific assembly
                    typeof(AldursLab.WurmAssistant3.Gui.Windows.ReflectionRootAnchor), 
                    typeof(AldursLab.WurmAssistant3.ReflectionRootAnchor)
                },
                new[]
                {
                    // viewmodels should be almost exclusively in core assembly
                    typeof (AldursLab.WurmAssistant3.ReflectionRootAnchor),
                    typeof (AldursLab.WurmAssistant3.Core.ReflectionRootAnchor)
                });

            Initialize();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return viewViewModelAssemblies;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // configure and bind app-specific dependencies

            kernel.Bind<IEnvironment>().ToConstant(environment).InSingletonScope();

            windowManager = new WindowManager();
            kernel.Bind<WindowManager>().ToConstant(windowManager);
            kernel.Bind<IWindowManager>().To<WindowManager>();

            kernel.Bind<AppHostViewModel>().ToSelf().InSingletonScope();

            try
            {
                sharedDataDirectory = new SharedDataDirectory();
            }
            catch (LockFailedException)
            {
                // if cannot obtain exclusive lock on data directory, it means another instance is already running
                Application.Current.Shutdown();
                return;
            }

            var config = new WurmAssistantConfig() {DataDirectoryFullPath = sharedDataDirectory.FullName};
            kernel.Bind<IWurmAssistantConfig>().ToConstant(config);
            var marshaller = new WpfGuiThreadEventMarshaller(environment);
            kernel.Bind<IEventMarshaller, WpfGuiThreadEventMarshaller>().ToConstant(marshaller);

            // create hosting window for the app
            var hostView = kernel.Get<AppHostViewModel>();
            windowManager.ShowWindow(hostView, null, new Dictionary<string, object>());

            // initialize and resolve the app
            coreBootstrapper = new CoreBootstrapper(kernel);

            // show the application startup screen

            var appStartVm = coreBootstrapper.GetAppStartViewModel();
            hostView.CurrentScreen = appStartVm;

            // initialize application

            coreBootstrapper.BootstrapRuntime();
            globalLogger = kernel.Get<LoggerFactory>().Create();

            // show the application running screen

            var appRunningVm = coreBootstrapper.GetAppRunningViewModel();
            hostView.CurrentScreen = appRunningVm;
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (globalLogger != null)
            {
                var prefix = sender != null ? sender.GetType().FullName : string.Empty;
                var exceptionMessage = e.Exception != null ? ": " + e.Exception.Message : String.Empty;
                var completeMessage = prefix + ": Unhandled global exception" + exceptionMessage;
                globalLogger.Error(e.Exception, completeMessage);
                e.Handled = true;
            }
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            environment.Closing = true;
            if (globalLogger != null) globalLogger.Info("Exiting WurmAssistant");
            if (sharedDataDirectory != null) sharedDataDirectory.Dispose();
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
    }
}
