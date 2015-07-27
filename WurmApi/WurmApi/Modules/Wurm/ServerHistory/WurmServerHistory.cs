using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Modules.Wurm.ServerHistory
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

        public async Task<ServerName> GetServer(CharacterName character, DateTime exactDate)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            var result = await provider.TryGetAtTimestamp(exactDate);
            if (result == null)
            {
                throw new DataNotFoundException(
                    string.Format("Server not found for timestamp {0} and character name {1}", exactDate, character));
            }
            return result;
        }

        public async Task<ServerName> GetCurrentServer(CharacterName character)
        {
            ServerHistoryProvider provider = GetServerHistoryProvider(character);
            var result = await provider.TryGetCurrentServer();
            if (result == null)
            {
                throw new DataNotFoundException(
                    string.Format("Current server not found for character name {0}", character));
            }
            return result;
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
