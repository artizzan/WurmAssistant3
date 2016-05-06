using System;
using System.Collections.Generic;
using System.Media;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Root.Contracts;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Modules.BuiltIn
{
    class DefaultSoundEngine : ISoundEngine
    {
        readonly List<WeakReference<SoundPlayer>> soundPlayers = new List<WeakReference<SoundPlayer>>();
        int counter;

        public DefaultSoundEngine(IUpdateLoop updateLoop)
        {
            updateLoop.Updated += (sender, args) =>
            {
                counter++;
                if (counter > 100)
                {
                    counter = 0;
                    CleanupWeakRefs();
                }
            };
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
