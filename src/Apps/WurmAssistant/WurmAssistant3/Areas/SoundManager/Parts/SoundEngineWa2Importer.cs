using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Parts;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Parts
{
    public class SoundEngineWa2Importer
    {
        private readonly ISoundsLibrary soundsLibrary;
        private readonly ILogger logger;

        public SoundEngineWa2Importer(ISoundsLibrary soundsLibrary, ILogger logger)
        {
            if (soundsLibrary == null) throw new ArgumentNullException("soundsLibrary");
            if (logger == null) throw new ArgumentNullException("logger");
            this.soundsLibrary = soundsLibrary;
            this.logger = logger;
        }

        public async Task ImportFromDtoAsync(WurmAssistantDto dto)
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
            mergeAssistantView.Show();
            await mergeAssistantView.Completed;
        }
    }
}