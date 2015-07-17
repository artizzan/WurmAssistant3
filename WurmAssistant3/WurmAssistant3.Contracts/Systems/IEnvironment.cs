using AldurSoft.WurmAssistant3.Modules;

namespace AldurSoft.WurmAssistant3.Systems
{
    /// <summary>
    /// Provides information about execution environment.
    /// </summary>
    public interface IEnvironment
    {
        string FullPathToCurrentDataDirectory { get; }

        string FullPathToRootDataDirectory { get; }

        string FullPathToBackupDataDirectory(string backupId);

        /// <summary>
        /// Directory containing application data of WurmAssistant2
        /// </summary>
        string FullPathToLegacyDataDirectory { get; }

        /// <summary>
        /// Directory of the runtime assemblies.
        /// </summary>
        string CodeBaseDirFullPath { get; }

        /// <summary>
        /// Gets directory dedicated solely to holding custom persistent module data.
        /// This is not the directory used by PersistentManager.
        /// </summary>
        string GetFullPathToModuleWorkingDirectory(ModuleId moduleId);

        /// <summary>
        /// Gets the directory dedicated for persisted data od its PersistentManager.
        /// </summary>
        string GetFullPathToModulePersistentManagerDirectory(ModuleId moduleId);
    }
}
