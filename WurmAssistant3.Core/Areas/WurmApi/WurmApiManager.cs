using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.InstallDirectory;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using AldursLab.WurmAssistant3.Core.Areas.Logging;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Model;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi.Model;
using AldursLab.WurmAssistant3.Core.IoC;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.WurmApi
{
    public static class WurmApiManager
    {
        public static void AutodetectInstallDirectory(IKernel kernel)
        {
            try
            {
                // attempt to autodetect directory and bind the result
                // assume if consumer can't resolve, will try to bind in alternative way
                var dir = WurmClientInstallDirectory.AutoDetect();
                kernel.Bind<IWurmClientInstallDirectory>().ToConstant(dir);
            }
            catch (Exception exception)
            {
                kernel.LogCoreInfo(exception,
                    "Autodetection of Wurm Online Game Client failed. Reason: " + exception.Message);
            }
        }

        public static void BindSingletonApi([NotNull] IKernel kernel)
        {
            var api = ConstructWurmApi(kernel);
            kernel.Bind<IWurmApi>().ToConstant(api);
        }

        static IWurmApi ConstructWurmApi(IKernel kernel)
        {
            WurmAssistantSettings settings = kernel.Get<WurmAssistantSettings>();
            IWurmApiLoggerFactory loggerFactory = kernel.Get<IWurmApiLoggerFactory>();
            IWurmAssistantDataDirectory dataDirectory = kernel.Get<IWurmAssistantDataDirectory>();
            IWurmApiEventMarshaller eventMarshaller = kernel.Get<IWurmApiEventMarshaller>();

            IWurmClientInstallDirectory wurmInstallDirectory = kernel.Get<IWurmClientInstallDirectory>();

            WurmApiConfig wurmApiConfig;
            if (settings.RunningPlatform != Platform.Unknown)
            {
                wurmApiConfig = new WurmApiConfig { Platform = settings.RunningPlatform };
            }
            else
            {
                throw new InvalidOperationException("config.RunningPlatform is Unknown");
            }

            var wurmApi = AldursLab.WurmApi.WurmApiFactory.Create(Path.Combine(dataDirectory.DirectoryPath, "WurmApi"),
                loggerFactory.Create(),
                eventMarshaller,
                wurmInstallDirectory,
                wurmApiConfig);

            return wurmApi;
        }
    }
}
