using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules.Irrklang
{
    class IrrklangSoundEngineProxy : ISoundEngine
    {
        readonly IrrKlang.ISoundEngine engine;

        public IrrklangSoundEngineProxy()
        {
            engine = new IrrKlang.ISoundEngine();
        }

        public float SoundVolume { get; set; }

        public ISound Play2D(string soundFilename, bool playLooped, bool startPaused)
        {
            return new IrrklangSound(engine.Play2D(soundFilename, playLooped, startPaused));
        }

        public void StopAllSounds()
        {
            engine.StopAllSounds();
        }
    }
}
