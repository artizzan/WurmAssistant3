using System;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    class ServerHistoryProviderFactory
    {
        private readonly IPersistentCollection persistentCollection;
        private readonly IWurmLogsHistory wurmLogsHistory;
        private readonly IWurmServerList wurmServerList;
        private readonly IWurmApiLogger logger;
        private readonly IWurmLogsMonitorInternal wurmLogsMonitor;
        private readonly IWurmLogFiles wurmLogFiles;
        readonly IInternalEventAggregator internalEventAggregator;

        public ServerHistoryProviderFactory(
            IPersistentCollection persistentCollection,
            IWurmLogsHistory wurmLogsHistory,
            IWurmServerList wurmServerList,
            IWurmApiLogger logger,
            IWurmLogsMonitorInternal wurmLogsMonitor,
            IWurmLogFiles wurmLogFiles, [NotNull] IInternalEventAggregator internalEventAggregator)
        {
            if (persistentCollection == null) throw new ArgumentNullException(nameof(persistentCollection));
            if (wurmLogsHistory == null) throw new ArgumentNullException(nameof(wurmLogsHistory));
            if (wurmServerList == null) throw new ArgumentNullException(nameof(wurmServerList));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmLogsMonitor == null) throw new ArgumentNullException(nameof(wurmLogsMonitor));
            if (wurmLogFiles == null) throw new ArgumentNullException(nameof(wurmLogFiles));
            if (internalEventAggregator == null) throw new ArgumentNullException(nameof(internalEventAggregator));
            this.persistentCollection = persistentCollection;
            this.wurmLogsHistory = wurmLogsHistory;
            this.wurmServerList = wurmServerList;
            this.logger = logger;
            this.wurmLogsMonitor = wurmLogsMonitor;
            this.wurmLogFiles = wurmLogFiles;
            this.internalEventAggregator = internalEventAggregator;
        }

        public ServerHistoryProvider Create(CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));
            var persistent = persistentCollection.GetObject<PersistentModel.ServerHistory>(characterName.Normalized);
            var wurmCharacterLogFiles = wurmLogFiles.GetForCharacter(characterName);
            return new ServerHistoryProvider(
                characterName,
                persistent,
                wurmLogsMonitor,
                wurmLogsHistory,
                wurmServerList,
                logger,
                wurmCharacterLogFiles,
                internalEventAggregator);
        }
    }
}
