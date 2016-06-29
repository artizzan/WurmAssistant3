using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers.Meditation
{
    public partial class MeditationTimerOptionsForm : ExtendedForm
    {
        readonly MeditationTimer meditationTimer;
        readonly bool inited;

        public MeditationTimerOptionsForm(MeditationTimer timer)
        {
            meditationTimer = timer;
            InitializeComponent();
            checkBoxRemindSleepBonus.Checked = meditationTimer.SleepBonusReminder;
            int popupDurationMillis = meditationTimer.SleepBonusPopupDurationMillis;
            this.numericUpDownPopupDuration.Value = (
                popupDurationMillis/1000).ConstrainToRange(
                    (int) numericUpDownPopupDuration.Minimum,
                    (int) numericUpDownPopupDuration.Maximum);
            checkBoxShowMeditSkill.Checked = meditationTimer.ShowMeditSkill;
            checkBoxCount.Checked = meditationTimer.ShowMeditCount;
            inited = true;
        }

        private void MeditationTimerOptions_Load(object sender, EventArgs e)
        {
            if (this.Visible)
            {
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
            if (inited) meditationTimer.SleepBonusReminder = checkBoxRemindSleepBonus.Checked;
        }

        private void numericUpDownPopupDuration_ValueChanged(object sender, EventArgs e)
        {
            if (inited) meditationTimer.SleepBonusPopupDurationMillis = ((int)numericUpDownPopupDuration.Value) * 1000;
        }

        private void checkBoxShowMeditSkill_CheckedChanged(object sender, EventArgs e)
        {
            if (inited) meditationTimer.ShowMeditSkill = checkBoxShowMeditSkill.Checked;
        }

        private void checkBoxCount_CheckedChanged(object sender, EventArgs e)
        {
            if (inited) meditationTimer.ShowMeditCount = checkBoxCount.Checked;
        }
    }
}
