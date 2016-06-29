namespace AldursLab.WurmAssistant3.Areas.SoundManager.Stub
{
    class StubSound : ISound
    {
        public bool Paused { get; set; }

        public bool ReportsFinished { get { return true; } }

        public bool Finished { get { return true; } }

        public void Stop()
        {
        }

        public float Volume { get; set; }
    }
}