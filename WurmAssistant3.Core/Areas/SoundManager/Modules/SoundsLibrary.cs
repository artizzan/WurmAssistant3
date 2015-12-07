using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules
{
    [PersistentObject("SoundEngine_SoundsLibrary")]
    public class SoundsLibrary : PersistentObjectBase, ISoundsLibrary
    {
        readonly string soundFilesPath;
        readonly ILogger logger;

        [JsonProperty] 
        readonly Dictionary<Guid, SoundResource> soundResources = new Dictionary<Guid, SoundResource>();

        private readonly string tempSoundsDirPath;

        public SoundsLibrary([NotNull] string soundFilesPath, [NotNull] ILogger logger)
        {
            if (soundFilesPath == null) throw new ArgumentNullException("soundFilesPath");
            if (logger == null) throw new ArgumentNullException("logger");
            this.soundFilesPath = soundFilesPath;
            this.logger = logger;

            tempSoundsDirPath = Path.Combine(soundFilesPath, "Temp");

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

        public Guid AddSoundSkipId(Sound sound)
        {
            return AddSoundInternal(sound, true);
        }

        public Guid AddSound(Sound sound)
        {
            return AddSoundInternal(sound, false);
        }

        private Guid AddSoundInternal(Sound sound, bool skipGlobalId)
        {
            if (!skipGlobalId && sound.Id != null && soundResources.ContainsKey(sound.Id.Value))
            {
                throw new ArgumentException(string.Format("sound with id {0} already exists", sound.Id));
            }

            try
            {
                PrepareTempDir();
                var soundFilePath = Path.Combine(tempSoundsDirPath, sound.FileNameWithExt);
                File.WriteAllBytes(soundFilePath, sound.FileData);
                var resource = ImportInternal(soundFilePath, skipGlobalId ? null : sound.Id);
                Rename(resource, sound.Name);
                return resource.Id;
            }
            finally
            {
                ClearTempDir();
            }
        }

        void PrepareTempDir()
        {
            ClearTempDir();
            var directory = new DirectoryInfo(tempSoundsDirPath);
            directory.Create();
        }

        void ClearTempDir()
        {
            var directory = new DirectoryInfo(tempSoundsDirPath);
            if (directory.Exists) directory.Delete(true);
        }

        protected virtual void OnSoundsChanged()
        {
            var handler = SoundsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
