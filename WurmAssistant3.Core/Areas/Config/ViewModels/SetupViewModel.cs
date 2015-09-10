using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Model;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.ViewModels
{
    public class SetupViewModel
    {
        readonly WurmAssistantSettings settings;

        public SetupViewModel([NotNull] WurmAssistantSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            this.settings = settings;
        }

        public string WurmOnlineClientInstallPath
        {
            get { return settings.WurmGameClientInstallDirectory; }
            set { settings.WurmGameClientInstallDirectory = value; }
        }

        public Platform Platform
        {
            get { return settings.RunningPlatform; }
            set { settings.RunningPlatform = value; }
        }
    }
}
