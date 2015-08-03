using System.IO;

namespace AldurSoft.WurmApi.Tests.Builders.WurmClient
{
    class GameSettings
    {
        FileInfo GameSettingsTxt { get; set; }
        public FileInfo FileInfo { get { return GameSettingsTxt; } }

        public GameSettings(FileInfo gameSettingsTxt)
        {
            GameSettingsTxt = gameSettingsTxt;
            File.WriteAllText(GameSettingsTxt.FullName, Defaults.gamesettings);
        }
    }
}