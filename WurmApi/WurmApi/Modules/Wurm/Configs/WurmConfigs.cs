using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AldursLab.Essentials.Extensions.DotNet;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Configs
{
    public class WurmConfigs : IWurmConfigs, IDisposable, IHandle<ConfigDirectoriesChanged>
    {
        readonly IWurmConfigDirectories wurmConfigDirectories;
        readonly ILogger logger;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IInternalEventAggregator eventAggregator;

        IReadOnlyDictionary<string, WurmConfig> nameToConfigMap = new Dictionary<string, WurmConfig>();

        volatile int rebuildPending = 1;
        readonly object locker = new object();

        readonly PublicEvent onAvailableConfigsChanged;
        readonly PublicEvent onAnyConfigChanged;

        internal WurmConfigs(
            [NotNull] IWurmConfigDirectories wurmConfigDirectories,
            [NotNull] ILogger logger, [NotNull] IPublicEventInvoker publicEventInvoker,
            [NotNull] IInternalEventAggregator eventAggregator)
        {
            if (wurmConfigDirectories == null) throw new ArgumentNullException("wurmConfigDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            this.wurmConfigDirectories = wurmConfigDirectories;
            this.logger = logger;
            this.publicEventInvoker = publicEventInvoker;
            this.eventAggregator = eventAggregator;

            onAvailableConfigsChanged = publicEventInvoker.Create(
                () => AvailableConfigsChanged.SafeInvoke(this), 
                WurmApiTuningParams.PublicEventMarshallerDelay);
            onAnyConfigChanged = publicEventInvoker.Create(
                () => AnyConfigChanged.SafeInvoke(this),
                WurmApiTuningParams.PublicEventMarshallerDelay);

            eventAggregator.Subscribe(this);
        }

        public event EventHandler<EventArgs> AvailableConfigsChanged;

        public event EventHandler<EventArgs> AnyConfigChanged;

        public void Handle(ConfigDirectoriesChanged message)
        {
            rebuildPending = 1;
            onAvailableConfigsChanged.Trigger();
        }

        public IEnumerable<IWurmConfig> All
        {
            get
            {
                Refresh();
                return this.nameToConfigMap.Values.ToArray();
            }
        }

        public IWurmConfig GetConfig([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            Refresh();

            name = name.Trim().ToUpperInvariant();
            WurmConfig config;
            if (!this.nameToConfigMap.TryGetValue(name, out config))
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
                eventAggregator.Unsubscribe(this);
                onAnyConfigChanged.Detach();
                onAvailableConfigsChanged.Detach();
                foreach (var wurmConfig in this.nameToConfigMap)
                {
                    wurmConfig.Value.ConfigChanged -= ConfigOnConfigChanged;
                    wurmConfig.Value.Dispose();
                }
            }
        }

        private void Refresh()
        {
            if (rebuildPending == 1)
            {
                lock (locker)
                {
                    if (Interlocked.CompareExchange(ref rebuildPending, 0, 1) == 1)
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
                                FileInfo gameSettingsFileInfo =
                                    new FileInfo(wurmConfigDirectories.GetGameSettingsFileFullPathForConfigName(configName));

                                config = new WurmConfig(gameSettingsFileInfo.FullName, logger, publicEventInvoker);
                                config.ConfigChanged += ConfigOnConfigChanged;

                                newMap.Add(configName, config);
                                anyChanges = true;
                            }
                            else
                            {
                                newMap.Add(configName, config);
                            }
                        }
                        if (anyChanges)
                        {
                            nameToConfigMap = newMap;
                        }
                    }
                }
            }
        }
    }
}
