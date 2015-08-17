using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AldursLab.WurmAssistant.LauncherCore
{
    public interface IInstallLocation
    {
        bool Installed { get; }
        string InstallLocationPath { get; }
        void ClearLocation();
        void RunWurmAssistant();
    }

    public class InstallLocation : IInstallLocation
    {
        readonly string installDirPath;
        readonly string wurmAssistantExeFileName;
        readonly IProcessRunner processRunner;

        public InstallLocation(string installDirPath, string wurmAssistantExeFileName, IProcessRunner processRunner)
        {
            if (installDirPath == null)
                throw new ArgumentNullException("installDirPath");
            if (wurmAssistantExeFileName == null) throw new ArgumentNullException("wurmAssistantExeFileName");
            if (processRunner == null) throw new ArgumentNullException("processRunner");

            this.installDirPath = installDirPath;
            this.wurmAssistantExeFileName = wurmAssistantExeFileName;
            this.processRunner = processRunner;

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

        public void RunWurmAssistant()
        {
            processRunner.Start(Path.Combine(installDirPath, wurmAssistantExeFileName));
        }
    }
}