using System;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Components.Singletons;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Config.Modules
{
    [PersistentObject("WurmAssistantConfig")]
    public class WurmAssistantConfig : PersistentObjectBase, IWurmAssistantConfig
    {
        readonly IConsoleArgs consoleArgs;

        [JsonProperty]
        int version = 0;

        [JsonProperty]
        string wurmGameClientInstallDirectory;

        [JsonProperty]
        Platform runningPlatform;

        [JsonProperty]
        bool reSetupRequested;

        [JsonProperty]
        bool dropAllWurmApiCachesToggle;

        public WurmAssistantConfig([NotNull] IConsoleArgs consoleArgs)
        {
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            this.consoleArgs = consoleArgs;
        }

        protected override void OnPersistentDataLoaded()
        {
            if (version == 0)
            {
                dropAllWurmApiCachesToggle = true;
                version = 1;
            }
        }

        public string WurmGameClientInstallDirectory
        {
            get { return wurmGameClientInstallDirectory; }
            set
            {
                if (value == wurmGameClientInstallDirectory)
                    return;
                wurmGameClientInstallDirectory = value;
                FlagAsChanged();
            }
        }

        public Platform RunningPlatform
        {
            get { return runningPlatform; }
            set
            {
                if (value == runningPlatform)
                    return;
                runningPlatform = value;
                FlagAsChanged();
            }
        }

        public bool ReSetupRequested
        {
            get { return reSetupRequested; }
            set
            {
                if (value == reSetupRequested)
                    return;
                reSetupRequested = value;
                FlagAsChanged();
            }
        }

        public bool DropAllWurmApiCachesToggle
        {
            get { return dropAllWurmApiCachesToggle; }
            set
            {
                if (value == dropAllWurmApiCachesToggle)
                    return;
                dropAllWurmApiCachesToggle = value;
                FlagAsChanged();
            }
        }

        public bool WurmUnlimitedMode
        {
            get { return consoleArgs.WurmUnlimitedMode; }
        }
    }
}