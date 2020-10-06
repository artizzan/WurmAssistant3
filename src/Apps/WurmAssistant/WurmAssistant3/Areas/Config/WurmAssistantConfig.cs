using System;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.WurmApi;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Config
{
    [KernelBind(BindingHint.Singleton), PersistentObject("WurmAssistantConfig")]
    public class WurmAssistantConfig : PersistentObjectBase, IWurmAssistantConfig
    {
        readonly IConsoleArgs consoleArgs;

        [JsonProperty]
        int version = 2;

        [JsonProperty]
        string wurmGameClientInstallDirectory;

        [JsonProperty, Obsolete("Back to supporting windows only")]
        Platform runningPlatform;

        [JsonProperty("reSetupRequested")]
        bool wurmApiResetRequested;

        [JsonProperty]
        bool dropAllWurmApiCachesToggle = false;

        [JsonProperty]
        bool skipWurmConfigsValidation;

        [JsonProperty]
        Guid installationId = Guid.NewGuid();

        [JsonProperty]
        bool allowInsights = true;

        [JsonProperty]
        bool useTopRightPopupStrategy = false;

        public WurmAssistantConfig([NotNull] IConsoleArgs consoleArgs)
        {
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            this.consoleArgs = consoleArgs;
        }

        protected override void OnPersistentDataLoaded()
        {
            if (version == 0)
            {
                DropAllWurmApiCachesToggle = true;
                Version = 1;
                FlagAsChanged();
            }
            if (version == 1)
            {
                InstallationId = Guid.NewGuid();
                Version = 2;
            }

            if (this.WurmApiResetRequested || WurmGameClientInstallDirectory.IsNullOrEmpty())
            {
                // run setup;
                var view = new WurmApiSetupForm(WurmGameClientInstallDirectory, WurmUnlimitedMode);
                if (view.ShowDialog() != DialogResult.OK)
                {
                    throw new ConfigCancelledException("Configuration dialog was cancelled by user");
                }

                if (WurmGameClientInstallDirectory != view.SelectedWurmInstallDirectory)
                {
                    WurmGameClientInstallDirectory = view.SelectedWurmInstallDirectory;
                    DropAllWurmApiCachesToggle = true;
                }

                WurmApiResetRequested = false;
            }
        }

        public bool WurmUnlimitedMode => consoleArgs.WurmUnlimitedMode;

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

        public bool WurmApiResetRequested
        {
            get { return wurmApiResetRequested; }
            set
            {
                if (value == wurmApiResetRequested)
                    return;
                wurmApiResetRequested = value;
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

        public bool SkipWurmConfigsValidation
        {
            get { return skipWurmConfigsValidation; }
            set
            {
                if (value == skipWurmConfigsValidation)
                    return;
                skipWurmConfigsValidation = value;
                FlagAsChanged();
            }
        }

        public Guid InstallationId
        {
            get { return installationId; }
            set
            {
                if (value == installationId)
                    return;
                installationId = value;
                FlagAsChanged();
            }
        }

        public bool AllowInsights
        {
            get { return allowInsights; }
            set
            {
                if (value == allowInsights)
                    return;
                allowInsights = value;
                FlagAsChanged();
            }
        }

        public bool UseTopRightPopupStrategy
        {
            get { return useTopRightPopupStrategy; }
            set
            {
                if (value == useTopRightPopupStrategy)
                    return;
                useTopRightPopupStrategy = value;
                FlagAsChanged();
            }
        }

        int Version
        {
            get { return version; }
            set
            {
                if (value == version)
                    return;
                version = value;
                FlagAsChanged();
            }
        }
    }
}