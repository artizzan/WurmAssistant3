using System;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using IrrKlang;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Modules
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
    }

    class PlayingSoundHandleNullObject : IPlayingSoundHandle
    {
        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void Stop()
        {
        }

        public bool IsFinished { get { return true; } }
        public bool IsPaused { get; private set; }
        public float CurrentVolume { get; set; }
    }
}