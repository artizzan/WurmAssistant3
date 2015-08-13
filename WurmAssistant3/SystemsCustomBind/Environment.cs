using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AldursLab.WurmAssistant3.Modules;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.SystemsCustomBind
{
    public class Environment : IEnvironment
    {
        private readonly static string ReleaseType = Settings.Default.ReleaseType;
        private readonly string codeBaseDirFullPath = Assembly.GetExecutingAssembly().GetCodeBasePath();
        private static readonly string[] AllowedReleaseTypes = new[] { "Beta", "Stable" };

        private readonly string rootDataDirFullPath =
            Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "Aldurcraft",
                "WurmAssistant3",
                ReleaseType);

        public Environment()
        {
            if (!AllowedReleaseTypes.Contains(ReleaseType))
            {
                throw new InvalidOperationException("ReleaseType is not allowedReleaseTypes: " + ReleaseType);
            }
        }

        public string FullPathToRootDataDirectory
        {
            get { return rootDataDirFullPath; }
        }

        public string FullPathToCurrentDataDirectory
        {
            get { return Path.Combine(FullPathToRootDataDirectory, "Current"); }
        }

        public string FullPathToBackupDataDirectory(string backupId)
        {
            return Path.Combine(FullPathToRootDataDirectory, "Backup", backupId);
        }

        public string FullPathToLegacyDataDirectory
        {
            get
            {
                return Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                    "Aldurcraft",
                    "WurmAssistant2");
            }
        }

        public string CodeBaseDirFullPath
        {
            get { return codeBaseDirFullPath; }
        }

        public string GetFullPathToModuleWorkingDirectory(ModuleId moduleId)
        {
            return Path.Combine(FullPathToRootDataDirectory, "Modules", moduleId.GetFilePathFriendlyString(), "WorkDir");
        }

        public string GetFullPathToModulePersistentManagerDirectory(ModuleId moduleId)
        {
            return Path.Combine(FullPathToRootDataDirectory, "Modules", moduleId.GetFilePathFriendlyString(), "Data");
        }
    }
}
