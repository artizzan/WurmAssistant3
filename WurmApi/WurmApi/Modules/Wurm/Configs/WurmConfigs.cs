using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Configs
{
    public class WurmConfigs : IWurmConfigs, IDisposable
    {
        readonly IWurmConfigDirectories wurmConfigDirectories;
        readonly ILogger logger;
        readonly IEventMarshaller eventMarshaller;

        IReadOnlyDictionary<string, WurmConfig> nameToConfigMap = new Dictionary<string, WurmConfig>();

        readonly RepeatableThreadedOperation rebuilder;

        public WurmConfigs(
            [NotNull] IWurmConfigDirectories wurmConfigDirectories,
            [NotNull] ILogger logger, [NotNull] IEventMarshaller eventMarshaller)
        {
            if (wurmConfigDirectories == null) throw new ArgumentNullException("wurmConfigDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            if (eventMarshaller == null) throw new ArgumentNullException("eventMarshaller");
            this.wurmConfigDirectories = wurmConfigDirectories;
            this.logger = logger;
            this.eventMarshaller = eventMarshaller;

            rebuilder = new RepeatableThreadedOperation(() =>
            {
                bool anyChanges = false;
                var configNamesNormalized = wurmConfigDirectories.AllDirectoryNamesNormalized.ToArray();
                var oldMap = nameToConfigMap;
                var newMap = new Dictionary<string, WurmConfig>();
                foreach (var configName in configNamesNormalized)
                {
                    WurmConfig config;
                    if (!oldMap.TryGetValue(configName, out config))
                    {
                        try
                        {
                            FileInfo gameSettingsFileInfo =
                                new FileInfo(wurmConfigDirectories.GetGameSettingsFileFullPathForConfigName(configName));

                            config = new WurmConfig(gameSettingsFileInfo.FullName, logger,
                                eventMarshaller);
                            config.ConfigChanged += ConfigOnConfigChanged;

                            newMap.Add(configName, config);
                            anyChanges = true;
                        }
                        catch (Exception exception)
                        {
                            logger.Log(LogLevel.Warn,
                                "Exception whle attempting to create config for name: " + configName, this, exception);
                        }
                    }
                    else
                    {
                        newMap.Add(configName, config);
                    }

                }
                var removedConfigNames = configNamesNormalized.Where(name => !oldMap.ContainsKey(name)).ToArray();
                foreach (var configName in removedConfigNames)
                {
                    var config = oldMap[configName];
                    config.ConfigChanged -= ConfigOnConfigChanged;
                    config.Dispose();
                    newMap.Remove(configName);
                    anyChanges = true;
                }
                if (anyChanges)
                {
                    nameToConfigMap = newMap;
                    OnConfigsChanged();
                }
            });

            rebuilder.OperationError +=
                (sender, args) =>
                {
                    const int retryDelay = 5;
                    logger.Log(LogLevel.Error,
                        string.Format("Error during configs rebuild, retrying in {0} seconds", retryDelay), this, args.Exception);
                    rebuilder.DelayedSignal(TimeSpan.FromSeconds(retryDelay));
                };

            wurmConfigDirectories.DirectoriesChanged += WurmConfigDirectoriesOnDirectoriesChanged;
            rebuilder.Signal();
            rebuilder.WaitSynchronouslyForInitialOperation(TimeSpan.FromSeconds(30));
        }

        private void WurmConfigDirectoriesOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            rebuilder.Signal();
        }

        public event EventHandler<EventArgs> ConfigsChanged;

        public IEnumerable<IWurmConfig> All
        {
            get
            {
                return this.nameToConfigMap.Values.ToArray();
            }
        }

        public IWurmConfig GetConfig([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            name = name.Trim().ToUpperInvariant();
            WurmConfig config;
            if (!this.nameToConfigMap.TryGetValue(name, out config))
            {
                throw new DataNotFoundException("Config not found for name: " + name);
            }
            return config;
        }

        protected virtual void OnConfigsChanged()
        {
            try
            {
                var handler = ConfigsChanged;
                if (handler != null)
                {
                    eventMarshaller.Marshal(() => handler(this, EventArgs.Empty));
                }
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "OnConfigsChanged delegate threw exception", this, exception);
            }
        }

        private void ConfigOnConfigChanged(object sender, EventArgs eventArgs)
        {
            OnConfigsChanged();
        }

        public void Dispose()
        {
            wurmConfigDirectories.DirectoriesChanged -= WurmConfigDirectoriesOnDirectoriesChanged;
            rebuilder.Dispose();
            foreach (var wurmConfig in this.nameToConfigMap)
            {
                wurmConfig.Value.ConfigChanged -= ConfigOnConfigChanged;
                wurmConfig.Value.Dispose();
            }
        }
    }
}
