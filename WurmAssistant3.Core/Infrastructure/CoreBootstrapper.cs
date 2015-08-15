using System;
using System.IO;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Logging;
using AldursLab.WurmAssistant3.Core.ViewModels;
using JetBrains.Annotations;
using Ninject;
using ILogger = AldursLab.WurmAssistant3.Core.Logging.ILogger;

namespace AldursLab.WurmAssistant3.Core.Infrastructure
{
    public class CoreBootstrapper
    {
        readonly IKernel kernel;

        LoggingManager loggingManager;
        ILogger coreLogger;
        IWurmApi wurmApi; 

        public CoreBootstrapper([NotNull] IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            this.kernel = kernel;

            kernel.Bind<LoggerFactory>().ToSelf().InSingletonScope();
            kernel.Bind<LogOutputViewModel, ILogMessageProcessor>().To<LogOutputViewModel>().InSingletonScope();

            kernel.Bind<AppRunningViewModel>().ToSelf().InSingletonScope();
        }

        public AppStartViewModel GetAppStartViewModel()
        {
            return kernel.Get<AppStartViewModel>();
        }

        public void Bootstrap()
        {
            var config = kernel.Get<WurmAssistantConfig>();

            loggingManager = new LoggingManager(Path.Combine(config.DataDirectoryFullPath, "Logs"));
            kernel.Bind<LoggingManager>().ToConstant(loggingManager);
            var loggerFactory = kernel.Get<LoggerFactory>();
            coreLogger = loggerFactory.Create("Core");

            coreLogger.Info("Logging system ready");
            coreLogger.Info("WurmApi init");

            var eventMarshaller = kernel.Get<IEventMarshaller>();

            wurmApi = WurmApiFactory.Create(Path.Combine(config.DataDirectoryFullPath, "WurmApi"),
                loggerFactory.CreateWithGuiThreadMarshaller("WurmApi"),
                eventMarshaller);
            kernel.Bind<IWurmApi>().ToConstant(wurmApi);

            coreLogger.Info("WurmApi ready");
        }

        public AppRunningViewModel GetAppRunningViewModel()
        {
            return kernel.Get<AppRunningViewModel>();
        }
    }

    public class WurmAssistantConfig
    {
        public string DataDirectoryFullPath { get; set; }
    }
}
