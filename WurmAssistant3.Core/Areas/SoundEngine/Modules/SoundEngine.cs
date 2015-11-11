using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Views;
using AldursLab.WurmAssistant3.Core.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;
using WurmAssistantDataTransfer.Dtos;
using ISoundEngine = AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts.ISoundEngine;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Modules
{
    [PersistentObject("SoundEngine")]
    public sealed class SoundEngine : PersistentObjectBase, ISoundEngine, IInitializable, IFeature
    {
        readonly ISoundsLibrary soundsLibrary;
        private readonly ILogger logger;
        readonly IrrKlang.ISoundEngine engine;

        [JsonProperty]
        float globalVolume;

        [JsonProperty]
        bool globalMute;

        readonly SoundManagerView view;

        public SoundEngine([NotNull] ISoundsLibrary soundsLibrary, ILogger logger)
        {
            if (soundsLibrary == null) throw new ArgumentNullException("soundsLibrary");
            if (logger == null) throw new ArgumentNullException("logger");
            this.soundsLibrary = soundsLibrary;
            this.logger = logger;

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
            var choiceView = new ChooseSoundView(soundsLibrary, this);
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
            get { return Resources.SoundManagerIcon; }
        }

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        public void PopulateDto(WurmAssistantDto dto)
        {
        }

        public async Task ImportDataFromWa2Async(WurmAssistantDto dto)
        {
            SoundEngineWa2Importer importer = new SoundEngineWa2Importer(soundsLibrary, logger);
            await importer.ImportFromDtoAsync(dto);
        }

        public int DataImportOrder { get { return -1; } }

        #endregion
    }
}
