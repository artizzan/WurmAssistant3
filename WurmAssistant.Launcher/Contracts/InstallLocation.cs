using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using AldursLab.WurmAssistant.Launcher.Dto;
using AldursLab.WurmAssistant.Shared;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    public class InstallLocation : IInstallLocation
    {
        readonly string installDirPath;
        readonly ControllerConfig config;
        readonly IProcessRunner processRunner;
        readonly IGui gui;

        public InstallLocation([NotNull] ControllerConfig config, [NotNull] IProcessRunner processRunner,
            [NotNull] IGui gui)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (processRunner == null) throw new ArgumentNullException("processRunner");
            if (gui == null) throw new ArgumentNullException("gui");
            this.config = config;
            this.processRunner = processRunner;
            this.gui = gui;

            string path = Path.Combine(config.LauncherBinDirFullPath, "bin");
            path = Path.Combine(path, config.WurmUnlimitedMode ? "wu" : "wo");
            path = Path.Combine(path, config.BuildCode);
            installDirPath = path;

            if (!Path.IsPathRooted(installDirPath))
            {
                throw new InvalidOperationException("rootPath must be absolute");
            }

            if (!Directory.Exists(installDirPath)) Directory.CreateDirectory(installDirPath);
        }

        public string BuildCode { get { return config.BuildCode; } }

        public bool AnyInstalled
        {
            get
            {
                return GetInstalledVersions().Any();
            }
        }

        public string InstallLocationPath { get { return installDirPath; }}

        public void RunWurmAssistant([NotNull] string buildNumber)
        {
            if (buildNumber == null) throw new ArgumentNullException("buildNumber");

            var exepath = Path.Combine(installDirPath, buildNumber, "AldursLab.WurmAssistant3.exe");
            var args = string.Empty;
            if (config.WurmUnlimitedMode) args += " -WurmUnlimited";
            if (config.UseRelativeDataDirPath) args += " -RelativeDataDir";
            processRunner.Start(exepath, args);
        }

        [CanBeNull]
        public Wa3VersionInfo TryGetLatestInstalledVersion()
        {
            if (AnyInstalled)
            {
                return GetInstalledVersions().OrderByDescending(info => info.BuildNumber).First();
            }

            return null;
        }

        public IEnumerable<Wa3VersionInfo> GetInstalledVersions()
        {
            return
                new DirectoryInfo(installDirPath)
                    .EnumerateDirectories()
                    .Select(
                        info =>
                        {
                            try
                            {
                                var version = Wa3VersionInfo.CreateFromVersionDat(
                                    File.ReadAllText(
                                        Path.Combine(info.FullName, "version.dat").Trim()));
                                if (version.BuildNumber != info.Name)
                                {
                                    gui.AddUserMessage("Version does not match directory name", Color.Orange);
                                    return null;
                                }
                                return version;
                            }
                            catch (Exception exception)
                            {
                                gui.AddUserMessage("Version parsing failed, error: " + exception.Message, Color.Orange);
                                return null;
                            }
                        })
                    .Where(info => info != null);
        } 
    }
}