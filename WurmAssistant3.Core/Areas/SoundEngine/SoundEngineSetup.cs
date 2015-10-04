using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Modules;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine
{
    public static class SoundEngineSetup
    {
        public static void BindSoundEngine(IKernel kernel)
        {
            var dataDirectory = kernel.Get<IWurmAssistantDataDirectory>();

            kernel.Bind<Modules.SoundEngine, ISoundEngine>()
                  .To<Modules.SoundEngine>()
                  .InSingletonScope();

            kernel.Bind<ISoundsLibrary>()
                  .To<SoundsLibrary>()
                  .InSingletonScope()
                  .WithConstructorArgument("soundFilesPath", Path.Combine(dataDirectory.DirectoryPath, "SoundBank"));

            kernel.Bind<IFeature>().ToMethod(context => context.Kernel.Get<Modules.SoundEngine>()).Named("Sounds Manager");
        }
    }
}
