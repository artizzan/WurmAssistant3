using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Core;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Main
{
    [KernelBind(BindingHint.Singleton), PersistentObject("WurmUnlimitedLogsDirChecker")]
    public class WurmUnlimitedLogsDirChecker : PersistentObjectBase
    {
        readonly IWurmPaths wurmPaths;
        readonly IWurmCharacters wurmCharacters;
        readonly IUserNotifier userNotifier;
        readonly IWurmAssistantConfig wurmAssistantConfig;

        [JsonProperty] bool warningShown;

        public WurmUnlimitedLogsDirChecker([NotNull] IWurmApi wurmApi, [NotNull] IUserNotifier userNotifier,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            this.wurmPaths = wurmApi.Paths ?? throw new ArgumentNullException(nameof(wurmPaths));
            this.wurmCharacters = wurmApi.Characters ?? throw new ArgumentNullException(nameof(wurmCharacters));
            this.userNotifier = userNotifier ?? throw new ArgumentNullException(nameof(userNotifier));
            this.wurmAssistantConfig = wurmAssistantConfig ?? throw new ArgumentNullException(nameof(wurmAssistantConfig));
        }

        public void HandleOldLogsDirContents()
        {
            if (wurmAssistantConfig.WurmUnlimitedMode && !warningShown)
            {
                var charactersWithOldLogs =
                    wurmCharacters.All
                                  .Select(character => new
                                  {
                                      CharacterName = character.Name,
                                      LogsPath = wurmPaths.GetOldWuLogsDirFullPathForCharacter(character.Name)
                                  })
                                  .Select(arg => new
                                  {
                                      CharacterName = arg.CharacterName,
                                      FileCount = Directory.Exists(arg.LogsPath) ? Directory.EnumerateFiles(arg.LogsPath).Count() : 0,
                                      LogsPath = arg.LogsPath
                                  })
                                  .Where(arg => arg.FileCount > 0)
                                  .ToList();

                if (charactersWithOldLogs.Any())
                {
                    userNotifier.NotifyWithMessageBox("Recently Wurm Unlimited has changed where it saves Wurm log files. "
                                                      + "The old directory was named 'test_logs', while the new one is just 'logs'. "
                                                      + $"Wurm Assistant has found some logs at the old directories for characters:\r\n\r\n"
                                                      + $"{string.Join(",\r\n\r\n", charactersWithOldLogs.Select(arg => $"{arg.CharacterName} has {arg.FileCount} files at {arg.LogsPath}"))}.\r\n\r\n"
                                                      + "While nothing will outright break because of this, some Wurm Assistant features "
                                                      + "require log searches for some things, for example to find current skill levels. "
                                                      + "If you notice any issue with eg. meditation timer or smilexamines, "
                                                      + "consider moving/merging all log files from old directory to the new one.");
                }

                warningShown = true;
                FlagAsChanged();
            }
        }
    }
}
