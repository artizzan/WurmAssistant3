using System;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.LogFeedManager
{
    class GrangerDebugLogger
    {
        readonly ILogger logger;

        public GrangerDebugLogger([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public void Log(string message)
        {
            logger.Info(message);
        }

        internal void Log(string message, bool isError, Exception _e = null)
        {
            if (_e == null)logger.Error(message);
            else logger.Error(_e, message);
        }
    }
}
