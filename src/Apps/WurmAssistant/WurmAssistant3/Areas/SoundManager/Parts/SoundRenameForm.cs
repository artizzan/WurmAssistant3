using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.SoundManager.Parts
{
    public partial class SoundRenameForm : Form
    {
        public string NewName { get { return textBoxRename.Text; } }

        public SoundRenameForm(string sndname)
        {
            InitializeComponent();
            textBoxRename.Text = sndname;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}
