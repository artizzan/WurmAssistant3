using System.IO;

namespace AldurSoft.WurmApi.Modules.Wurm.Paths
{
    public class WurmPaths : IWurmPaths
    {
        private readonly IWurmInstallDirectory wurmInstallDirectory;

        private string configsDirPath;
        private string playersDirPath;

        public WurmPaths(IWurmInstallDirectory wurmInstallDirectory)
        {
            this.wurmInstallDirectory = wurmInstallDirectory;
        }

        public string ConfigsDirFullPath
        {
            get
            {
                return configsDirPath
                       ?? (configsDirPath =
                           Path.Combine(this.wurmInstallDirectory.FullPath, "configs"));
            }
        }

        public string CharactersDirFullPath
        {
            get
            {
                return playersDirPath
                       ?? (playersDirPath =
                           Path.Combine(this.wurmInstallDirectory.FullPath, "players"));
            }
        }
    }
}
