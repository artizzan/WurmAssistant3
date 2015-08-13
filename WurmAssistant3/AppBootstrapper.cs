using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using AldursLab.WurmAssistant3.Core;
using AldursLab.WurmAssistant3.Core.Logging;
using Caliburn.Micro;
using Ninject;

namespace AldursLab.WurmAssistant3
{
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly IKernel kernel = new StandardKernel();
        CoreBootstrapper coreBootstrapper;
        ILogger globalLogger;

        public AppBootstrapper()
        {
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var dataDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AldursLab",
                "WurmAssistant3");

            coreBootstrapper = new CoreBootstrapper(kernel,
                new WurmAssistantConfig() {DataDirectoryFullPath = dataDirPath},
                new EventMarshaller());
            coreBootstrapper.Bootstrap();

            globalLogger = coreBootstrapper.GetGlobalLogger();

            //todo: display error message on bootstrap error
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
            if (globalLogger != null)
            {
                globalLogger.Info("Exiting WurmAssistant");
            }
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
