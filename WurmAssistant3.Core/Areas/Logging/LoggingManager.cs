using System;
using System.IO;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Model;
using AldursLab.WurmAssistant3.Core.Areas.Logging.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.Root.ViewModels;
using Ninject;
using ILogger = AldursLab.WurmAssistant3.Core.Areas.Logging.Model.ILogger;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging
{
    public class LoggingManager : ILoggerFactory, IWurmApiLoggerFactory, ILogMessagePublisher, ILogMessageHandler
    {
        public static void BindLoggingFactories(IKernel kernel)
        {
            var dataDirectory = kernel.Get<IWurmAssistantDataDirectory>();
            var logOutputDirFullPath = Path.Combine(dataDirectory.DirectoryPath, "Logs");
            LoggingConfig.Setup(logOutputDirFullPath);

            LoggingManager manager = new LoggingManager();
            kernel.Bind<ILoggerFactory>().ToConstant(manager);
            kernel.Bind<IWurmApiLoggerFactory>().ToConstant(manager);
            kernel.Bind<ILogMessagePublisher>().ToConstant(manager);
            kernel.Bind<ILogMessageHandler>().ToConstant(manager);
        }

        public static void BindLoggerAutoResolver(IKernel kernel)
        {
            kernel.Bind<ILogger>().ToMethod(context =>
            {
                // create logger with category matching target type name
                var factory = context.Kernel.Get<ILoggerFactory>();
                return factory.Create(context.Binding.Service.FullName);
            });
        }

        public static void EnableLoggingView(IKernel kernel)
        {
            var mainViewModel = kernel.Get<MainViewModel>();
            var logViewModel = kernel.Get<LogViewModel>();
            mainViewModel.LogViewModel = logViewModel;
        }

        readonly Logger factoryLogger;

        private LoggingManager()
        {
            factoryLogger = new Logger(this, "WurmApi");
        }

        public int ErrorCount { get; private set; }
        public event EventHandler<EventArgs> ErrorCountChanged;
        public event EventHandler<LogMessageEventArgs> EventLogged;

        ILogger ILoggerFactory.Create(string category)
        {
            return new Logger(this, category);
        }

        AldursLab.WurmApi.ILogger IWurmApiLoggerFactory.Create()
        {
            return new Logger(this, "WurmApi");
        }

        public void HandleEvent(NLog.LogLevel nlogLevel, string message, Exception exception)
        {
            if (nlogLevel == LogLevel.Error || nlogLevel == LogLevel.Fatal)
            {
                ErrorCount++;
                try
                {
                    OnErrorCountChanged();
                }
                catch (Exception handlingException)
                {
                    // calling logger directly to avoid enless loop of this error
                    factoryLogger.Error(handlingException, "Error at OnErrorCountChanged");
                }
            }

            var handler = EventLogged;
            if (handler != null)
            {
                var args = new LogMessageEventArgs(nlogLevel, message, exception);
                try
                {
                    handler(this, args);
                }
                catch (Exception handlingException)
                {
                    // calling logger directly to avoid enless loop of this error
                    factoryLogger.Error(handlingException, "Error during log event forwarding");
                }
            }
        }

        protected virtual void OnErrorCountChanged()
        {
            var handler = ErrorCountChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
