using System;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.SoundManager
{
    class PlayingSoundHandle : IPlayingSoundHandle
    {
        readonly ISound soundHandle;

        public PlayingSoundHandle([NotNull] ISound soundHandle)
        {
            if (soundHandle == null) throw new ArgumentNullException("soundHandle");
            this.soundHandle = soundHandle;
        }

        public void Pause()
        {
            soundHandle.Paused = true;
        }

        public void Resume()
        {
            soundHandle.Paused = false;
        }

        public void Stop()
        {
            soundHandle.Stop();
        }

        public bool IsFinished
        {
            get { return soundHandle.Finished; }
        }

        public bool IsPaused
        {
            get { return soundHandle.Paused; }
        }

        public float CurrentVolume
        {
            get { return soundHandle.Volume; }
            set { soundHandle.Volume = value; }
        }

        public bool IsNullSound { get { return false; } }
    }
}