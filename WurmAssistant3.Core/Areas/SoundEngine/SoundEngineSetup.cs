using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine
{
    public static class SoundEngineSetup
    {
        public static void BindSoundEngine(IKernel kernel)
        {
            kernel.Bind<ISoundEngine>().To<Modules.SoundEngine>().InSingletonScope();
        }
    }
}
