using System;
using System.Drawing;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Singletons
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
    }
}