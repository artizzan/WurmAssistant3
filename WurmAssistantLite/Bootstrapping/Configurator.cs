using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using AldursLab.WurmAssistantLite.Bootstrapping.Persistent;
using AldursLab.WurmAssistantLite.Views;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistantLite.Bootstrapping
{
    class Configurator
    {
        readonly WurmAssistantLiteSettings wurmAssistantLiteSettings;

        public Configurator([NotNull] WurmAssistantLiteSettings wurmAssistantLiteSettings)
        {
            if (wurmAssistantLiteSettings == null) throw new ArgumentNullException("wurmAssistantLiteSettings");
            this.wurmAssistantLiteSettings = wurmAssistantLiteSettings;
        }

        public bool ExecConfig()
        {
            if (!wurmAssistantLiteSettings.SetupRequired)
            {
                return true;
            }

            var setupForm = new WurmAssistantSetupForm
            {
                OperatingSystem = wurmAssistantLiteSettings.RunningPlatform,
                WurmOnlineClientFullPath = wurmAssistantLiteSettings.WurmGameClientInstallDirectory
            };
            var result = setupForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                wurmAssistantLiteSettings.WurmGameClientInstallDirectory = setupForm.WurmOnlineClientFullPath;
                wurmAssistantLiteSettings.RunningPlatform = setupForm.OperatingSystem;
                wurmAssistantLiteSettings.SetupRequired = false;
                return true;
            }

            return false;
        }

        public WurmAssistantConfig BuildWurmAssistantConfig()
        {
            return new WurmAssistantConfig()
            {
                RunningPlatform = wurmAssistantLiteSettings.RunningPlatform,
                DataDirectoryFullPath = wurmAssistantLiteSettings.DataDirectoryFullPath,
                WurmGameClientInstallDirectory = wurmAssistantLiteSettings.WurmGameClientInstallDirectory
            };
        }
    }
}