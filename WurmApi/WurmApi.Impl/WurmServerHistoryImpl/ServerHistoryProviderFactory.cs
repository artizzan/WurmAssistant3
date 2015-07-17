using System;

using AldurSoft.SimplePersist;

namespace AldurSoft.WurmApi.Impl.WurmServerHistoryImpl
{
    public class ServerHistoryProviderFactory
    {
        private readonly IWurmApiDataContext dataContext;
        private readonly IWurmLogsHistory wurmLogsHistory;
        private readonly IWurmServerList wurmServerList;
        private readonly ILogger logger;
        private readonly IWurmLogsMonitor wurmLogsMonitor;
        private readonly IWurmLogFiles wurmLogFiles;

        public ServerHistoryProviderFactory(
            IWurmApiDataContext dataContext,
            IWurmLogsHistory wurmLogsHistory,
            IWurmServerList wurmServerList,
            ILogger logger,
            IWurmLogsMonitor wurmLogsMonitor,
            IWurmLogFiles wurmLogFiles)
        {
            if (dataContext == null) throw new ArgumentNullException("dataContext");
            if (wurmLogsHistory == null) throw new ArgumentNullException("wurmLogsHistory");
            if (wurmServerList == null) throw new ArgumentNullException("wurmServerList");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmLogsMonitor == null) throw new ArgumentNullException("wurmLogsMonitor");
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            this.dataContext = dataContext;
            this.wurmLogsHistory = wurmLogsHistory;
            this.wurmServerList = wurmServerList;
            this.logger = logger;
            this.wurmLogsMonitor = wurmLogsMonitor;
            this.wurmLogFiles = wurmLogFiles;
        }

        public virtual ServerHistoryProvider Create(CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            var persistenceManager = dataContext.ServerHistory.Get(new EntityKey(characterName.Normalized));
            var wurmCharacterLogFiles = wurmLogFiles.GetManagerForCharacter(characterName);
            return new ServerHistoryProvider(
                characterName,
                persistenceManager,
                wurmLogsMonitor,
                wurmLogsHistory,
                wurmServerList,
                logger,
                wurmCharacterLogFiles);
        }
    }
}
