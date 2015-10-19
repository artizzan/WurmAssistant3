using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Prayer;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Prayer
{
    public partial class PrayerTimerOptionsForm : ExtendedForm
    {
        private PrayerTimer prayerTimer;
        private TimerDefaultSettingsForm formSettings;
        readonly ISoundEngine soundEngine;

        public PrayerTimerOptionsForm(PrayerTimer prayerTimer, TimerDefaultSettingsForm form,
            [NotNull] ISoundEngine soundEngine)
        {
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            InitializeComponent();
            this.prayerTimer = prayerTimer;
            this.formSettings = form;
            this.soundEngine = soundEngine;

            numericUpDownFavorWhenThis.Value =
                ((decimal) prayerTimer.FavorSettings.FavorNotifyOnLevel).ConstrainToRange(0M, 100M);
            checkBoxPopupPersist.Checked = prayerTimer.FavorSettings.FavorNotifyPopupPersist;
            textBoxSoundName.Text = prayerTimer.FavorSettings.FavorNotifySoundName;
            checkBoxNotifySound.Checked = prayerTimer.FavorSettings.FavorNotifySound;
            checkBoxNotifyPopup.Checked = prayerTimer.FavorSettings.FavorNotifyPopup;
            checkBoxFavorWhenMAX.Checked = prayerTimer.FavorSettings.FavorNotifyWhenMAX;
            checkBoxShowFaithSkill.Checked = prayerTimer.ShowFaithSkillOnTimer;
        }

        private void PrayerTimerOptions_Load(object sender, EventArgs e)
        {
            if (this.Visible) this.Location = GetCenteredChildPositionRelativeToParentWorkAreaBoundEx(this, formSettings);
        }

        private void checkBoxNotifySound_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSoundName.Visible
                = buttonChangeSound.Visible
                = prayerTimer.FavorSettings.FavorNotifySound
                = checkBoxNotifySound.Checked;
            prayerTimer.FlagAsChanged();
        }

        private void checkBoxNotifyPopup_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxPopupPersist.Visible
                = prayerTimer.FavorSettings.FavorNotifyPopup
                = checkBoxNotifyPopup.Checked;
            prayerTimer.FlagAsChanged();
        }

        private void buttonChangeSound_Click(object sender, EventArgs e)
        {
            var result = soundEngine.ChooseSound();
            if (result.ActionResult == ActionResult.Ok)
            {
                //todo reimpl: use guid not string
                var soundName = result.SoundResource.Id.ToString();
                textBoxSoundName.Text = soundName;
                prayerTimer.FavorSettings.FavorNotifySoundName = soundName;
                prayerTimer.ForceUpdateFavorNotify(soundName);
                prayerTimer.FlagAsChanged();
            }
        }

        private void checkBoxPopupPersist_CheckedChanged(object sender, EventArgs e)
        {
            prayerTimer.FavorSettings.FavorNotifyPopupPersist = checkBoxPopupPersist.Checked;
            prayerTimer.ForceUpdateFavorNotify(popupPersistent:checkBoxPopupPersist.Checked);
            prayerTimer.FlagAsChanged();
        }

        private void numericUpDownFavorWhenThis_ValueChanged(object sender, EventArgs e)
        {
            prayerTimer.FavorSettings.FavorNotifyOnLevel
                = ((float)numericUpDownFavorWhenThis.Value).ConstrainToRange(0F, 100F);
            prayerTimer.FlagAsChanged();
        }

        private void checkBoxFavorWhenMAX_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFavorWhenThis.Enabled = !checkBoxFavorWhenMAX.Checked;
            prayerTimer.FavorSettings.FavorNotifyWhenMAX = checkBoxFavorWhenMAX.Checked;
            prayerTimer.FlagAsChanged();
        }

        private void checkBoxShowFaithSkill_CheckedChanged(object sender, EventArgs e)
        {
            prayerTimer.ShowFaithSkillOnTimer = checkBoxShowFaithSkill.Checked;
        }
    }
}
