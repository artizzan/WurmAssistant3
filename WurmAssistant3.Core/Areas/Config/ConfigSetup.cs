using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Config.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Config.Views;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi.Modules;
using AldursLab.WurmAssistant3.Core.IoC;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using Ninject;
using Ninject.Extensions.Factory;

namespace AldursLab.WurmAssistant3.Core.Areas.Config
{
    public static class ConfigSetup
    {
        public static void BindComponents(IKernel kernel)
        {
            kernel.Bind<ISettingsEditViewFactory>().ToFactory();
            kernel.Bind<SettingsEditView>().ToSelf();

            kernel.Bind<IServersEditorViewFactory>().ToFactory();
            kernel.Bind<ServerInfoManager>().ToSelf().InSingletonScope();
        }

        public static void ConfigureApplication(IKernel kernel)
        {
            var host = kernel.Get<IHostEnvironment>();
            var settings = kernel.Get<WurmAssistantConfig>();
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

            if (settings.ReSetupRequested || !platformKnown || !installDirKnown)
            {
                // run setup;
                var view = new FirstTimeSetupView(kernel.Get<WurmAssistantConfig>());
                if (view.ShowDialog() != DialogResult.OK)
                {
                    throw new ConfigCancelledException("Configuration dialog was cancelled by user");
                }
                settings.ReSetupRequested = false;
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
    }
}
