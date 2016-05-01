using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class UpdateSourceUpdater
    {
        readonly IWurmAssistantService wurmAssistantMainService;
        readonly UserSettings userSettings;

        string result;

        public UpdateSourceUpdater([NotNull] IWurmAssistantService wurmAssistantMainService,
            [NotNull] UserSettings userSettings)
        {
            if (wurmAssistantMainService == null) throw new ArgumentNullException("wurmAssistantMainService");
            if (userSettings == null) throw new ArgumentNullException("userSettings");
            this.wurmAssistantMainService = wurmAssistantMainService;
            this.userSettings = userSettings;
        }

        public async Task FetchUpdateSourceHost()
        {
            result = await wurmAssistantMainService.GetCurrentUpdateSourceHost();
        }

        public void CommitUpdatedSourceHost()
        {
            userSettings.WurmAssistantWebServiceUrl = result;
        }
    }
}
