using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Features;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.SoundManager
{
    public class AreaConfiguration : IAreaConfiguration
    {
        public void Configure(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<SoundManagerFeature>().InSingletonScope().Named("Sounds Manager");
        }
    }
}