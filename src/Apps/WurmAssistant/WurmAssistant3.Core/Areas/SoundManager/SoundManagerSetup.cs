using System.IO;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules.BuiltIn;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules.Irrklang;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules.Stub;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundManager
{
    public static class SoundManagerSetup
    {
        public static void Bind(IKernel kernel)
        {
            var dataDirectory = kernel.Get<IWurmAssistantDataDirectory>();
            var config = kernel.Get<IWurmAssistantConfig>();
            if (config.RunningPlatform == Platform.Windows)
            {
                kernel.Bind<ISoundEngine>().To<IrrklangSoundEngineProxy>().InSingletonScope();
            }
            else
            {
                kernel.Bind<ISoundEngine>().To<DefaultSoundEngine>().InSingletonScope();
            }

            kernel.Bind<Modules.SoundManager, ISoundManager>()
                  .To<Modules.SoundManager>()
                  .InSingletonScope();

            kernel.Bind<ISoundsLibrary>()
                  .To<SoundsLibrary>()
                  .InSingletonScope()
                  .WithConstructorArgument("soundFilesPath", Path.Combine(dataDirectory.DirectoryPath, "SoundBank"));

            kernel.Bind<IFeature>().ToMethod(context => context.Kernel.Get<Modules.SoundManager>()).Named("Sounds Manager");
        }
    }
}
