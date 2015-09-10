using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using AldursLab.WurmAssistant3.Core.Areas.Config.ViewModels;
using AldursLab.WurmAssistant3.Core.Areas.Config.Views;
using AldursLab.WurmAssistant3.Core.Areas.PersistentData;
using AldursLab.WurmAssistant3.Core.Areas.PersistentData.Model;
using AldursLab.WurmAssistant3.Core.Areas.Root.Model;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi.Model;
using AldursLab.WurmAssistant3.Core.IoC;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Config
{
    public static class ConfigManager
    {
        static readonly WurmAssistantDataDirectory DataDirectory;

        static ConfigManager()
        {
            DataDirectory = new WurmAssistantDataDirectory();
        }

        public static void BindDataDirectory(IKernel kernel)
        {
            kernel.ProhibitGet<WurmAssistantDataDirectory>();
            kernel.Bind<IWurmAssistantDataDirectory>().ToConstant(DataDirectory);
        }

        public static void LockDataDirectory()
        {
            DataDirectory.Lock();
        }

        public static void BindApplicationSettings(IKernel kernel)
        {
            var persistent = kernel.Get<IPersistentFactory>().Get<WurmAssistantSettings.PersistentData>();
            var settings = new WurmAssistantSettings(persistent);

            kernel.Bind<WurmAssistantSettings>().ToConstant(settings);
        }

        public static void ConfigureApplication(IKernel kernel)
        {
            var host = kernel.Get<IHostEnvironment>();
            var settings = kernel.Get<WurmAssistantSettings>();
            var wurmClientInstallDir = kernel.TryGet<IWurmClientInstallDirectory>();

            if (settings.RunningPlatform == Platform.Unknown)
            {
                if (host.Platform != Platform.Unknown)
                {
                    settings.RunningPlatform = host.Platform;
                }
            }
            bool platformKnown = settings.RunningPlatform != Platform.Unknown;

            if (wurmClientInstallDir != null)
            {
                settings.WurmGameClientInstallDirectory = wurmClientInstallDir.FullPath;
            }
            bool installDirKnown = !string.IsNullOrWhiteSpace(settings.WurmGameClientInstallDirectory);

            bool reconfigRequested = settings.RecondingRequested;

            if (reconfigRequested || !platformKnown || !installDirKnown)
            {
                // run setup;
                var view = new SetupView(kernel.Get<SetupViewModel>());
                if (view.ShowDialog() != DialogResult.OK)
                {
                    throw new ApplicationException("Configuration dialog was cancelled by user");
                }
                wurmClientInstallDir = new WurmInstallDirectoryOverride();
            }

            if (wurmClientInstallDir == null || wurmClientInstallDir.FullPath != settings.WurmGameClientInstallDirectory)
            {
                wurmClientInstallDir = new WurmInstallDirectoryOverride()
                {
                    FullPath = settings.WurmGameClientInstallDirectory
                };
                kernel.Rebind<IWurmClientInstallDirectory>().ToConstant(wurmClientInstallDir);
            }
        }

        public static void UnlockDataDirectory()
        {
            DataDirectory.Unlock();
        }
    }
}
