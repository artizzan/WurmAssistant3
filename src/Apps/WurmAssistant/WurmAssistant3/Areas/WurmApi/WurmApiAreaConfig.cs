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
    public class WurmApiAreaConfig : AreaConfig
    {
        public override void Configure(IKernel kernel)
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

                if (string.IsNullOrWhiteSpace(config.WurmGameClientInstallDirectory))
                {
                    throw new InvalidOperationException("Unknown path to Wurm Game Client installation folder.");
                }

                IWurmClientInstallDirectory wurmInstallDirectory =
                    new WurmInstallDirectoryOverride(config.WurmGameClientInstallDirectory);
                ServerInfoManager serverInfoManager = kernel.Get<ServerInfoManager>();

                var wurmApiConfig = new WurmApiConfig
                {
                    Platform = Platform.Windows,
                    ClearAllCaches = config.DropAllWurmApiCachesToggle,
                    WurmUnlimitedMode = config.WurmUnlimitedMode
                };
                serverInfoManager.UpdateWurmApiConfigDictionary(wurmApiConfig.ServerInfoMap);

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
