using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.LogSearcher.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.LogSearcher
{
    public static class LogSearcherSetup
    {
        public static void BindLogSearcher(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<LogSearchFeature>().InSingletonScope().Named("LogSearcher");
        }
    }
}
