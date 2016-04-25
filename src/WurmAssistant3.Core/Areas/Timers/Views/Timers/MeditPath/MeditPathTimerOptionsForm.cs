using System;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.MeditPath;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.MeditPath
{
    public partial class MeditPathTimerOptionsForm : ExtendedForm
    {
        private TimerDefaultSettingsForm defaultSettingsForm;
        private readonly MeditPathTimer meditPathTimer;

        bool manualCooldownSet = false;

        public MeditPathTimerOptionsForm(TimerDefaultSettingsForm defaultSettingsForm, MeditPathTimer meditPathTimer)
        {
            this.defaultSettingsForm = defaultSettingsForm;
            this.meditPathTimer = meditPathTimer;
            InitializeComponent();
        }

        private void MeditPathTimerOptions_Load(object sender, EventArgs e)
        {
            if (meditPathTimer.NextQuestionAttemptOverridenUntil != DateTime.MinValue) manualCooldownSet = true;
            RefreshButtonText();
        }

        private void buttonManualCD_Click(object sender, EventArgs e)
        {
            if (manualCooldownSet)
            {
                meditPathTimer.NextQuestionAttemptOverridenUntil = DateTime.MinValue;
                manualCooldownSet = false;
                RefreshButtonText();
                meditPathTimer.UpdateCooldown();
            }
            else
            {
                ChooseQuestionTimerManuallyForm ui = new ChooseQuestionTimerManuallyForm();
                if (ui.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    meditPathTimer.SetManualQTimer(ui.GetResultMeditLevel(), ui.GetResultOriginDate());
                    manualCooldownSet = true;
                    RefreshButtonText();
                }
            }
        }

        void RefreshButtonText()
        {
            if (manualCooldownSet) buttonManualCD.Text = "Manual cooldown is set\r\nClick to remove";
            else buttonManualCD.Text = "Set cooldown manually";
        }
    }
}
