using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormGrangerNewInfo : Form
    {
        private GrangerSettings Settings;

        public FormGrangerNewInfo(GrangerSettings Settings)
        {
            this.Settings = Settings;
            InitializeComponent();
        }

        private void FormGrangerNewInfo_Load(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.DoNotShowReadFirstWindow = checkBox1.Checked;
        }
    }
}
