using System;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Modules.LogScanner;

namespace AldursLab.WurmAssistant3.ViewModels.Modules.LogScanner
{
    public class LogScannerToolControlViewModel : ModuleToolControlViewModel
    {
        private readonly ILogScannerModule logScannerModule;

        public LogScannerToolControlViewModel([NotNull] ILogScannerModule logScannerModule)
        {
            if (logScannerModule == null) throw new ArgumentNullException("logScannerModule");
            this.logScannerModule = logScannerModule;
        }
    }
}
