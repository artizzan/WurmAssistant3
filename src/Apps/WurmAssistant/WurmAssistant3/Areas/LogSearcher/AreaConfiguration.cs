using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.LogSearcher.Features;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.LogSearcher
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<LogSearchFeature>().InSingletonScope().Named("LogSearcher");
        }
    }
}