using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Views;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Views;
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

        public void AddSound(Sound sound)
        {
            soundsLibrary.AddSound(sound);
        }

        public void AddSoundAsNewId(Sound sound)
        {
            soundsLibrary.AddSoundSkipId(sound);
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

        public void ImportFromDto(WurmAssistantDto dto)
        {
            var items = dto.Sounds.Select(sound =>
            {
                ISoundResource existingSound = null;
                MergeResult defaultMergeResult = MergeResult.AddAsNew;
                if (sound.Id != null)
                {
                    existingSound = soundsLibrary.TryGetSound(sound.Id.Value);
                    defaultMergeResult = MergeResult.DoNothing;
                }
                if (existingSound == null)
                {
                    existingSound = soundsLibrary.TryGetFirstSoundMatchingName(sound.Name);
                    defaultMergeResult = MergeResult.DoNothing;
                }
                return new ImportItem<Sound, ISoundResource>()
                {
                    Source = sound,
                    Destination = existingSound,
                    SourceAspectConverter =
                        s => s != null
                            ? string.Format("Id: {0}, Name: {1}, FileName: {2}, Size: {3}",
                                s.Id,
                                s.Name,
                                s.FileNameWithExt,
                                (s.FileData.Length / 1024) + " kB")
                            : string.Empty,
                    DestinationAspectConverter =
                        s =>
                        {
                            if (s == null)
                                return string.Empty;

                            var fileInfo = new FileInfo(s.FileFullName);
                            var result =
                                string.Format("Id: {0}, Name: {1}, FileName: {2}, Size: {3}",
                                    s.Id,
                                    s.Name,
                                    fileInfo.Exists ? fileInfo.Name : "File missing!",
                                    fileInfo.Exists ? (fileInfo.Length / 1024) + " kB" : "File missing!");
                            return result;
                        },
                    MergeResult = defaultMergeResult,
                    ResolutionAction =
                        (result, soundSource, soundDestination) =>
                        {
                            if (result == MergeResult.AddAsNew)
                            {
                                soundsLibrary.AddSound(soundSource);
                            }
                        }
                };
            }).ToArray();
            var mergeAssistantView = new ImportMergeAssistantView(items, logger);
            mergeAssistantView.Text = "Choose sounds to import...";
            mergeAssistantView.StartPosition = FormStartPosition.CenterScreen;
            mergeAssistantView.ShowDialog();
        }

        public int DataImportOrder { get { return -1; } }

        #endregion
    }
}
