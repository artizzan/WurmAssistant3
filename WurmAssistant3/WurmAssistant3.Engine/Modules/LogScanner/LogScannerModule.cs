using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Attributes;
using AldurSoft.WurmAssistant3.Engine.Modules.LogScanner.Impl;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Modules.LogScanner;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.LogScanner
{
    [WurmAssistantModule("LogScanner")]
    public class LogScannerModule : ModuleBase, ILogScannerModule
    {
        private readonly LogScannerDataContext dataContext;

        public LogScannerModule(
            ModuleId moduleId,
            IModuleEngine moduleEngine,
            IPersistentManager persistentManager)
            : base(moduleId, moduleEngine, persistentManager)
        {
            this.dataContext = new LogScannerDataContext(persistentManager);
        }
    }
}
