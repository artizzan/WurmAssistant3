using System;
using System.Diagnostics;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    [KernelHint(BindingHint.Singleton)]
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