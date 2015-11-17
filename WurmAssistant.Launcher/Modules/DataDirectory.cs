using System;
using System.IO;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Dto;
using AldursLab.WurmAssistant.Shared;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class DataDirectory : IDataDirectory
    {
        private readonly string dataDirLockFilePath;
        FileLock wa3AppLock;

        public DataDirectory([NotNull] ControllerConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");

            string path;
            if (config.UseRelativeDataDirPath)
            {
                path = config.WurmUnlimitedMode
                    ? Path.Combine(config.LauncherBinDirFullPath, "data-wa-u")
                    : Path.Combine(config.LauncherBinDirFullPath, "data-wa-o");
            }
            else
            {
                path = config.WurmUnlimitedMode
                    ? AppPaths.WurmAssistantUnlimited.DataDir.FullPath
                    : AppPaths.WurmAssistant3.DataDir.FullPath;
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            dataDirLockFilePath = Path.Combine(path, AppPaths.LockFileRelativePath);
        }

        public void EnterWa3Lock()
        {
            if (wa3AppLock == null)
            {
                wa3AppLock = FileLock.EnterWithCreate(dataDirLockFilePath);
            }
        }

        public void ReleaseWa3Lock()
        {
            if (wa3AppLock != null)
                wa3AppLock.Dispose();
        }
    }
}