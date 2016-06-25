using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers
{
    public partial class TimerDefaultSettingsForm : ExtendedForm
    {
        readonly WurmTimer parentTimer;
        readonly ISoundManager soundManager;
        readonly bool isInited = false;

        public TimerDefaultSettingsForm(WurmTimer wurmTimer, [NotNull] ISoundManager soundManager)
        {
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            parentTimer = wurmTimer;
            this.soundManager = soundManager;
            InitializeComponent();
            if (wurmTimer.MoreOptionsAvailable) buttonMoreOptions.Visible = true;
            this.Text = wurmTimer.Name;
            //set all options values
            this.checkBoxPopup.Checked = parentTimer.PopupNotify;
            this.checkBoxSound.Checked = parentTimer.SoundNotify;
            this.checkBoxPopupPersistent.Checked = parentTimer.PersistentPopup;
            this.textBoxSoundName.Text = soundManager.GetSoundById(parentTimer.SoundId).Name;
            this.checkBoxOnAssistantLaunch.Checked = parentTimer.PopupOnWaLaunch;
            int popupDurationMillis = parentTimer.PopupDurationMillis;
            this.numericUpDownPopupDuration.Value =
                (popupDurationMillis/1000).ConstrainToRange((int) numericUpDownPopupDuration.Minimum,
                    (int) numericUpDownPopupDuration.Maximum);
            isInited = true;
        }

        private void FormTimerSettingsDefault_Load(object sender, EventArgs e)
        {
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
            parentTimer.OpenMoreOptions(this);
        }

        private void checkBoxPopup_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePanels();
            if (isInited)
            {
                parentTimer.PopupNotify = checkBoxPopup.Checked;
            }
        }

        private void checkBoxSound_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePanels();
            if (isInited)
            {
                parentTimer.SoundNotify = checkBoxSound.Checked;
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
                parentTimer.PersistentPopup = checkBoxPopupPersistent.Checked;
            }
        }

        private void buttonSoundChange_Click(object sender, EventArgs e)
        {
            if (isInited)
            {
                var result = soundManager.ChooseSound(this);
                if (result.ActionResult == ActionResult.Ok)
                {
                    var newsound = result.SoundResource;
                    textBoxSoundName.Text = newsound.Name;
                    parentTimer.SoundId = newsound.Id;
                }
            }
        }

        private void buttonTurnOff_Click(object sender, EventArgs e)
        {
            parentTimer.TurnOff();
            this.Close();
        }

        private void checkBoxOnAssistantLaunch_CheckedChanged(object sender, EventArgs e)
        {
            if (isInited)
            {
                parentTimer.PopupOnWaLaunch = checkBoxOnAssistantLaunch.Checked;
            }
        }

        private void numericUpDownPopupDuration_ValueChanged(object sender, EventArgs e)
        {
            if (isInited)
            {
                parentTimer.PopupDurationMillis = ((int)numericUpDownPopupDuration.Value) * 1000;
            }
        }
    }
}
