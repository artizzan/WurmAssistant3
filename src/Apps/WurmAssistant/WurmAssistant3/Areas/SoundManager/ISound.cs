namespace AldursLab.WurmAssistant3.Areas.SoundManager
{
    public interface ISound
    {
        bool Paused { get; set; }
        bool ReportsFinished { get; }
        bool Finished { get; }
        void Stop();
        float Volume { get; set; }
    }
}