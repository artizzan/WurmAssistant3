using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.Prayer;
using AldursLab.WurmAssistant3.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Timers.Views.Timers.Prayer
{
    public partial class PrayerTimerOptionsForm : ExtendedForm
    {
        private readonly PrayerTimer prayerTimer;
        private TimerDefaultSettingsForm formSettings;
        readonly ISoundManager soundManager;

        public PrayerTimerOptionsForm(PrayerTimer prayerTimer, TimerDefaultSettingsForm form,
            [NotNull] ISoundManager soundManager)
        {
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            InitializeComponent();
            this.prayerTimer = prayerTimer;
            this.formSettings = form;
            this.soundManager = soundManager;

            numericUpDownFavorWhenThis.Value =
                ((decimal) prayerTimer.FavorSettings.FavorNotifyOnLevel).ConstrainToRange(0M, 100M);
            checkBoxPopupPersist.Checked = prayerTimer.FavorSettings.FavorNotifyPopupPersist;
            textBoxSoundName.Text = soundManager.GetSoundById(prayerTimer.FavorSettings.FavorNotifySoundId).Name;
            checkBoxNotifySound.Checked = prayerTimer.FavorSettings.FavorNotifySound;
            checkBoxNotifyPopup.Checked = prayerTimer.FavorSettings.FavorNotifyPopup;
            checkBoxFavorWhenMAX.Checked = prayerTimer.FavorSettings.FavorNotifyWhenMax;
            checkBoxShowFaithSkill.Checked = prayerTimer.ShowFaithSkillOnTimer;
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
            var result = soundManager.ChooseSound(this);
            if (result.ActionResult == ActionResult.Ok)
            {
                textBoxSoundName.Text = result.SoundResource.Name;
                prayerTimer.FavorSettings.FavorNotifySoundId = result.SoundResource.Id;
                prayerTimer.ForceUpdateFavorNotify(result.SoundResource.Id);
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
            prayerTimer.FavorSettings.FavorNotifyWhenMax = checkBoxFavorWhenMAX.Checked;
            prayerTimer.FlagAsChanged();
        }

        private void checkBoxShowFaithSkill_CheckedChanged(object sender, EventArgs e)
        {
            prayerTimer.ShowFaithSkillOnTimer = checkBoxShowFaithSkill.Checked;
        }
    }
}
