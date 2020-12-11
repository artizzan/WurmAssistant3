using System;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager.BuiltIn;
using AldursLab.WurmAssistant3.Areas.SoundManager.Irrklang;

namespace AldursLab.WurmAssistant3.Areas.SoundManager
{
    [KernelBind(BindingHint.Singleton)]
    class SoundEngine : ISoundEngine
    {
        private readonly ISoundEngine soundEngine;

        public SoundEngine(ITimerFactory timerFactory, ILogger logger)
        {
            try
            {
                soundEngine = new IrrklangSoundEngineProxy();
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to create preferred sound engine, falling back to backup engine.");
                soundEngine = new DefaultSoundEngine(timerFactory);
            }
        }

        public ISound Play2D(string soundFilename, bool playLooped, bool startPaused)
        {
            return soundEngine.Play2D(soundFilename, playLooped, startPaused);
        }

        public float SoundVolume
        {
            get => soundEngine.SoundVolume;
            set => soundEngine.SoundVolume = value;
        }

        public void StopAllSounds()
        {
            soundEngine.StopAllSounds();
        }
    }
}