using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Synchronization;

namespace AldursLab.WurmAssistant.LauncherCore
{
    public class LaunchController
    {
        readonly IGuiHost host;
        readonly IGui gui;

        readonly ControllerConfig config;

        public LaunchController(IGuiHost host, ControllerConfig config)
        {
            if (host == null) throw new ArgumentNullException("host");
            this.host = host;

            if (config == null) throw new ArgumentNullException("config");
            this.config = config;

            SevenZipManager.EnsurePathSet();

            var updaterGui = new UpdaterGui(host);
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
                gui.SetState(LauncherState.Error);
                gui.AddUserMessage("Wurm Assistant Launcher has encountered an error:\r\n" + exception.ToString());
                gui.ShowGui();
            }
        }

        async Task Run()
        {
            LauncherSync launcherSync = null;
            IInstallLocation installLocation = null;
            try
            {
                launcherSync = new LauncherSync(Path.Combine(config.RootDirFullPath, "Launcher"));
                installLocation = new InstallLocation(Path.Combine(config.RootDirFullPath, "Bin"),
                    config.WurmAssistantExeFileName,
                    new ProcessRunner());

                launcherSync.EnterLock();

                LauncherData data = launcherSync.GetLauncherPersistentState();

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
                        stagingLocation.ExtractIntoExtractionDir(latestStagedPackage);
                        installLocation.ClearLocation();
                        stagingLocation.MoveExtractionDir(installLocation.InstallLocationPath);
                        stagingLocation.ClearStagingArea();
                        data.WurmAssistantInstalledVersion = latestStagedPackage.Version;
                        launcherSync.SetLauncherPersistentState(data);
                    }
                    TryRunWurmAssistant(installLocation);

                    var remoteVersion = await wurmAssistantService.GetLatestVersionAsync(gui);
                    if (data.WurmAssistantInstalledVersion < remoteVersion)
                    {
                        await wurmAssistantService.GetPackageAsync(gui, remoteVersion);
                        // done - next run will install this staged version
                    }
                }
                else
                {
                    gui.ShowGui();
                    gui.AddUserMessage("Installing latest version of Wurm Assistant");
                    var latestVersion = await wurmAssistantService.GetLatestVersionAsync(gui);
                    var latestPackage = await wurmAssistantService.GetPackageAsync(gui, latestVersion);
                    gui.AddUserMessage("Extracting Wurm Assistant version " + latestVersion);
                    latestPackage.ExtractIntoDirectory(installLocation.InstallLocationPath);
                    data.WurmAssistantInstalledVersion = latestVersion;
                    launcherSync.SetLauncherPersistentState(data);
                    gui.AddUserMessage("Installation complete.");
                    gui.AddUserMessage("Starting Wurm Assistant...");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    installLocation.RunWurmAssistant();
                    gui.HideGui();
                    stagingLocation.ClearStagingArea();
                }

                launcherSync.SetLauncherPersistentState(data);

                gui.HideGui();
            }
            catch (LockFailedException)
            {
                if (installLocation != null)
                {
                    if (installLocation.Installed)
                    {
                        TryRunWurmAssistant(installLocation);
                    }
                }
            }
            finally
            {
                if (launcherSync != null)
                {
                    launcherSync.ReleaseLock();
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
