using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Views
{
    public partial class SettingsEditView : ExtendedForm
    {
        readonly IWurmAssistantConfig wurmAssistantConfig;

        public SettingsEditView([NotNull] IWurmAssistantConfig wurmAssistantConfig)
        {
            if (wurmAssistantConfig == null) throw new ArgumentNullException("wurmAssistantConfig");
            this.wurmAssistantConfig = wurmAssistantConfig;
            InitializeComponent();

            firstTimeSetupAgain.Checked = wurmAssistantConfig.ReSetupRequested;
            cleanWurmApiCaches.Checked = wurmAssistantConfig.DropAllWurmApiCachesToggle;
            MinimizeToTray.Checked = wurmAssistantConfig.MinimizeToTrayEnabled;
        }

        private void firstTimeSetupAgain_CheckedChanged(object sender, EventArgs e)
        {
            wurmAssistantConfig.ReSetupRequested = firstTimeSetupAgain.Checked;
        }

        private void cleanWurmApiCaches_CheckedChanged(object sender, EventArgs e)
        {
            wurmAssistantConfig.DropAllWurmApiCachesToggle = cleanWurmApiCaches.Checked;
        }

        private void MinimizeToTray_CheckedChanged(object sender, EventArgs e)
        {
            wurmAssistantConfig.MinimizeToTrayEnabled = MinimizeToTray.Checked;
        }
    }
}
