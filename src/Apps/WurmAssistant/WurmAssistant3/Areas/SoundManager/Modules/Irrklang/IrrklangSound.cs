using System;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Modules.Irrklang
{
    class IrrklangSound : ISound
    {
        readonly IrrKlang.ISound sound;

        public IrrklangSound(IrrKlang.ISound sound)
        {
            if (sound == null) throw new ArgumentNullException("sound");
            this.sound = sound;
        }

        public bool Paused
        {
            get { return sound.Paused; }
            set { sound.Paused = value; }
        }

        public bool ReportsFinished { get { return true; } }

        public bool Finished
        {
            get { return sound.Finished; }
        }
        public void Stop()
        {
            sound.Stop();
        }

        public float Volume
        {
            get { return sound.Volume; }
            set { sound.Volume = value; }
        }
    }
}