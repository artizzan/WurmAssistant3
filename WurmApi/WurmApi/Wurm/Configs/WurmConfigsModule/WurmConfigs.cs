using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Validation;
using AldurSoft.WurmApi.Wurm.GameClients;

namespace AldurSoft.WurmApi.Wurm.Configs.WurmConfigsModule
{
    public class WurmConfigs : IWurmConfigs, IRequireRefresh, IDisposable
    {
        private readonly IWurmGameClients wurmGameClients;
        private readonly IWurmConfigDirectories wurmConfigDirectories;
        private readonly ILogger logger;
        private readonly IThreadGuard threadGuard;
        private readonly Dictionary<string, WurmConfig> nameToConfigMap = new Dictionary<string, WurmConfig>();

        public WurmConfigs(
            IWurmGameClients wurmGameClients,
            IWurmConfigDirectories wurmConfigDirectories,
            ILogger logger,
            IThreadGuard threadGuard)
        {
            if (wurmGameClients == null) throw new ArgumentNullException("wurmGameClients");
            if (wurmConfigDirectories == null) throw new ArgumentNullException("wurmConfigDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            this.wurmGameClients = wurmGameClients;
            this.wurmConfigDirectories = wurmConfigDirectories;
            this.logger = logger;
            this.threadGuard = threadGuard;

            RefreshConfigs();
            wurmConfigDirectories.DirectoriesChanged += WurmConfigDirectoriesOnDirectoriesChanged;
        }

        public event EventHandler<EventArgs> ConfigsChanged;

        public void Refresh()
        {
            threadGuard.ValidateCurrentThread();
            foreach (var keyValuePair in nameToConfigMap)
            {
                keyValuePair.Value.Refresh();
            }
        }

        public IEnumerable<IWurmConfig> All
        {
            get
            {
                threadGuard.ValidateCurrentThread();
                return this.nameToConfigMap.Values.ToArray();
            }
        }

        public IWurmConfig GetConfig(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            threadGuard.ValidateCurrentThread();

            name = name.Trim().ToUpperInvariant();
            WurmConfig config;
            if (this.nameToConfigMap.TryGetValue(name, out config))
            {
                return config;
            }
            else
            {
                throw new WurmApiException(string.Format("Config with name {0} is not known to WurmApi.", name));
            }
        }

        protected virtual void OnConfigsChanged()
        {
            var handler = ConfigsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void RefreshConfigs()
        {
            bool anyChanges = false;
            var configNamesNormalized = wurmConfigDirectories.AllDirectoryNamesNormalized.ToArray();
            foreach (var configName in configNamesNormalized)
            {
                WurmConfig config;
                if (!nameToConfigMap.TryGetValue(configName, out config))
                {
                    try
                    {
                        FileInfo gameSettingsFileInfo =
                            new FileInfo(wurmConfigDirectories.GetGameSettingsFileFullPathForConfigName(configName));

                        config = new WurmConfig(gameSettingsFileInfo.FullName, wurmGameClients, threadGuard);
                        this.nameToConfigMap.Add(configName, config);
                        config.ConfigChanged += ConfigOnConfigChanged;

                        anyChanges = true;
                    }
                    catch (Exception exception)
                    {
                        logger.Log(LogLevel.Warn, "Exception whle attempting to create config for name: " + configName, exception);
                    }
                }
                else
                {
                    config.Refresh();
                }
            }
            var removedConfigNames = configNamesNormalized.Where(name => !nameToConfigMap.ContainsKey(name)).ToArray();
            foreach (var configName in removedConfigNames)
            {
                var config = nameToConfigMap[configName];
                config.Dispose();
                nameToConfigMap.Remove(configName);
                anyChanges = true;
            }
            if (anyChanges)
            {
                OnConfigsChanged();
            }
        }

        private void WurmConfigDirectoriesOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            RefreshConfigs();
        }

        private void ConfigOnConfigChanged(object sender, EventArgs eventArgs)
        {
            OnConfigsChanged();
        }

        public void Dispose()
        {
            foreach (var wurmConfig in this.nameToConfigMap)
            {
                wurmConfig.Value.ConfigChanged -= ConfigOnConfigChanged;
                wurmConfig.Value.Dispose();
            }
        }
    }
}
