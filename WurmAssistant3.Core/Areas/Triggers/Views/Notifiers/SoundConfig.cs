using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.Notifiers
{
    public partial class SoundConfig : UserControl, INotifierConfig
    {
        private readonly ISoundNotifier soundNotifier;
        readonly ISoundManager soundManager;

        public UserControl ControlHandle { get { return this; } }

        private static Color _soundTextBoxDefBackColor;

        public event EventHandler Removed;

        public SoundConfig(ISoundNotifier soundNotifier, [NotNull] ISoundManager soundManager)
        {
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            InitializeComponent();
            _soundTextBoxDefBackColor = SoundTextBox.BackColor;
            this.soundNotifier = soundNotifier;
            this.soundManager = soundManager;

            SetSoundTextBoxText(this.soundManager.GetSoundById(this.soundNotifier.SoundId).Name);
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            var result = soundManager.ChooseSound();
            if (result.ActionResult == ActionResult.Ok)
            {
                soundNotifier.SoundId = result.SoundResource.Id;
                SetSoundTextBoxText(soundManager.GetSoundById(this.soundNotifier.SoundId).Name);
            }
        }

        void SetSoundTextBoxText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                SoundTextBox.BackColor = Color.PapayaWhip;
                SoundTextBox.Text = "no sound set";
            }
            else
            {
                SoundTextBox.BackColor = _soundTextBoxDefBackColor;
                SoundTextBox.Text = text;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            OnRemoved();
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            soundManager.PlayOneShot(soundNotifier.SoundId);
        }

        protected void OnRemoved()
        {
            var handler = Removed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
