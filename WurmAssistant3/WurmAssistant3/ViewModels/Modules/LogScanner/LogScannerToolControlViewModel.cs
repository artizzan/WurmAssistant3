using System;

using AldurSoft.WurmAssistant3.Modules.LogScanner;

using Core.AppFramework.Wpf.ViewModels;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.ViewModels.Modules.LogScanner
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
