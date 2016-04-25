using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
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
