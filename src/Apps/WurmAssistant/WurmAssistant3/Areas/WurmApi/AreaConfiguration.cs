using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Config.Services;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.WurmApi.Parts;
using AldursLab.WurmAssistant3.Areas.WurmApi.Services;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.WurmApi
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IWurmApi>().ToMethod(BuildWurmApiFactory(kernel)).InSingletonScope();
        }

        private static Func<Ninject.Activation.IContext, IWurmApi> BuildWurmApiFactory(IKernel kernel)
        {
            return context =>
            {
                IWurmAssistantConfig config = kernel.Get<IWurmAssistantConfig>();
                IWurmApiLoggerFactory loggerFactory = kernel.Get<IWurmApiLoggerFactory>();
                IWurmApiEventMarshaller eventMarshaller = kernel.Get<IWurmApiEventMarshaller>();

                IWurmClientInstallDirectory wurmInstallDirectory =
                    new WurmInstallDirectoryOverride(config.WurmGameClientInstallDirectory);
                ServerInfoManager serverInfoManager = kernel.Get<ServerInfoManager>();

                WurmApiConfig wurmApiConfig;
                if (config.RunningPlatform != Platform.Unknown)
                {
                    wurmApiConfig = new WurmApiConfig
                    {
                        Platform = config.RunningPlatform,
                        ClearAllCaches = config.DropAllWurmApiCachesToggle,
                        WurmUnlimitedMode = config.WurmUnlimitedMode
                    };
                    serverInfoManager.UpdateWurmApiConfigDictionary(wurmApiConfig.ServerInfoMap);
                }
                else
                {
                    throw new InvalidOperationException("config.RunningPlatform is Unknown");
                }

                var wurmApiDataDir = new DirectoryInfo(Path.Combine(config.WurmGameClientInstallDirectory, "WurmApi"));

                var wurmApi = AldursLab.WurmApi.WurmApiFactory.Create(
                    new WurmApiCreationOptions()
                    {
                        DataDirPath = wurmApiDataDir.FullName,
                        WurmApiLogger = loggerFactory.Create(),
                        WurmApiEventMarshaller = eventMarshaller,
                        WurmClientInstallDirectory = wurmInstallDirectory,
                        WurmApiConfig = wurmApiConfig
                    });

                config.DropAllWurmApiCachesToggle = false;

                var validator = new WurmClientValidator(wurmApi);
                if (!validator.SkipOnStart)
                {
                    var issues = validator.Validate();
                    if (issues.Any()) validator.ShowSummaryWindow(issues);
                }

                return wurmApi;
            };
        }
    }
}
