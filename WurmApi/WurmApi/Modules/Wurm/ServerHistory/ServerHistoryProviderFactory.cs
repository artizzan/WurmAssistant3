using System;
using AldursLab.PersistentObjects;
using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.Modules.Wurm.LogsMonitor;

namespace AldurSoft.WurmApi.Modules.Wurm.ServerHistory
{
    class ServerHistoryProviderFactory
    {
        private readonly IPersistentCollection persistentCollection;
        private readonly IWurmLogsHistory wurmLogsHistory;
        private readonly IWurmServerList wurmServerList;
        private readonly ILogger logger;
        private readonly IWurmLogsMonitorInternal wurmLogsMonitor;
        private readonly IWurmLogFiles wurmLogFiles;

        public ServerHistoryProviderFactory(
            IPersistentCollection persistentCollection,
            IWurmLogsHistory wurmLogsHistory,
            IWurmServerList wurmServerList,
            ILogger logger,
            IWurmLogsMonitorInternal wurmLogsMonitor,
            IWurmLogFiles wurmLogFiles)
        {
            if (persistentCollection == null) throw new ArgumentNullException("persistentCollection");
            if (wurmLogsHistory == null) throw new ArgumentNullException("wurmLogsHistory");
            if (wurmServerList == null) throw new ArgumentNullException("wurmServerList");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmLogsMonitor == null) throw new ArgumentNullException("wurmLogsMonitor");
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            this.persistentCollection = persistentCollection;
            this.wurmLogsHistory = wurmLogsHistory;
            this.wurmServerList = wurmServerList;
            this.logger = logger;
            this.wurmLogsMonitor = wurmLogsMonitor;
            this.wurmLogFiles = wurmLogFiles;
        }

        public virtual ServerHistoryProvider Create(CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            var persistent = persistentCollection.GetObject<PersistentModel.ServerHistory>(characterName.Normalized);
            var wurmCharacterLogFiles = wurmLogFiles.GetForCharacter(characterName);
            return new ServerHistoryProvider(
                characterName,
                persistent,
                wurmLogsMonitor,
                wurmLogsHistory,
                wurmServerList,
                logger,
                wurmCharacterLogFiles);
        }
    }
}
