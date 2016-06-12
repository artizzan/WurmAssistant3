using System;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Parts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Services
{
    [KernelHint(BindingHint.Singleton), PersistentObject("SoundEngine")]
    public sealed class SoundManager : PersistentObjectBase, ISoundManager, IInitializable
    {
        readonly ISoundsLibrary soundsLibrary;
        private readonly ILogger logger;
        readonly ISoundEngine engine;

        [JsonProperty]
        float globalVolume;

        [JsonProperty]
        bool globalMute;

        readonly SoundManagerForm form;

        public SoundManager([NotNull] ISoundsLibrary soundsLibrary, ISoundEngine soundEngine, ILogger logger, IProcessStarter processStarter)
        {
            if (soundsLibrary == null) throw new ArgumentNullException("soundsLibrary");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (logger == null) throw new ArgumentNullException("logger");
            if (processStarter == null) throw new ArgumentNullException("processStarter");
            this.soundsLibrary = soundsLibrary;
            this.logger = logger;

            engine = soundEngine;
            globalVolume = 0.5f;

            form = new SoundManagerForm(this, soundsLibrary, processStarter);
        }

        public void Initialize()
        {
            engine.SoundVolume = globalVolume;
            form.SetSoundSlider(globalVolume);
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

        public ISoundResource GetFirstSoundByName(string name)
        {
            var sound = soundsLibrary.TryGetFirstSoundMatchingName(name);
            return sound ?? new SoundResourceNullObject();
        }

        public Guid AddSound(Sound sound)
        {
            return soundsLibrary.AddSound(sound);
        }

        public Guid AddSoundAsNewId(Sound sound)
        {
            return soundsLibrary.AddSoundSkipId(sound);
        }

        public bool GlobalMute
        {
            get { return globalMute; }
            set { globalMute = value; FlagAsChanged(); }
        }

        public IPlayingSoundHandle PlayOneShot(ISoundResource soundResource)
        {
            return PlayOneShot(soundResource != null ? soundResource.Id : Guid.Empty);
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
            return ChooseSound(null);
        }

        public ChooseSoundResult ChooseSound(Form parentForm)
        {
            var choiceView = new ChooseSoundForm(soundsLibrary, this);
            DialogResult result;
            if (parentForm != null)
            {
                result = choiceView.ShowDialogCenteredOnForm(parentForm);
            }
            else
            {
                result = choiceView.ShowDialog();
            }
            return new ChooseSoundResult(result == DialogResult.OK ? ActionResult.Ok : ActionResult.Cancel,
                choiceView.ChosenSound);
        }

        public void ShowSoundManager()
        {
            form.ShowAndBringToFront();
        }

        public void StopAllSounds()
        {
            engine.StopAllSounds();
        }

        public void HideGui()
        {
            form.Hide();
        }
    }
}
