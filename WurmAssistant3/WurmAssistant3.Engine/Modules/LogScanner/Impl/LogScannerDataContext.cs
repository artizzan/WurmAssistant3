using System;
using AldursLab.Deprec.Core;

namespace AldursLab.WurmAssistant3.Engine.Modules.LogScanner.Impl
{
    class LogScannerDataContext
    {
        private readonly IPersistentManager persistentManager;

        public LogScannerDataContext([NotNull] IPersistentManager persistentManager)
        {
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.persistentManager = persistentManager;
        }
    }
}
