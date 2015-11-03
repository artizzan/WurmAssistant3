using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant.Shared;
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
                gui.AddUserMessage("Wurm Assistant Launcher has encountered an error: " + exception.ToStringMessagesOnly(), Color.Red);
                gui.ShowGui();
            }
        }

        async Task Run()
        {
            bool wasError = false;

            Launcher launcher = null;
            IInstallLocation installLocation = null;
            try
            {
                launcher = new Launcher(Path.Combine(config.RootDirFullPath, "Launcher"));
                installLocation = new InstallLocation(Path.Combine(config.RootDirFullPath, "Bin"),
                    config.WurmAssistantExeFileName,
                    new ProcessRunner());

                launcher.EnterLock();

                IStagingLocation stagingLocation =
                    new StagingLocation(Path.Combine(config.RootDirFullPath, "Staging"));
                IWurmAssistantService wurmAssistantService = 
                    new WurmAssistantService(config, stagingLocation);

                stagingLocation.ClearExtractionDir();
                stagingLocation.ClearStagingArea();

                gui.ShowGui();

                if (installLocation.Installed)
                {
                    gui.EnableSkip();
                    gui.SkipAction = () =>
                    {
                        TryRunWurmAssistant(installLocation);
                    };
                }

                var localVersion = installLocation.TryGetInstalledVersion();
                gui.AddUserMessage("Local version is: " + (localVersion != null ? localVersion.ToString() : "None"));

                Wa3VersionInfo targetVersion = null;

                if (config.BuildNumber != null)
                {
                    targetVersion = new Wa3VersionInfo(config.BuildCode, config.BuildNumber);
                }
                else
                {
                    try
                    {
                        var latestBuildNumber = await wurmAssistantService.GetLatestVersionAsync(gui, config.BuildCode);
                        if (!string.IsNullOrEmpty(latestBuildNumber))
                        {
                            targetVersion = new Wa3VersionInfo(config.BuildCode, latestBuildNumber);
                        }
                        else
                        {
                            if (config.BuildCode.StartsWith("stable", StringComparison.InvariantCultureIgnoreCase)
                                && config.AttemptBetaIfNoStable)
                            {
                                gui.AddUserMessage(
                                    string.Format("No build found for build code: {0}. Trying latest beta build.",
                                        config.BuildCode),
                                    Color.Orange);
                                latestBuildNumber = await wurmAssistantService.GetLatestVersionAsync(gui, "beta");
                                if (!string.IsNullOrEmpty(latestBuildNumber))
                                {
                                    gui.AddUserMessage(
                                        string.Format("Beta build found. Continuing...",
                                            config.BuildCode),
                                        Color.Orange);
                                    targetVersion = new Wa3VersionInfo("beta", latestBuildNumber);
                                }
                                else
                                {
                                    wasError = true;
                                    gui.AddUserMessage(string.Format("No build found for build code: {0}", config.BuildCode), Color.Red);
                                }
                            }
                            else
                            {
                                wasError = true;
                                gui.AddUserMessage(string.Format("No build found for build code: {0}", config.BuildCode), Color.Red);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        wasError = true;
                        var template = "Launcher failed to check latest WA version of build {0}. Error: {1}";
                        gui.AddUserMessage(string.Format(template, config.BuildCode, exception.ToStringMessagesOnly()), Color.Red);
                        debug.Write(string.Format(template, config.BuildCode, exception.ToString()));
                    }
                }

                if (targetVersion == null)
                {
                    // skip
                }
                else if (localVersion == targetVersion)
                {
                    gui.AddUserMessage("Target version is already installed.");
                }
                else
                {
                    try
                    {
                        await wurmAssistantService.GetPackageAsync(gui, targetVersion.BuildCode, targetVersion.BuildNumber);
                    }
                    catch (Exception exception)
                    {
                        wasError = true;
                        var template = "Launcher failed to download WA3 version {0} of build {1}. Error: {2}";
                        gui.AddUserMessage(
                            string.Format(template, targetVersion, config.BuildCode, exception.ToStringMessagesOnly()),
                            Color.Red);
                        debug.Write(string.Format(template, targetVersion, config.BuildCode, exception.ToString()));
                    }
                }

                TryInstallFromStage(stagingLocation, installLocation);
                await Task.Delay(TimeSpan.FromSeconds(1));

                if (!ValidateExpectedVersionIsInstalled(installLocation,
                    targetVersion != null ? targetVersion.BuildCode : config.BuildCode,
                    targetVersion != null ? targetVersion.BuildNumber : config.BuildNumber))
                {
                    wasError = true;
                }

                if (!TryRunWurmAssistant(installLocation))
                {
                    wasError = true;
                }

                if (!wasError)
                {
                    gui.HideGui();
                    host.Close();
                }
                else
                {
                    gui.SetState(LauncherState.Error);
                }
            }
            catch (LockFailedException exception)
            {
                debug.Write("LockFailedException: " + exception.ToString());
                gui.AddUserMessage("LockFailedException: " + exception.ToStringMessagesOnly(), Color.Red);
                if (installLocation != null)
                {
                    if (installLocation.Installed)
                    {
                        bool close = ValidateExpectedVersionIsInstalled(installLocation, config.BuildCode, config.BuildNumber);
                        TryRunWurmAssistant(installLocation);
                        if (close)
                        {
                            host.Close();
                        }
                        else
                        {
                            gui.SetState(LauncherState.Error);
                        }
                    }
                }
            }
            finally
            {
                if (launcher != null)
                {
                    launcher.ReleaseLock();
                }
                gui.DisableSkip();
            }
        }

        void TryInstallFromStage(IStagingLocation stagingLocation, IInstallLocation installLocation)
        {
            if (stagingLocation.AnyPackageStaged)
            {
                try
                {
                    installLocation.EnterWa3Lock();
                    var latestStagedPackage = stagingLocation.GetStagedPackage();
                    gui.AddUserMessage("Updating...");
                    gui.SetProgressStatus("Updating...");
                    gui.SetProgressPercent(null);
                    stagingLocation.ExtractIntoExtractionDir(latestStagedPackage);
                    installLocation.ClearLocation();
                    stagingLocation.MoveExtractionDir(installLocation.InstallLocationPath);
                    stagingLocation.ClearStagingArea();
                    gui.AddUserMessage("Updated to version: " + installLocation.TryGetInstalledVersion().ToString());
                    gui.AddUserMessage("Update complete");
                    gui.SetProgressStatus("Update complete");
                }
                catch (LockFailedException exception)
                {
                    gui.AddUserMessage(
                        "Could not enter WA3 lock, if the app is running, it must be closed before update. Exact error: "
                        + exception.ToStringMessagesOnly(),
                        Color.Red);
                    debug.Write("App lock failed: " + exception.ToString());
                }
                catch (Exception exception)
                {
                    gui.AddUserMessage(
                        "General launcher error: "
                        + exception.ToStringMessagesOnly(),
                        Color.Red);
                    debug.Write("General launcher error: " + exception.ToString());
                }
                finally
                {
                    installLocation.ReleaseWa3Lock();
                }
            }
        }

        bool ValidateExpectedVersionIsInstalled([NotNull] IInstallLocation installLocation, [NotNull] string targetBuildCode,
            [CanBeNull] string targetBuildNumber)
        {
            if (targetBuildCode == null) throw new ArgumentNullException("targetBuildCode");

            if (installLocation.Installed)
            {
                var installedVersion = installLocation.TryGetInstalledVersion();
                if (installedVersion == null)
                {
                    gui.AddUserMessage("Installed WA version could not be read.");
                    gui.AddUserMessage("Attempting to launch installed version...");
                    return false;
                }

                if (installedVersion.BuildCode != targetBuildCode ||
                    (!string.IsNullOrEmpty(targetBuildNumber) && targetBuildNumber != installedVersion.BuildNumber))
                {
                    gui.AddUserMessage(
                        string.Format(
                            "Warning! Installed version does not meet expected target version.\nInstalled: {0}\nTarget: {1}",
                            installedVersion,
                            string.Format("BuildCode: {0}, BuildNumber: {1}",
                                targetBuildCode,
                                targetBuildNumber ?? "Any")), Color.Orange);
                    gui.AddUserMessage("Attempting to launch installed version...");
                    return false;
                }
            }
            return true;
        }

        bool TryRunWurmAssistant([NotNull] IInstallLocation installLocation)
        {
            if (installLocation == null) throw new ArgumentNullException("installLocation");
            try
            {
                installLocation.RunWurmAssistant(config.WurmUnlimitedMode ? "-WurmUnlimited" : null);
                return true;
            }
            catch (Exception exception)
            {
                debug.Write("Error at TryRunWurmAssistant: " + exception.ToString());
                gui.ShowGui();
                gui.AddUserMessage("Error while attempting to start Wurm Assistant: " + exception.ToStringMessagesOnly());
                return false;
            }
        }
    }

    public class ControllerConfig
    {
        public string RootDirFullPath { get; set; }
        public string WebServiceRootUrl { get; set; }
        public string WurmAssistantExeFileName { get; set; }
        public string BuildCode { get; set; }
        [CanBeNull] public string BuildNumber { get; set; }
        public bool WurmUnlimitedMode { get; set; }
        public bool AttemptBetaIfNoStable { get; set; }
    }
}
