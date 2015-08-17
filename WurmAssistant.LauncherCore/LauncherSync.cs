using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Synchronization;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant.LauncherCore
{
    public class LauncherSync
    {
        readonly string launcherDataDirPath;
        const string LockFileName = "launcher.lock";
        const string DataFileName = "launcher.state";

        FileLock fileLock;

        readonly JsonSerializer serializer = new JsonSerializer();
        readonly string dataFilePath;

        volatile bool entered;

        public LauncherSync(string launcherDataDirPath)
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


            dataFilePath = Path.Combine(launcherDataDirPath, DataFileName);
        }

        public void EnterLock()
        {
            if (!entered)
            {
                fileLock = FileLock.EnterWithCreate(Path.Combine(launcherDataDirPath, LockFileName));
                entered = true;
            }
        }

        public LauncherData GetLauncherPersistentState()
        {
            if (File.Exists(dataFilePath))
            {
                var fileContent = File.ReadAllText(dataFilePath);
                var deserialized = serializer.Deserialize<LauncherData>(new JsonTextReader(new StringReader(fileContent)));
                return deserialized;
            }
            else return new LauncherData();
        }

        public void SetLauncherPersistentState(LauncherData launcherData)
        {
            using (var sw = new StreamWriter(dataFilePath))
            {
                serializer.Serialize(sw, launcherData);
            }
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

    public class LauncherData
    {
        Version wurmAssistantInstalledVersion;

        public Version WurmAssistantInstalledVersion
        {
            get { return wurmAssistantInstalledVersion ?? new Version(0,0,0,0); }
            set { wurmAssistantInstalledVersion = value; }
        }
    }
}
