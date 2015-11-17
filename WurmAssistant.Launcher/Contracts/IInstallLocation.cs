using System.Collections.Generic;
using AldursLab.WurmAssistant.Shared;

namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    public interface IInstallLocation
    {
        bool AnyInstalled { get; }
        string InstallLocationPath { get; }
        void RunWurmAssistant(string buildNumber);
        Wa3VersionInfo TryGetLatestInstalledVersion();
        string BuildCode { get; }
        IEnumerable<Wa3VersionInfo> GetInstalledVersions();
    }
}