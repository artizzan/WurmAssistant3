using System;
using System.Collections.Generic;

namespace AldursLab.WurmAssistant3.Areas.SoundManager
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
    }
}
