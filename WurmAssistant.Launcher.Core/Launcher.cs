using System;
using System.IO;
using AldursLab.Essentials.Synchronization;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.FlatFiles;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public class Launcher
    {
        readonly string launcherDataDirPath;
        const string LockFileName = "launcher.lock";
        const string PersistentDataDirName = "Data";

        FileLock fileLock;

        readonly JsonSerializer serializer = new JsonSerializer();

        readonly PersistentCollectionsLibrary library;
        readonly LauncherData launcherData;

        volatile bool entered;

        public Launcher(string launcherDataDirPath)
        {
            if (launcherDataDirPath == null) throw new ArgumentNullException("launcherDataDirPath");
            this.launcherDataDirPath = launcherDataDirPath;

            this.launcherDataDirPath = launcherDataDirPath;

            if (!Path.IsPathRooted(launcherDataDirPath))
            {
                throw new InvalidOperationException("rootPath must be absolute");
            }

            if (!Directory.Exists(launcherDataDirPath))
            {
                Directory.CreateDirectory(launcherDataDirPath);
            }

            var dataDirPath = Path.Combine(launcherDataDirPath, PersistentDataDirName);
            library = new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(dataDirPath));
            var entity = library.DefaultCollection.GetObject<LauncherDataEntity>("LauncherDataEntity");

            launcherData = new LauncherData(entity);
        }

        public void EnterLock()
        {
            if (!entered)
            {
                fileLock = FileLock.EnterWithCreate(Path.Combine(launcherDataDirPath, LockFileName));
                entered = true;
            }
        }

        public LauncherData GetPersistentData()
        {
            return launcherData;
        }

        public void SavePersistentData()
        {
            library.SaveAll();
        }

        public void ReleaseLock()
        {
            if (entered)
            {
                try
                {
                    fileLock.Dispose();
                }
                finally
                {
                    entered = false;
                }
            }
        }
    }

    public class LauncherDataEntity : Entity
    {
        public LauncherDataEntity()
        {
            WurmAssistantInstalledVersion = new Version(0, 0, 0, 0);
        }

        [NotNull]
        public Version WurmAssistantInstalledVersion { get; set; }
    }

    public class LauncherData : PersistentEntityBase<LauncherDataEntity>
    {
        public LauncherData(IPersistent<LauncherDataEntity> persistent) : base(persistent)
        {
        }

        [NotNull]
        public Version WurmAssistantInstalledVersion
        {
            get { return Entity.WurmAssistantInstalledVersion; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                Entity.WurmAssistantInstalledVersion = value;
            }
        }
    }
}
