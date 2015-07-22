using System.IO;
using System.Linq;
using AldurSoft.WurmApi.Utility;
using AldurSoft.WurmApi.Wurm.Paths;

namespace AldurSoft.WurmApi.Wurm.Configs.WurmConfigDirectoriesModule
{
    /// <summary>
    /// Manages directory information about wurm configs
    /// </summary>
    public class WurmConfigDirectories : WurmSubdirsMonitor, IWurmConfigDirectories
    {
        public WurmConfigDirectories(
            IWurmPaths wurmPaths) : base(wurmPaths.ConfigsDirFullPath)
        {
        }

        public string GetGameSettingsFileFullPathForConfigName(string directoryName)
        {
            var dirPath = TryGetFullPathForDirName(directoryName);
            if (dirPath == null)
            {
                throw new WurmApiException(
                    string.Format(
                        "Directory full path not found for name: {0} ; Dir monitor for: {1}",
                        directoryName,
                        this.DirectoryFullPath));
            }
            var configDirectoryInfo = new DirectoryInfo(dirPath);
            var file = configDirectoryInfo.GetFiles("gamesettings.txt").FirstOrDefault();
            if (file == null)
            {
                throw new WurmApiException(
                    string.Format(
                        "gamesettings.txt full path not found for name: {0} ; Dir monitor for: {1}",
                        directoryName,
                        this.DirectoryFullPath));
            }
            return file.FullName;
        }
    }
}
