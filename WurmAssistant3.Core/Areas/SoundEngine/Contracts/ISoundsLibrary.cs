using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts
{
    public interface ISoundsLibrary
    {
        event EventHandler<EventArgs> SoundsChanged;
        void Import(string fileFullPath);
        void Rename(ISoundResource resource, string newName);
        void Remove(ISoundResource resource);
        void AdjustVolume(ISoundResource resource, float newValue);
        ISoundResource TryGetSound(Guid id);
        ICollection<ISoundResource> GetAllSounds();
    }
}
