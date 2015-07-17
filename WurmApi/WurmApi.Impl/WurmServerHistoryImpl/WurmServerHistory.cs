using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Impl.WurmServerHistoryImpl
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
