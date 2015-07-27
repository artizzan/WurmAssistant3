using System.IO;

namespace AldurSoft.WurmApi.Modules.Wurm.Paths
{
    public class WurmPaths : IWurmPaths
    {
        private readonly string configsDirPath;
        private readonly string playersDirPath;

        public WurmPaths(IWurmInstallDirectory wurmInstallDirectory)
        {
            configsDirPath = Path.Combine(wurmInstallDirectory.FullPath, "configs");
            playersDirPath = Path.Combine(wurmInstallDirectory.FullPath, "players");
        }

        public string ConfigsDirFullPath
        {
            get { return configsDirPath; }
        }

        public string CharactersDirFullPath
        {
            get { return playersDirPath; }
        }
    }
}
