using System;
using System.Collections.Generic;
using System.Media;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Parts.BuiltIn
{
    class DefaultSoundEngine : ISoundEngine
    {
        readonly List<WeakReference<SoundPlayer>> soundPlayers = new List<WeakReference<SoundPlayer>>();

        public DefaultSoundEngine([NotNull] ITimerFactory timerFactory)
        {
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));

            var timer = timerFactory.CreateUiThreadTimer();
            timer.Interval = TimeSpan.FromSeconds(50);
            timer.Tick += (sender, args) => CleanupWeakRefs();
            timer.Start();
        }

        void CleanupWeakRefs()
        {
            foreach (var weakReference in soundPlayers.ToArray())
            {
                SoundPlayer sb;
                if (!weakReference.TryGetTarget(out sb))
                {
                    soundPlayers.Remove(weakReference);
                }
            }
        }

        public ISound Play2D(string soundFilename, bool playLooped, bool startPaused)
        {
            var player = new SoundPlayer(soundFilename);
            if (playLooped)
            {
                player.PlayLooping();
            }
            else
            {
                player.Play();
            }
            
            soundPlayers.Add(new WeakReference<SoundPlayer>(player));

            return new DefaultSound(player);
        }

        public float SoundVolume { get; set; }

        public void StopAllSounds()
        {
            foreach (var weakReference in soundPlayers)
            {
                SoundPlayer sb;
                if (weakReference.TryGetTarget(out sb))
                {
                    sb.Stop();
                }
            }
        }
    }
}
