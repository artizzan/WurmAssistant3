using System;
using System.IO;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using AldursLab.WurmAssistant3.Core.Logging;
using AldursLab.WurmAssistant3.Core.ViewModels;
using JetBrains.Annotations;
using Ninject;
using ILogger = AldursLab.WurmAssistant3.Core.Logging.ILogger;

namespace AldursLab.WurmAssistant3.Core
{
    public class CoreBootstrapper : IDisposable
    {
        readonly IKernel kernel;

        LoggingManager loggingManager;
        ILogger coreLogger;
        IWurmApi wurmApi;

        bool disposed = false;

        public CoreBootstrapper([NotNull] IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            this.kernel = kernel;

            BootstrapForSetup();
        }

        private void BootstrapForSetup()
        {
            kernel.Bind<LoggerFactory>().ToSelf().InSingletonScope();
            kernel.Bind<LogOutputViewModel, ILogMessageProcessor>().To<LogOutputViewModel>().InSingletonScope();
            kernel.Bind<AppRunningViewModel>().ToSelf().InSingletonScope();
        }

        public AppStartViewModel GetAppStartViewModel()
        {
            return kernel.Get<AppStartViewModel>();
        }

        public void BootstrapRuntime()
        {
            var config = kernel.Get<IWurmAssistantConfig>();

            loggingManager = new LoggingManager(Path.Combine(config.DataDirectoryFullPath, "Logs"));
            kernel.Bind<LoggingManager>().ToConstant(loggingManager);
            var loggerFactory = kernel.Get<LoggerFactory>();
            coreLogger = loggerFactory.Create("Core");
            coreLogger.Info("Logging system ready");

            coreLogger.Info("WurmApi init");
            var eventMarshaller = kernel.Get<IEventMarshaller>();
            ConstructWurmApi(config, loggerFactory, eventMarshaller);
            coreLogger.Info("WurmApi ready");
        }

        void ConstructWurmApi(IWurmAssistantConfig config, LoggerFactory loggerFactory, IEventMarshaller eventMarshaller)
        {
            IWurmInstallDirectory wurmInstallDirectory = null;
            if (!string.IsNullOrWhiteSpace(config.WurmGameClientInstallDirectory))
            {
                wurmInstallDirectory = new WurmInstallDirectoryOverride()
                {
                    FullPath = config.WurmGameClientInstallDirectory
                };
            }

            WurmApiConfig wurmApiConfig = null;
            if (config.RunningPlatform != Platform.Unknown)
            {
                wurmApiConfig = new WurmApiConfig {Platform = config.RunningPlatform};
            }

            wurmApi = WurmApiFactory.Create(Path.Combine(config.DataDirectoryFullPath, "WurmApi"),
                loggerFactory.CreateWithGuiThreadMarshaller("WurmApi"),
                eventMarshaller,
                wurmInstallDirectory,
                wurmApiConfig);
            kernel.Bind<IWurmApi>().ToConstant(wurmApi);
        }

        public AppRunningViewModel GetAppRunningViewModel()
        {
            return kernel.Get<AppRunningViewModel>();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (wurmApi != null)
                {
                    wurmApi.Dispose();
                }
                disposed = true;
            }
        }
    }
}
