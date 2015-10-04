using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials.Eventing;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Modules
{
    [PersistentObject("SoundEngine_SoundsLibrary")]
    public class SoundsLibrary : PersistentObjectBase, ISoundsLibrary
    {
        readonly string soundFilesPath;
        readonly ILogger logger;

        [JsonProperty] 
        readonly Dictionary<Guid, SoundResource> soundResources = new Dictionary<Guid, SoundResource>();

        public SoundsLibrary([NotNull] string soundFilesPath, [NotNull] ILogger logger)
        {
            if (soundFilesPath == null) throw new ArgumentNullException("soundFilesPath");
            if (logger == null) throw new ArgumentNullException("logger");
            this.soundFilesPath = soundFilesPath;
            this.logger = logger;

            var dirInfo = new DirectoryInfo(soundFilesPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        public event EventHandler<EventArgs> SoundsChanged;

        public void Import(string fileFullPath)
        {
            var resource = new SoundResource();
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

        protected virtual void OnSoundsChanged()
        {
            var handler = SoundsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
