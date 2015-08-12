using AldursLab.WurmAssistant3.Attributes;
using AldursLab.WurmAssistant3.Engine.Modules.LogScanner.Impl;
using AldursLab.WurmAssistant3.Modules;
using AldursLab.WurmAssistant3.Modules.LogScanner;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Modules.LogScanner
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
