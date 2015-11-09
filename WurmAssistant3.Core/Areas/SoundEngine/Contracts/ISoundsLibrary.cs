using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts
{
    public interface ISoundsLibrary
    {
        event EventHandler<EventArgs> SoundsChanged;
        ISoundResource Import(string fileFullPath);
        void Rename(ISoundResource resource, string newName);
        void Remove(ISoundResource resource);
        void AdjustVolume(ISoundResource resource, float newValue);
        ISoundResource TryGetSound(Guid id);
        ICollection<ISoundResource> GetAllSounds();
        ISoundResource TryGetFirstSoundMatchingName(string name);

        /// <exception cref="ArgumentException">Sound with this Id already exists.</exception>
        Guid AddSound(Sound sound);

        Guid AddSoundSkipId(Sound sound);
    }
}
