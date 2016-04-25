using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules.Stub
{
    class StubSoundEngine : ISoundEngine
    {
        public ISound Play2D(string soundFilename, bool playLooped, bool startPaused)
        {
            return new StubSound();
        }

        public float SoundVolume { get; set; }
        public void StopAllSounds()
        {
        }
    }
}
