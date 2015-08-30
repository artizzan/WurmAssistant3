using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Synchronization;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public class LaunchController
    {
        readonly IGuiHost host;
        readonly IGui gui;

        readonly ControllerConfig config;
        readonly IDebug debug;

        public LaunchController(IGuiHost host, ControllerConfig config, [NotNull] IDebug debug)
        {
            if (host == null) throw new ArgumentNullException("host");
            this.host = host;

            if (config == null) throw new ArgumentNullException("config");
            if (debug == null) throw new ArgumentNullException("debug");
            this.config = config;
            this.debug = debug;

            var updaterGui = new UpdaterGui(host, debug);
            host.SetContent(updaterGui);

            gui = updaterGui;
        }

        public async void Execute()
        {
            try
            {
                await Run();
            }
            catch (Exception exception)
            {
                debug.Write("Error at Execute: " + exception.ToString());
                gui.SetState(LauncherState.Error);
                gui.AddUserMessage("Wurm Assistant Launcher has encountered an error:\r\n" + exception.ToString());
                gui.ShowGui();
                MessageBox.Show("Launcher has crashed: " + exception.ToString(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        async Task Run()
        {
            Launcher launcher = null;
            IInstallLocation installLocation = null;
            try
            {
                launcher = new Launcher(Path.Combine(config.RootDirFullPath, "Launcher"));
                installLocation = new InstallLocation(Path.Combine(config.RootDirFullPath, "Bin"),
                    config.WurmAssistantExeFileName,
                    new ProcessRunner());

                launcher.EnterLock();

                LauncherData launcherData = launcher.GetPersistentData();

                IStagingLocation stagingLocation =
                    new StagingLocation(Path.Combine(config.RootDirFullPath, "Staging"));
                IWurmAssistantService wurmAssistantService = new WurmAssistantService(
                    config.WebServiceRootUrl,
                    stagingLocation);

                stagingLocation.ClearExtractionDir();

                if (installLocation.Installed)
                {
                    if (stagingLocation.AnyPackageStaged)
                    {
                        var latestStagedPackage = stagingLocation.GetLatestStagedPackage();
                        gui.ShowGui();
                        gui.AddUserMessage("Updating to version " + latestStagedPackage.Version);
                        gui.SetProgressStatus("Updating...");
                        gui.SetProgressPercent(null);
                        stagingLocation.ExtractIntoExtractionDir(latestStagedPackage);
                        installLocation.ClearLocation();
                        stagingLocation.MoveExtractionDir(installLocation.InstallLocationPath);
                        stagingLocation.ClearStagingArea();
                        launcherData.WurmAssistantInstalledVersion = latestStagedPackage.Version;
                        launcher.SavePersistentData();
                        gui.AddUserMessage("Update complete");
                        gui.SetProgressStatus("Update complete");
                        await Task.Delay(1);
                        gui.HideGui();
                    }
                    TryRunWurmAssistant(installLocation);

                    await CheckAndDownloadLatestPackage(wurmAssistantService, launcher, launcherData);
                }
                else
                {
                    gui.ShowGui();
                    gui.AddUserMessage("Installing latest version of Wurm Assistant");
                    var latestVersion = await wurmAssistantService.GetLatestVersionAsync(gui);
                    var latestPackage = await wurmAssistantService.GetPackageAsync(gui, latestVersion);
                    gui.AddUserMessage("Extracting Wurm Assistant version " + latestVersion);
                    latestPackage.ExtractIntoDirectory(installLocation.InstallLocationPath);
                    launcherData.WurmAssistantInstalledVersion = latestVersion;
                    launcher.SavePersistentData();
                    gui.AddUserMessage("Installation complete.");
                    gui.AddUserMessage("Starting Wurm Assistant...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    installLocation.RunWurmAssistant();
                    gui.HideGui();
                    stagingLocation.ClearStagingArea();
                }

                launcher.SavePersistentData();

                gui.HideGui();
                host.Close();
            }
            catch (LockFailedException exception)
            {
                debug.Write("LockFailedException: " + exception.ToString());
                if (installLocation != null)
                {
                    if (installLocation.Installed)
                    {
                        TryRunWurmAssistant(installLocation);
                    }
                }
                host.Close();
            }
            finally
            {
                if (launcher != null)
                {
                    launcher.ReleaseLock();
                }
            }
        }

        async Task CheckAndDownloadLatestPackage(IWurmAssistantService wurmAssistantService, Launcher launcher,
            LauncherData launcherData)
        {
            Version remoteVersion = null;
            try
            {
                remoteVersion = await wurmAssistantService.GetLatestVersionAsync(gui);
            }
            catch (Exception exception)
            {
                launcher.WriteErrorFile(
                    string.Format("Launcher failed to check latest WA version. Error: {0}",
                        exception.ToString()));
            }

            if (remoteVersion != null && launcherData.WurmAssistantInstalledVersion < remoteVersion)
            {
                try
                {
                    await wurmAssistantService.GetPackageAsync(gui, remoteVersion);
                    // done - next run will install this staged version
                }
                catch (Exception exception)
                {
                    launcher.WriteErrorFile(
                        string.Format("Launcher failed to download latest WA version. Error: {0}",
                            exception.ToString()));
                }
            }
        }

        bool TryRunWurmAssistant(IInstallLocation installLocation)
        {
            try
            {
                installLocation.RunWurmAssistant();
                return true;
            }
            catch (Exception exception)
            {
                debug.Write("Error at TryRunWurmAssistant: " + exception.ToString());
                gui.ShowGui();
                gui.AddUserMessage("Error while attempting to start Wurm Assistant: " + exception.ToString());
                return false;
            }
        }
    }

    public class ControllerConfig
    {
        public string RootDirFullPath { get; set; }
        public string WebServiceRootUrl { get; set; }
        public string WurmAssistantExeFileName { get; set; }
    }
}
