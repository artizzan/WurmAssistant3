using System;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Wurm.Characters;

namespace AldurSoft.WurmApi.Wurm.Logs.Monitoring.WurmLogsMonitorModule
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