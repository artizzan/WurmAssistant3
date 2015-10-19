using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers
{
    public partial class TimerDefaultSettingsForm : ExtendedForm
    {
        WurmTimer ParentTimer;
        readonly ISoundEngine soundEngine;
        bool isInited = false;
        public TimerDefaultSettingsForm(WurmTimer wurmTimer, [NotNull] ISoundEngine soundEngine)
        {
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            ParentTimer = wurmTimer;
            this.soundEngine = soundEngine;
            InitializeComponent();
            if (wurmTimer.MoreOptionsAvailable) buttonMoreOptions.Visible = true;
            this.Text = wurmTimer.Name;
            //set all options values
            this.checkBoxPopup.Checked = ParentTimer.PopupNotify;
            this.checkBoxSound.Checked = ParentTimer.SoundNotify;
            this.checkBoxPopupPersistent.Checked = ParentTimer.PersistentPopup;
            this.textBoxSoundName.Text = ParentTimer.SoundName;
            this.checkBoxOnAssistantLaunch.Checked = ParentTimer.PopupOnWaLaunch;
            int popupDurationMillis = ParentTimer.PopupDurationMillis;
            this.numericUpDownPopupDuration.Value =
                (popupDurationMillis/1000).ConstrainToRange((int) numericUpDownPopupDuration.Minimum,
                    (int) numericUpDownPopupDuration.Maximum);
            isInited = true;
        }

        private void FormTimerSettingsDefault_Load(object sender, EventArgs e)
        {
            if (this.Visible)
                this.Location = GetCenteredChildPositionRelativeToParentWorkAreaBoundEx(this, ParentTimer.GetModuleUi());
            this.toolTip1.SetToolTip(this.checkBoxPopupPersistent, "Popup must be closed manually");
            this.toolTip1.SetToolTip(this.buttonTurnOff, "Turn off this timer (your settings will be preserved)");
        }

        void UpdatePanels()
        {
            if (checkBoxPopup.Checked) panelPopup.Visible = true;
            else panelPopup.Visible = false;
            if (checkBoxSound.Checked) panelSoundNotify.Visible = true;
            else panelSoundNotify.Visible = false;
        }

        private void buttonMoreOptions_Click(object sender, EventArgs e)
        {
            ParentTimer.OpenMoreOptions(this);
        }

        private void checkBoxPopup_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePanels();
            if (isInited)
            {
                ParentTimer.PopupNotify = checkBoxPopup.Checked;
            }
        }

        private void checkBoxSound_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePanels();
            if (isInited)
            {
                ParentTimer.SoundNotify = checkBoxSound.Checked;
            }
        }

        private void checkBoxPopupPersistent_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPopupPersistent.Checked)
            {
                numericUpDownPopupDuration.Enabled = false;
            }
            else
            {
                numericUpDownPopupDuration.Enabled = true;
            }

            if (isInited)
            {
                ParentTimer.PersistentPopup = checkBoxPopupPersistent.Checked;
            }
        }

        private void buttonSoundChange_Click(object sender, EventArgs e)
        {
            if (isInited)
            {
                var result = soundEngine.ChooseSound();
                if (result.ActionResult == ActionResult.Ok)
                {
                    string newsound = result.SoundResource.Id.ToString();
                    textBoxSoundName.Text = newsound;
                    ParentTimer.SoundName = newsound;
                }
            }
        }

        private void buttonTurnOff_Click(object sender, EventArgs e)
        {
            ParentTimer.TurnOff();
            this.Close();
        }

        private void checkBoxOnAssistantLaunch_CheckedChanged(object sender, EventArgs e)
        {
            if (isInited)
            {
                ParentTimer.PopupOnWaLaunch = checkBoxOnAssistantLaunch.Checked;
            }
        }

        private void numericUpDownPopupDuration_ValueChanged(object sender, EventArgs e)
        {
            if (isInited)
            {
                ParentTimer.PopupDurationMillis = ((int)numericUpDownPopupDuration.Value) * 1000;
            }
        }
    }
}
