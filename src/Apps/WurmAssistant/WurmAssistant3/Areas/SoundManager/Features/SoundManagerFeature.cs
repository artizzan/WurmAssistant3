using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Features
{
    public class SoundManagerFeature : IFeature
    {
        readonly ISoundManager soundManager;

        public SoundManagerFeature([NotNull] ISoundManager soundManager)
        {
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            this.soundManager = soundManager;
        }

        void IFeature.Show()
        {
            soundManager.ShowSoundManager();
        }

        void IFeature.Hide()
        {
            soundManager.HideGui();
        }

        string IFeature.Name => "Sounds Manager";

        Image IFeature.Icon => Resources.SoundManagerIcon;

        async Task IFeature.InitAsync()
        {
            await Task.FromResult(true);
        }

        public void PopulateDto(WurmAssistantDto dto)
        {
        }

        public async Task ImportDataFromWa2Async(WurmAssistantDto dto)
        {
            await soundManager.ImportDataFromWa2Async(dto);
        }

        public int DataImportOrder => -1;
    }
}