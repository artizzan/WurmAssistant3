using System;
using System.Diagnostics;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Root.Components
{
    class ProcessStarter : IProcessStarter
    {
        readonly ILogger logger;

        public ProcessStarter([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public void StartSafe(string filePath)
        {
            try
            {
                Process.Start(filePath);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error while starting process: " + filePath);
            }
        }
    }
}