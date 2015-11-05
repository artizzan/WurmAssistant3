using System;
using System.IO;
using System.Linq;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant.Shared;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IInstallLocation
    {
        bool Installed { get; }
        string InstallLocationPath { get; }
        void ClearLocation();
        void RunWurmAssistant(string args = null);
        Wa3VersionInfo TryGetInstalledVersion();
        void EnterWa3Lock();
        void ReleaseWa3Lock();
    }

    public class InstallLocation : IInstallLocation
    {
        readonly string installDirPath;
        readonly string wurmAssistantExeFileName;
        readonly IProcessRunner processRunner;
        private readonly string dataDirLockFilePath;

        FileLock wa3AppLock;

        public InstallLocation(string installDirPath, string wurmAssistantExeFileName, IProcessRunner processRunner,
            [NotNull] string dataDirLockFilePath)
        {
            if (installDirPath == null)
                throw new ArgumentNullException("installDirPath");
            if (wurmAssistantExeFileName == null) throw new ArgumentNullException("wurmAssistantExeFileName");
            if (processRunner == null) throw new ArgumentNullException("processRunner");
            if (dataDirLockFilePath == null) throw new ArgumentNullException("dataDirLockFilePath");

            this.installDirPath = installDirPath;
            this.wurmAssistantExeFileName = wurmAssistantExeFileName;
            this.processRunner = processRunner;
            this.dataDirLockFilePath = dataDirLockFilePath;

            if (!Path.IsPathRooted(installDirPath))
            {
                throw new InvalidOperationException("rootPath must be absolute");
            }
        }

        public bool Installed
        {
            get
            {
                if (!Directory.Exists(installDirPath)) return false;
                var anyFiles = Directory.GetFiles(installDirPath).Any();
                var anyDirs = Directory.GetDirectories(installDirPath).Any();
                return anyFiles || anyDirs;
            }
        }

        public string InstallLocationPath { get { return installDirPath; }}

        public void ClearLocation()
        {
            if (Directory.Exists(installDirPath))
            {
                Directory.Delete(installDirPath, true);
            }
        }

        public void RunWurmAssistant(string args = null)
        {
            processRunner.Start(Path.Combine(installDirPath, wurmAssistantExeFileName), args);
        }

        [CanBeNull]
        public Wa3VersionInfo TryGetInstalledVersion()
        {
            if (Installed)
            {
                var versionFile = new FileInfo(Path.Combine(InstallLocationPath, "version.dat"));
                if (versionFile.Exists)
                {
                    return Wa3VersionInfo.CreateFromVersionDat(File.ReadAllText(versionFile.FullName).Trim());
                }
            }

            return null;
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
            if (wa3AppLock != null) wa3AppLock.Dispose();
        }
    }
}