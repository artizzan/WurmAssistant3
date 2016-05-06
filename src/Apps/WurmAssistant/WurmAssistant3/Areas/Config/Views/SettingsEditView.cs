using System;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.WurmApi.Contracts;
using AldursLab.WurmAssistant3.Root.Contracts;
using AldursLab.WurmAssistant3.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Config.Views
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
