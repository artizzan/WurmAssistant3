namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts
{
    /// <summary>
    /// Represents a handle to a playable sound instance.
    /// Can be used to control this particular sound instance playback.
    /// </summary>
    public interface IPlayingSoundHandle
    {
        void Pause();
        void Resume();
        void Stop();
        bool IsFinished { get; }
        bool IsPaused { get; }
        float CurrentVolume { get; set; }
        bool IsNullSound { get; }
    }
}