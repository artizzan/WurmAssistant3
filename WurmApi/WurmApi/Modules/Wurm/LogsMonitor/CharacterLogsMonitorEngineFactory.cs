using System;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    public class CharacterLogsMonitorEngineFactory
    {
        private readonly ILogger logger;
        private readonly SingleFileMonitorFactory singleFileMonitorFactory;
        private readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        public CharacterLogsMonitorEngineFactory(
            ILogger logger,
            SingleFileMonitorFactory singleFileMonitorFactory,
            IWurmCharacterLogFiles wurmCharacterLogFiles)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (singleFileMonitorFactory == null) throw new ArgumentNullException("singleFileMonitorFactory");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            this.logger = logger;
            this.singleFileMonitorFactory = singleFileMonitorFactory;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;
        }

        public virtual CharacterLogsMonitorEngine Create(CharacterName characterName)
        {
            return new CharacterLogsMonitorEngine(characterName, logger, singleFileMonitorFactory, wurmCharacterLogFiles);
        }
    }
}