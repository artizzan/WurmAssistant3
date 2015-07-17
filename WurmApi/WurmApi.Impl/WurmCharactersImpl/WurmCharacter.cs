using System;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Impl.WurmCharactersImpl
{
    public class WurmCharacter : IWurmCharacter, IRequireRefresh, IDisposable
    {
        private readonly IWurmConfigs wurmConfigs;
        private readonly IWurmServers wurmServers;
        private readonly IWurmServerHistory wurmServerHistory;
        private readonly IThreadGuard threadGuard;
        private readonly PlayerConfigWatcher playerConfigWatcher;
        private IWurmConfig currentConfig;

        public WurmCharacter(CharacterName name, string playerDirectoryFullPath, 
            IWurmConfigs wurmConfigs, IWurmServers wurmServers, IWurmServerHistory wurmServerHistory,
            IThreadGuard threadGuard)
        {
            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.threadGuard = threadGuard;
            if (name == null) throw new ArgumentNullException("name");
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            Name = name;

            playerConfigWatcher = new PlayerConfigWatcher(playerDirectoryFullPath);
            RefreshCurrentConfig();
            playerConfigWatcher.Changed += (sender, args) => RefreshCurrentConfig();
        }

        private void RefreshCurrentConfig()
        {
            CurrentConfig = wurmConfigs.GetConfig(playerConfigWatcher.CurrentConfigName);
        }

        public virtual CharacterName Name { get; private set; }

        public IWurmConfig CurrentConfig
        {
            get
            {
                threadGuard.ValidateCurrentThread();
                return currentConfig;
            }
            private set
            {
                threadGuard.ValidateCurrentThread();
                currentConfig = value;
            }
        }

        /// <summary>
        /// Returns exact server, that the player was on at given stamp.
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        /// <exception cref="WurmApiException"></exception>
        public async Task<IWurmServer> GetHistoricServerAtLogStamp(DateTime stamp)
        {
            threadGuard.ValidateCurrentThread();
            var serverName = await wurmServerHistory.TryGetServer(this.Name, stamp);
            if (serverName == null)
            {
                throw new WurmApiException("No historic server info found for given stamp " + stamp);
            }
            var server = wurmServers.TryGetByName(serverName);
            if (server == null)
            {
                throw new WurmApiException("No server definiton found for name " + serverName);
            }
            return server;
        }

        /// <exception cref="WurmApiException"></exception>
        public async Task<IWurmServer> GetCurrentServer()
        {
            threadGuard.ValidateCurrentThread();
            var serverName = await wurmServerHistory.TryGetCurrentServer(this.Name);
            if (serverName == null)
            {
                throw new WurmApiException("No server info found for current time.");
            }
            var server = wurmServers.TryGetByName(serverName);
            if (server == null)
            {
                throw new WurmApiException("No server definiton found for name " + serverName);
            }
            return server;
        }

        public void Refresh()
        {
            playerConfigWatcher.Refresh();
        }

        public void Dispose()
        {
            playerConfigWatcher.Dispose();
        }
    }
}