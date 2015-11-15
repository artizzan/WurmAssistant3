using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.LogSearcher.Modules;
using AldursLab.WurmAssistant3.Core.Areas.LogSearcher.Views;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.LogSearcher
{
    public static class LogSearcherSetup
    {
        public static void BindLogSearcher(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<LogSearchFeature>().InSingletonScope().Named("LogSearcher");
        }
    }
}
