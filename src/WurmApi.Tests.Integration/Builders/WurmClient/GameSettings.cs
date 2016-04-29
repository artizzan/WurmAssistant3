using System.IO;
using System.Text.RegularExpressions;

namespace AldursLab.WurmApi.Tests.Integration.Builders.WurmClient
{
    class GameSettings
    {
        public FileInfo GameSettingsTxt { get; private set; }

        public GameSettings(FileInfo gameSettingsTxt)
        {
            GameSettingsTxt = gameSettingsTxt;
            File.WriteAllText(GameSettingsTxt.FullName, Defaults.gamesettings);
        }

        public void ChangeValue(string setting, string newValue)
        {
            setting = Regex.Escape(setting);
            newValue = newValue.Replace("$", "$$");
            var source = File.ReadAllText(GameSettingsTxt.FullName);
            var output = Regex.Replace(source,
                string.Format(@"^({0}\=)(.+)$", setting),
                "$1" + newValue,
                RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            File.WriteAllText(GameSettingsTxt.FullName, output);
        }
    }
}