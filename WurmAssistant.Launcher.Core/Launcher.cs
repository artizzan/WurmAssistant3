using System;
using System.IO;
using System.Linq;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Synchronization;
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

        readonly DirectoryInfo errorMessagesOutputDir;

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

            errorMessagesOutputDir = new DirectoryInfo(Path.Combine(launcherDataDirPath, "ErrorLog"));
            if (!errorMessagesOutputDir.Exists)
            {
                errorMessagesOutputDir.Create();
                CleanupOldErrors();
            }

            var dataDirPath = Path.Combine(launcherDataDirPath, PersistentDataDirName);
        }

        public void WriteErrorFile(string message)
        {
            var newFile =
                new FileInfo(Path.Combine(errorMessagesOutputDir.FullName,
                    DateTimeOffset.Now.FormatForFileNameUniversal()));
            File.WriteAllText(newFile.FullName, message);
        }

        void CleanupOldErrors()
        {
            var allFiles = errorMessagesOutputDir.GetFiles().Select(info => new
            {
                FileInfo = info,
                Stamp = ParseFromFileName(info)
            });
            var oldTreshhold = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1));
            var oldFiles = allFiles.Where(arg => arg.Stamp < oldTreshhold).ToArray();
            foreach (var oldFile in oldFiles)
            {
                try
                {
                    oldFile.FileInfo.Delete();
                }
                catch (Exception)
                {
                    // may be locked, will be deleted next time
                }
            }
        }

        DateTimeOffset ParseFromFileName(FileInfo info)
        {
            try
            {
                return DateTimeExt.ParseFromFileNameUniversal(info);
            }
            catch (Exception exception)
            {
                WriteErrorFile(string.Format("Could not parse date stamp from file name {0}, will be deleted. Error: {1}",
                    info.FullName,
                    exception.ToString()));
                return DateTimeOffset.MinValue;
            }
        }

        public void EnterLock()
        {
            if (!entered)
            {
                fileLock = FileLock.EnterWithCreate(Path.Combine(launcherDataDirPath, LockFileName));
                entered = true;
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
}
