using System;
using System.IO;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Paths
{
    class WurmPaths : IWurmPaths
    {
        readonly IWurmApiConfig wurmApiConfig;

        private readonly string playersDirPath;

        public WurmPaths(IWurmClientInstallDirectory wurmInstallDirectory, [NotNull] IWurmApiConfig wurmApiConfig)
        {
            if (wurmApiConfig == null) throw new ArgumentNullException(nameof(wurmApiConfig));
            this.wurmApiConfig = wurmApiConfig;
            ConfigsDirFullPath = Path.Combine(wurmInstallDirectory.FullPath, "configs");
            playersDirPath = Path.Combine(wurmInstallDirectory.FullPath, "players");
        }

        public string LogsDirName => "logs";

        public string OldWuLogsDirName => "test_logs";

        public string ConfigsDirFullPath { get; }

        public string CharactersDirFullPath => playersDirPath;

        public string GetSkillDumpsFullPathForCharacter(CharacterName characterName)
        {
            return Path.Combine(playersDirPath,
                characterName.Capitalized,
                "dumps");
        }

        public string GetLogsDirFullPathForCharacter(CharacterName characterName)
        {
            return Path.Combine(playersDirPath,
                characterName.Capitalized,
                LogsDirName);
        }

        public string GetOldWuLogsDirFullPathForCharacter(CharacterName characterName)
        {
            return Path.Combine(playersDirPath,
                characterName.Capitalized,
                OldWuLogsDirName);
        }

        public string GetScreenshotsDirFullPathForCharacter(CharacterName characterName)
        {
            return Path.Combine(playersDirPath,
                characterName.Capitalized,
                "screenshots");
        }
    }
}
