using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Logging;
using JetBrains.Annotations;
using Ninject;
using ILogger = AldursLab.WurmAssistant3.Core.Logging.ILogger;

namespace AldursLab.WurmAssistant3.Core
{
    public class CoreBootstrapper
    {
        readonly IKernel kernel;
        readonly WurmAssistantConfig config;
        readonly IEventMarshaller eventMarshaller;

        LoggingManager loggingManager;
        ILogger globalLogger;
        IWurmApi wurmApi; 

        public CoreBootstrapper([NotNull] IKernel kernel, [NotNull] WurmAssistantConfig config,
            [NotNull] IEventMarshaller eventMarshaller)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            if (config == null) throw new ArgumentNullException("config");
            if (eventMarshaller == null) throw new ArgumentNullException("eventMarshaller");
            this.kernel = kernel;
            this.config = config;
            this.eventMarshaller = eventMarshaller;
        }

        public void Bootstrap()
        {
            loggingManager = new LoggingManager(Path.Combine(config.DataDirectoryFullPath, "Logs"));
            globalLogger = new Logger();
            globalLogger.Info("Logging system ready");
            globalLogger.Info("WurmApi init");
            wurmApi = WurmApiFactory.Create(Path.Combine(config.DataDirectoryFullPath, "WurmApi"),
                new Logger("WurmApi"),
                eventMarshaller);
            globalLogger.Info("WurmApi ready");
        }

        public ILogger GetGlobalLogger()
        {
            return globalLogger;
        }
    }

    public class WurmAssistantConfig
    {
        public string DataDirectoryFullPath { get; set; }
    }
}
