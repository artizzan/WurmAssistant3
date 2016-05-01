using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Modules.Events.Public;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Configs
{
    class WurmConfigs : IWurmConfigs, IDisposable, IHandle<ConfigDirectoriesChanged>
    {
        readonly IWurmConfigDirectories wurmConfigDirectories;
        readonly IWurmApiLogger logger;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IInternalEventAggregator eventAggregator;
        readonly TaskManager taskManager;

        IReadOnlyDictionary<string, WurmConfig> nameToConfigMap = new Dictionary<string, WurmConfig>();

        readonly object locker = new object();

        readonly PublicEvent onAvailableConfigsChanged;
        readonly PublicEvent onAnyConfigChanged;

        readonly TaskHandle taskHandle;

        public event EventHandler<EventArgs> AvailableConfigsChanged;
        public event EventHandler<EventArgs> AnyConfigChanged;

        internal WurmConfigs(
            [NotNull] IWurmConfigDirectories wurmConfigDirectories,
            [NotNull] IWurmApiLogger logger, [NotNull] IPublicEventInvoker publicEventInvoker,
            [NotNull] IInternalEventAggregator eventAggregator, [NotNull] TaskManager taskManager)
        {
            if (wurmConfigDirectories == null) throw new ArgumentNullException(nameof(wurmConfigDirectories));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (publicEventInvoker == null) throw new ArgumentNullException(nameof(publicEventInvoker));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (taskManager == null) throw new ArgumentNullException(nameof(taskManager));
            this.wurmConfigDirectories = wurmConfigDirectories;
            this.logger = logger;
            this.publicEventInvoker = publicEventInvoker;
            this.eventAggregator = eventAggregator;
            this.taskManager = taskManager;

            onAvailableConfigsChanged = publicEventInvoker.Create(
                () => AvailableConfigsChanged.SafeInvoke(this, EventArgs.Empty), 
                WurmApiTuningParams.PublicEventMarshallerDelay);
            onAnyConfigChanged = publicEventInvoker.Create(
                () => AnyConfigChanged.SafeInvoke(this, EventArgs.Empty),
                WurmApiTuningParams.PublicEventMarshallerDelay);

            try
            {
                Refresh();
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Error at initial configs update", this, exception);
            }

            taskHandle = new TaskHandle(Refresh, "WurmConfigs update");
            taskManager.Add(taskHandle);

            eventAggregator.Subscribe(this);

            taskHandle.Trigger();
        }

        void Refresh()
        {
            lock (locker)
            {
                bool anyChanges = false;
                var configNamesNormalized = wurmConfigDirectories.AllDirectoryNamesNormalized.ToArray();
                var oldMap = nameToConfigMap;
                var newMap = new Dictionary<string, WurmConfig>();
                List<Exception> configCreationErrors = new List<Exception>();

                foreach (var configName in configNamesNormalized)
                {
                    WurmConfig config;
                    if (!oldMap.TryGetValue(configName, out config))
                    {
                        try
                        {
                            FileInfo gameSettingsFileInfo =
                                new FileInfo(
                                    wurmConfigDirectories.GetGameSettingsFileFullPathForConfigName(
                                        configName));

                            config = new WurmConfig(gameSettingsFileInfo.FullName,
                                publicEventInvoker,
                                taskManager,
                                logger);

                            config.ConfigChanged += ConfigOnConfigChanged;

                            newMap.Add(configName, config);
                            anyChanges = true;
                        }
                        catch (Exception exception)
                        {
                            configCreationErrors.Add(exception);
                        }
                    }
                    else
                    {
                        newMap.Add(configName, config);
                    }
                }
                if (anyChanges)
                {
                    nameToConfigMap = newMap;
                    onAvailableConfigsChanged.Trigger();
                }

                if (configCreationErrors.Any())
                {
                    throw new AggregateException(configCreationErrors);
                }
            }
        }

        public void Handle(ConfigDirectoriesChanged message)
        {
            taskHandle.Trigger();
        }

        public IEnumerable<IWurmConfig> All => nameToConfigMap.Values.ToArray();

        public IWurmConfig GetConfig([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            name = name.Trim().ToUpperInvariant();
            WurmConfig config;
            if (!nameToConfigMap.TryGetValue(name, out config))
            {
                throw new DataNotFoundException("Config not found for name: " + name);
            }
            return config;
        }

        private void ConfigOnConfigChanged(object sender, EventArgs eventArgs)
        {
            onAnyConfigChanged.Trigger();
        }

        public void Dispose()
        {
            lock (locker)
            {
                taskManager.Remove(taskHandle);

                eventAggregator.Unsubscribe(this);
                onAnyConfigChanged.Detach();
                onAvailableConfigsChanged.Detach();
                foreach (var wurmConfig in nameToConfigMap)
                {
                    wurmConfig.Value.ConfigChanged -= ConfigOnConfigChanged;
                    wurmConfig.Value.Dispose();
                }
            }
        }
    }
}
