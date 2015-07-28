using System.IO;

namespace AldurSoft.WurmApi.Tests.Builders.WurmClient
{
    class WurmPlayer
    {
        readonly DirectoryInfo playerDir;

        DirectoryInfo DumpsDir { get; set; }
        DirectoryInfo LogsDir { get; set; }
        DirectoryInfo ScreenshotsDir { get; set; }
        FileInfo ConfigTxt { get; set; }

        public WurmPlayer(DirectoryInfo playerDir, string name)
        {
            Name = name;
            this.playerDir = playerDir;
            DumpsDir = playerDir.CreateSubdirectory("dumps");
            LogsDir = playerDir.CreateSubdirectory("logs");
            ScreenshotsDir = playerDir.CreateSubdirectory("screenshots");
            ConfigTxt = new FileInfo(Path.Combine(playerDir.FullName, "config.txt"));
            File.WriteAllText(ConfigTxt.FullName, string.Empty);
            Logs = new WurmLogs(LogsDir);
        }

        public string Name { get; private set; }

        public WurmLogs Logs { get; private set; }

        public DirectoryInfo PlayerDir
        {
            get { return playerDir; }
        }

        public WurmPlayer SetConfigName(string name)
        {
            File.WriteAllText(ConfigTxt.FullName + "\r\n", name);
            return this;
        }
    }
}