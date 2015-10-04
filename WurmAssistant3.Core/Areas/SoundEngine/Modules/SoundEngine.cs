using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Views;
using AldursLab.WurmAssistant3.Core.Resources;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;
using ISoundEngine = AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts.ISoundEngine;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Modules
{
    [PersistentObject("SoundEngine")]
    public class SoundEngine : PersistentObjectBase, ISoundEngine, IInitializable, IFeature
    {
        readonly ISoundsLibrary soundsLibrary;
        readonly IrrKlang.ISoundEngine engine;

        [JsonProperty]
        float globalVolume;

        [JsonProperty]
        bool globalMute;

        readonly SoundManagerView view;

        public SoundEngine([NotNull] ISoundsLibrary soundsLibrary)
        {
            if (soundsLibrary == null) throw new ArgumentNullException("soundsLibrary");
            this.soundsLibrary = soundsLibrary;

            engine = new IrrKlang.ISoundEngine();
            globalVolume = 0.5f;

            view = new SoundManagerView(this, soundsLibrary);
        }

        public void Initialize()
        {
            engine.SoundVolume = globalVolume;
        }

        public float GlobalVolume
        {
            get { return globalVolume; }
            set { globalVolume = value; engine.SoundVolume = globalVolume; FlagAsChanged(); }
        }

        public ISoundResource GetSoundById(Guid soundId)
        {
            var sound = soundsLibrary.TryGetSound(soundId);
            return sound ?? new SoundResourceNullObject();
        }

        public bool GlobalMute
        {
            get { return globalMute; }
            set { globalMute = value; FlagAsChanged(); }
        }

        public IPlayingSoundHandle PlayOneShot(ISoundResource soundResource)
        {
            return PlayOneShot(soundResource.Id);
        }

        public IPlayingSoundHandle PlayOneShot(Guid soundId)
        {
            var sound = soundsLibrary.TryGetSound(soundId);
            if (sound != null)
            {
                var soundHandle = engine.Play2D(sound.FileFullName, false, true);
                soundHandle.Volume = sound.AdjustedVolume;
                soundHandle.Paused = false;
                return new PlayingSoundHandle(soundHandle);
            }
            else
            {
                return new PlayingSoundHandleNullObject();
            }
        }

        public ChooseSoundResult ChooseSound()
        {
            var choiceView = new ChooseSoundView(soundsLibrary, this);
            var result = choiceView.ShowDialog();
            return new ChooseSoundResult(result == DialogResult.OK ? ActionResult.Ok : ActionResult.Cancel,
                choiceView.ChosenSound);
        }

        public void ShowSoundManager()
        {
            view.ShowAndBringToFront();
        }

        public void StopAllSounds()
        {
            engine.StopAllSounds();
        }

        #region IFeature

        void IFeature.Show()
        {
            ShowSoundManager();
        }

        void IFeature.Hide()
        {
            view.Hide();
        }

        string IFeature.Name
        {
            get { return "Sounds Manager"; }
        }

        Image IFeature.Icon
        {
            get { return WaResources.SoundManagerIcon; }
        }

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        #endregion
    }
}
