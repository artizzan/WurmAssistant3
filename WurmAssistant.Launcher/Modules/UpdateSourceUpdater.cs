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

        string result;

        public UpdateSourceUpdater([NotNull] IWurmAssistantService wurmAssistantMainService)
        {
            if (wurmAssistantMainService == null) throw new ArgumentNullException("wurmAssistantMainService");
            this.wurmAssistantMainService = wurmAssistantMainService;
        }

        public async Task FetchUpdateSourceHost()
        {
            result = await wurmAssistantMainService.GetCurrentUpdateSourceHost();
        }

        public void CommitUpdatedSourceHost()
        {
            Settings.Default.WurmAssistantWebServiceUrl2 = result;
            Settings.Default.Save();
        }
    }
}
