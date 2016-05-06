using System;
using System.Media;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Modules.BuiltIn
{
    class DefaultSound : ISound
    {
        readonly SoundPlayer player;

        public DefaultSound(SoundPlayer player)
        {
            if (player == null) throw new ArgumentNullException("player");
            this.player = player;
        }

        public bool Paused { get; set; }

        public bool ReportsFinished { get { return false; } }

        public bool Finished
        {
            get { return false; }
        }

        public void Stop()
        {
            player.Stop();
        }

        public float Volume { get; set; }
    }
}