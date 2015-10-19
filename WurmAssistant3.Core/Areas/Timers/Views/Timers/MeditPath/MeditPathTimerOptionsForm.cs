using System;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.MeditPath;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.MeditPath
{
    public partial class MeditPathTimerOptionsForm : ExtendedForm
    {
        private TimerDefaultSettingsForm defaultSettingsForm;
        private MeditPathTimer meditPathTimer;

        public MeditPathTimerOptionsForm(TimerDefaultSettingsForm defaultSettingsForm, MeditPathTimer meditPathTimer)
        {
            this.defaultSettingsForm = defaultSettingsForm;
            this.meditPathTimer = meditPathTimer;
            InitializeComponent();
        }

        bool ManualCDSet = false;

        private void MeditPathTimerOptions_Load(object sender, EventArgs e)
        {
            if (this.Visible) this.Location = GetCenteredChildPositionRelativeToParentWorkAreaBoundEx(this, defaultSettingsForm);
            if (meditPathTimer.NextQuestionAttemptOverridenUntil != DateTime.MinValue) ManualCDSet = true;
            RefreshButtonText();
        }

        private void buttonManualCD_Click(object sender, EventArgs e)
        {
            if (ManualCDSet)
            {
                meditPathTimer.NextQuestionAttemptOverridenUntil = DateTime.MinValue;
                ManualCDSet = false;
                RefreshButtonText();
                meditPathTimer.UpdateCooldown();
            }
            else
            {
                ChooseQuestionTimerManuallyForm ui = new ChooseQuestionTimerManuallyForm(meditPathTimer);
                if (ui.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    meditPathTimer.SetManualQTimer(ui.GetResultMeditLevel(), ui.GetResultOriginDate());
                    ManualCDSet = true;
                    RefreshButtonText();
                }
            }
        }

        void RefreshButtonText()
        {
            if (ManualCDSet) buttonManualCD.Text = "Manual cooldown is set\r\nClick to remove";
            else buttonManualCD.Text = "Set cooldown manually";
        }
    }
}
