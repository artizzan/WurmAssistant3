using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Meditation;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Meditation
{
    public partial class MeditationTimerOptionsForm : ExtendedForm
    {
        MeditationTimer MedTimer;
        TimerDefaultSettingsForm parentForm;
        bool inited = false;
        public MeditationTimerOptionsForm(MeditationTimer timer, TimerDefaultSettingsForm parent)
        {
            MedTimer = timer;
            parentForm = parent;
            InitializeComponent();
            checkBoxRemindSleepBonus.Checked = MedTimer.SleepBonusReminder;
            int popupDurationMillis = MedTimer.SleepBonusPopupDuration;
            this.numericUpDownPopupDuration.Value = (
                popupDurationMillis/1000).ConstrainToRange(
                    (int) numericUpDownPopupDuration.Minimum,
                    (int) numericUpDownPopupDuration.Maximum);
            checkBoxShowMeditSkill.Checked = MedTimer.ShowMeditSkill;
            checkBoxCount.Checked = MedTimer.ShowMeditCount;
            inited = true;
        }

        private void MeditationTimerOptions_Load(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.Location = GetCenteredChildPositionRelativeToParentWorkAreaBoundEx(this, parentForm);
                UpdateSleepBonusPanelVisibility();
            }
            toolTip1.SetToolTip(checkBoxRemindSleepBonus, "If sleep bonus was turned on just before meditation,\r\nthis will pop a reminder once it can be turned off");
        }

        void UpdateSleepBonusPanelVisibility()
        {
            if (checkBoxRemindSleepBonus.Checked) panelPopupDuration.Visible = true;
            else panelPopupDuration.Visible = false;
        }

        private void checkBoxRemindSleepBonus_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSleepBonusPanelVisibility();
            if (inited) MedTimer.SleepBonusReminder = checkBoxRemindSleepBonus.Checked;
        }

        private void numericUpDownPopupDuration_ValueChanged(object sender, EventArgs e)
        {
            if (inited) MedTimer.SleepBonusPopupDuration = ((int)numericUpDownPopupDuration.Value) * 1000;
        }

        private void checkBoxShowMeditSkill_CheckedChanged(object sender, EventArgs e)
        {
            if (inited) MedTimer.ShowMeditSkill = checkBoxShowMeditSkill.Checked;
        }

        private void checkBoxCount_CheckedChanged(object sender, EventArgs e)
        {
            if (inited) MedTimer.ShowMeditCount = checkBoxCount.Checked;
        }
    }
}
