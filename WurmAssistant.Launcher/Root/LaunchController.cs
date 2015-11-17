using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Dto;
using AldursLab.WurmAssistant.Launcher.Modules;
using AldursLab.WurmAssistant.Launcher.Views;
using AldursLab.WurmAssistant.Shared;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Root
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

            Modules.Launcher launcher = null;
            IInstallLocation installLocation = null;
            try
            {
                launcher = new Modules.Launcher(config);
                installLocation = new InstallLocation(config, new ProcessRunner(), gui);

                launcher.EnterLock();

                IStagingLocation stagingLocation =
                    new StagingLocation(config);
                IUpdateService updateService = 
                    new UpdateService(config, stagingLocation);

                stagingLocation.ClearExtractionDir();
                stagingLocation.ClearStagingArea();

                gui.ShowGui();

                Wa3VersionInfo localVersion;
                Wa3VersionInfo targetVersion = null;

                if (config.BuildNumber != null)
                {
                    targetVersion = new Wa3VersionInfo(config.BuildCode, config.BuildNumber);
                    localVersion =
                        installLocation.GetInstalledVersions()
                                       .FirstOrDefault(info => info.BuildNumber == targetVersion.BuildNumber);
                    if (localVersion != null)
                    {
                        gui.EnableSkip();
                        gui.SkipAction = () =>
                        {
                            TryRunWurmAssistant(installLocation, localVersion.BuildNumber);
                        };
                    }
                }
                else
                {
                    localVersion = installLocation.TryGetLatestInstalledVersion();
                    if (installLocation.AnyInstalled)
                    {
                        gui.EnableSkip();
                        gui.SkipAction = () =>
                        {
                            TryRunWurmAssistant(installLocation, localVersion.BuildNumber);
                        };
                    }
                    gui.AddUserMessage("Local version is: " + (localVersion != null ? localVersion.ToString() : "None"));
                    try
                    {
                        var latestBuildNumber = await updateService.GetLatestVersionAsync(gui, config.BuildCode);
                        if (!string.IsNullOrEmpty(latestBuildNumber))
                        {
                            targetVersion = new Wa3VersionInfo(config.BuildCode, latestBuildNumber);
                        }
                        else
                        {
                            wasError = true;
                            gui.AddUserMessage(string.Format("No build found for build code: {0}", config.BuildCode), Color.Red);
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
                        gui.AddUserMessage("Downloading...");
                        gui.SetProgressStatus("Downloading...");

                        var package =
                            await
                                updateService.GetPackageAsync(gui,
                                    targetVersion.BuildCode,
                                    targetVersion.BuildNumber);

                        gui.AddUserMessage("Installing...");
                        gui.SetProgressStatus("Installing...");
                        gui.SetProgressPercent(null);

                        stagingLocation.ExtractIntoExtractionDir(package);
                        stagingLocation.ClearStagingArea();

                        stagingLocation.MoveExtractionDir(Path.Combine(installLocation.InstallLocationPath,
                            targetVersion.BuildNumber));

                        gui.AddUserMessage("installed version: " + installLocation.TryGetLatestInstalledVersion().ToString());
                        gui.AddUserMessage("Update complete");
                        gui.SetProgressStatus("Update complete");
                    }
                    catch (Exception exception)
                    {
                        wasError = true;
                        var template = "Launcher failed to download or install WA3 version {0} of build {1}. Error: {2}";
                        gui.AddUserMessage(
                            string.Format(template, targetVersion, config.BuildCode, exception.ToStringMessagesOnly()),
                            Color.Red);
                        debug.Write(string.Format(template, targetVersion, config.BuildCode, exception.ToString()));
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(1));

                if (targetVersion != null)
                {
                    if (!TryRunWurmAssistant(installLocation, targetVersion.BuildNumber))
                    {
                        gui.AddUserMessage("Could not start WA for version: " + targetVersion);
                        wasError = true;
                    }
                }
                else if (localVersion != null)
                {
                    if (!TryRunWurmAssistant(installLocation, localVersion.BuildNumber))
                    {
                        gui.AddUserMessage("Could not start WA for version: " + localVersion);
                        wasError = true;
                    }
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
                gui.SetState(LauncherState.Error);
            }
            finally
            {
                if (launcher != null)
                {
                    launcher.ReleaseLock();
                }
            }
        }

        bool TryRunWurmAssistant([NotNull] IInstallLocation installLocation, string targetBuildNumber)
        {
            if (installLocation == null) throw new ArgumentNullException("installLocation");
            try
            {
                installLocation.RunWurmAssistant(targetBuildNumber);
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
}
