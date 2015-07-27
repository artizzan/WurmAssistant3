using System.IO;
using System.Linq;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Wurm.ConfigDirectories
{
    /// <summary>
    /// Manages directory information about wurm configs
    /// </summary>
    public class WurmConfigDirectories : WurmSubdirsMonitor, IWurmConfigDirectories
    {
        public WurmConfigDirectories(
            IWurmPaths wurmPaths, ILogger logger) : base(wurmPaths.ConfigsDirFullPath, logger)
        {
        }

        public string GetGameSettingsFileFullPathForConfigName(string directoryName)
        {
            var dirPath = GetFullPathForDirName(directoryName);
            if (dirPath == null)
            {
                throw new DataNotFoundException(
                    string.Format(
                        "Directory full path not found for name: {0} ; Dir monitor for: {1}",
                        directoryName,
                        this.DirectoryFullPath));
            }
            var configDirectoryInfo = new DirectoryInfo(dirPath);
            var file = configDirectoryInfo.GetFiles("gamesettings.txt").FirstOrDefault();
            if (file == null)
            {
                throw new DataNotFoundException(
                    string.Format(
                        "gamesettings.txt full path not found for name: {0} ; Dir monitor for: {1}",
                        directoryName,
                        this.DirectoryFullPath));
            }
            return file.FullName;
        }
    }
}
