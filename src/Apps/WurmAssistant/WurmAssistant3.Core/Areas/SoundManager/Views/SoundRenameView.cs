using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundManager.Views
{
    public partial class SoundRenameView : Form
    {
        public string NewName { get { return textBoxRename.Text; } }

        public SoundRenameView(string sndname)
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
