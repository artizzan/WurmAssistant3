using System;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormGrangerNewInfo : Form
    {
        readonly GrangerSettings settings;

        public FormGrangerNewInfo([NotNull] GrangerSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            this.settings = settings;
            InitializeComponent();
        }

        private void FormGrangerNewInfo_Load(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            settings.DoNotShowReadFirstWindow = checkBox1.Checked;
        }
    }
}
