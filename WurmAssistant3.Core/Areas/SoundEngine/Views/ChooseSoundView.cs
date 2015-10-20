using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Views
{
    public partial class ChooseSoundView : ExtendedForm
    {
        readonly ISoundsLibrary soundsLibrary;
        readonly ISoundEngine soundEngine;

        public ChooseSoundView([NotNull] ISoundsLibrary soundsLibrary, [NotNull] ISoundEngine soundEngine)
        {
            if (soundsLibrary == null) throw new ArgumentNullException("soundsLibrary");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            this.soundsLibrary = soundsLibrary;
            this.soundEngine = soundEngine;
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
            soundEngine.ShowSoundManager();
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
            soundEngine.StopAllSounds();
            if (listBox1.SelectedIndex > -1)
            {
                soundEngine.PlayOneShot((ISoundResource) listBox1.SelectedItem);
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
