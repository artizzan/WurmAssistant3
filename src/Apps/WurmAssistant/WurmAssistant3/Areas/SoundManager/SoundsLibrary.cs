using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.SoundManager
{
    [KernelBind(BindingHint.Singleton), PersistentObject("SoundEngine_SoundsLibrary")]
    public class SoundsLibrary : PersistentObjectBase, ISoundsLibrary
    {
        public const string SoundbankDirName = "SoundBank";

        readonly string soundFilesPath;
        readonly ILogger logger;

        [JsonProperty] 
        readonly Dictionary<Guid, SoundResource> soundResources = new Dictionary<Guid, SoundResource>();

        public SoundsLibrary([NotNull] IWurmAssistantDataDirectory wurmAssistantDataDirectory, [NotNull] ILogger logger)
        {
            if (wurmAssistantDataDirectory == null) throw new ArgumentNullException(nameof(wurmAssistantDataDirectory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.soundFilesPath = Path.Combine(wurmAssistantDataDirectory.DirectoryPath, SoundbankDirName);
            this.logger = logger;

            var dirInfo = new DirectoryInfo(soundFilesPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        public event EventHandler<EventArgs> SoundsChanged;

        public ISoundResource Import(string fileFullPath)
        {
            return ImportInternal(fileFullPath);
        }

        private ISoundResource ImportInternal(string fileFullPath, Guid? id = null)
        {
            if (id != null && soundResources.ContainsKey(id.Value))
            {
                throw new ArgumentException(string.Format("Sound with id {0} already exists", id));
            }

            var resource = new SoundResource(id);
            var sourceFile = new FileInfo(fileFullPath);
            if (!sourceFile.Exists)
            {
                throw new InvalidOperationException("Source file does not exist: " + fileFullPath);
            }
            var newFileName = resource.Id + Path.GetExtension(fileFullPath);
            var newFullFileName = Path.Combine(soundFilesPath, newFileName);
            sourceFile.CopyTo(newFullFileName);
            resource.Name = Path.GetFileNameWithoutExtension(sourceFile.Name);
            resource.FileFullName = newFullFileName;
            soundResources.Add(resource.Id, resource);
            FlagAsChanged();
            OnSoundsChanged();
            return resource;
        }

        public void Rename(ISoundResource resource, string newName)
        {
            var res = TryGetResourceInternal(resource.Id);
            if (res != null)
            {
                res.Name = newName;
                FlagAsChanged();
                OnSoundsChanged();
            }
        }

        public void Remove(ISoundResource resource)
        {
            if (soundResources.Remove(resource.Id))
            {
                File.Delete(resource.FileFullName);
                FlagAsChanged();
                OnSoundsChanged();
            }
        }

        public void AdjustVolume(ISoundResource resource, float newValue)
        {
            var res = TryGetResourceInternal(resource.Id);
            if (res != null)
            {
                res.AdjustedVolume = newValue;
                FlagAsChanged();
                OnSoundsChanged();
            }
        }

        SoundResource TryGetResourceInternal(Guid id)
        {
            SoundResource res;
            if (soundResources.TryGetValue(id, out res))
            {
                return res;
            }
            else return null;
        }

        public ISoundResource TryGetSound(Guid id)
        {
            return TryGetResourceInternal(id);
        }

        public ICollection<ISoundResource> GetAllSounds()
        {
            return soundResources.Values.ToArray();
        }

        public ISoundResource TryGetFirstSoundMatchingName(string name)
        {
            return
                soundResources.Values.FirstOrDefault(
                    resource => resource.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        protected virtual void OnSoundsChanged()
        {
            SoundsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
