using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.Notifiers
{
    public partial class SoundConfig : UserControl, INotifierConfig
    {
        private readonly ISoundNotifier soundNotifier;
        readonly ISoundEngine soundEngine;

        public UserControl ControlHandle { get { return this; } }

        private static Color _soundTextBoxDefBackColor;

        public event EventHandler Removed;

        public SoundConfig(ISoundNotifier soundNotifier, [NotNull] ISoundEngine soundEngine)
        {
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            InitializeComponent();
            _soundTextBoxDefBackColor = SoundTextBox.BackColor;
            this.soundNotifier = soundNotifier;
            this.soundEngine = soundEngine;

            SetSoundTextBoxText(this.soundEngine.GetSoundById(this.soundNotifier.SoundId).Name);
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            var result = soundEngine.ChooseSound();
            if (result.ActionResult == ActionResult.Ok)
            {
                soundNotifier.SoundId = result.SoundResource.Id;
                SetSoundTextBoxText(soundEngine.GetSoundById(this.soundNotifier.SoundId).Name);
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
            soundEngine.PlayOneShot(soundNotifier.SoundId);
        }

        protected void OnRemoved()
        {
            var handler = Removed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
