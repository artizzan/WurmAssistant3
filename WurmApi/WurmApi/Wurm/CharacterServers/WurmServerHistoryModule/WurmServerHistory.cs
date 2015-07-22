using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Persistence;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Logs;
using AldurSoft.WurmApi.Wurm.Logs.Monitoring;
using AldurSoft.WurmApi.Wurm.Logs.Searching;
using AldurSoft.WurmApi.Wurm.Servers;

namespace AldurSoft.WurmApi.Wurm.CharacterServers.WurmServerHistoryModule
{
    public class WurmServerHistory : IWurmServerHistory
    {
        readonly ServerHistoryProviderFactory providerFactory;

        readonly Dictionary<CharacterName, ServerHistoryProvider> historyProviders = new Dictionary<CharacterName, ServerHistoryProvider>();

        public WurmServerHistory(
            IWurmApiDataContext dataContext,
            IWurmLogsHistory wurmLogsHistory,
            IWurmServerList wurmServerList,
            ILogger logger,
            IWurmLogsMonitor wurmLogsMonitor,
            IWurmLogFiles wurmLogFiles)
        {
            providerFactory = new ServerHistoryProviderFactory(
                dataContext,
                wurmLogsHistory,
                wurmServerList,
                logger,
                wurmLogsMonitor,
                wurmLogFiles);
        }

        public async Task<ServerName> TryGetServer(CharacterName character, DateTime exactDate)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            return await provider.TryGetAtTimestamp(exactDate);
        }

        public async Task<ServerName> TryGetCurrentServer(CharacterName character)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            return await provider.TryGetCurrentServer();
        }

        ServerHistoryProvider GetServerHistoryProvider(CharacterName character)
        {
            ServerHistoryProvider provider;
            if (!historyProviders.TryGetValue(character, out provider))
            {
                provider = providerFactory.Create(character);
                historyProviders.Add(character, provider);
            }
            return provider;
        }
    }
}
