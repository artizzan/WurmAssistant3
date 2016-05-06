using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Modules.Stub
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
