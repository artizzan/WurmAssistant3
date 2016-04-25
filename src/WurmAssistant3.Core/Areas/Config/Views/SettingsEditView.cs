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
using AldursLab.WurmAssistant3.Core.Areas.WurmApi.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Views
{
    public partial class SettingsEditView : ExtendedForm
    {
        readonly IWurmAssistantConfig wurmAssistantConfig;
        readonly IWurmClientValidator wurmClientValidator;
        readonly IUserNotifier userNotifier;

        public SettingsEditView([NotNull] IWurmAssistantConfig wurmAssistantConfig,
            [NotNull] IWurmClientValidator wurmClientValidator, [NotNull] IUserNotifier userNotifier)
        {
            if (wurmAssistantConfig == null) throw new ArgumentNullException("wurmAssistantConfig");
            if (wurmClientValidator == null) throw new ArgumentNullException("wurmClientValidator");
            if (userNotifier == null) throw new ArgumentNullException("userNotifier");
            this.wurmAssistantConfig = wurmAssistantConfig;
            this.wurmClientValidator = wurmClientValidator;
            this.userNotifier = userNotifier;
            InitializeComponent();

            firstTimeSetupAgain.Checked = wurmAssistantConfig.ReSetupRequested;
            cleanWurmApiCaches.Checked = wurmAssistantConfig.DropAllWurmApiCachesToggle;
        }

        private void firstTimeSetupAgain_CheckedChanged(object sender, EventArgs e)
        {
            wurmAssistantConfig.ReSetupRequested = firstTimeSetupAgain.Checked;
        }

        private void cleanWurmApiCaches_CheckedChanged(object sender, EventArgs e)
        {
            wurmAssistantConfig.DropAllWurmApiCachesToggle = cleanWurmApiCaches.Checked;
        }

        private void validateConfigButton_Click(object sender, EventArgs e)
        {
            var result = wurmClientValidator.Validate();
            if (result.Any())
            {
                wurmClientValidator.ShowSummaryWindow(result);
            }
            else
            {
                userNotifier.NotifyWithMessageBox("No issues found");
            }
        }
    }
}
