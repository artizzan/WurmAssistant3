using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Modules
{
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
        public bool IsNullSound { get { return true; } }
    }
}