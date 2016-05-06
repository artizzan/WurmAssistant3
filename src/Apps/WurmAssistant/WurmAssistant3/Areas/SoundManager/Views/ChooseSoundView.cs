using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Views
{
    public partial class ChooseSoundView : ExtendedForm
    {
        readonly ISoundsLibrary soundsLibrary;
        readonly ISoundManager soundManager;

        public ChooseSoundView([NotNull] ISoundsLibrary soundsLibrary, [NotNull] ISoundManager soundManager)
        {
            if (soundsLibrary == null) throw new ArgumentNullException("soundsLibrary");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            this.soundsLibrary = soundsLibrary;
            this.soundManager = soundManager;
            InitializeComponent();
            RefreshList();

            soundsLibrary.SoundsChanged += SoundsLibraryOnSoundsChanged;
            this.Closed += (sender, args) => soundsLibrary.SoundsChanged -= SoundsLibraryOnSoundsChanged;
        }

        void SoundsLibraryOnSoundsChanged(object sender, EventArgs eventArgs)
        {
            RefreshList();
        }

        public ISoundResource ChosenSound { get; private set; }

        void RefreshList()
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(soundsLibrary.GetAllSounds().Cast<object>().ToArray());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChosenSound = (ISoundResource)listBox1.SelectedItem;
        }

        private void buttonAddSound_Click(object sender, EventArgs e)
        {
            soundManager.ShowSoundManager();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            PlaySound();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            PlaySound();
        }

        void PlaySound()
        {
            soundManager.StopAllSounds();
            if (listBox1.SelectedIndex > -1)
            {
                soundManager.PlayOneShot((ISoundResource) listBox1.SelectedItem);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ChosenSound == null)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                MessageBox.Show("Please select a sound first");
            }
        }
    }
}
