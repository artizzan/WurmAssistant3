using System;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Sermoner
{
    public partial class SermonerSettingsForm : ExtendedForm
    {
        private readonly SermonerForm sermonerForm;
        private readonly SermonerSettings sermonerSettings;
        public SermonerSettingsForm(SermonerForm sermonerForm, SermonerSettings sermonerSettings)
        {
            this.sermonerForm = sermonerForm;
            this.sermonerSettings = sermonerSettings;
            InitializeComponent();

            this.numOmitOldPreachers.Value = sermonerSettings.OmitOldPreacherTime;
        }

        private void numOmitOldPreachers_ValueChanged(object sender, EventArgs e)
        {
            sermonerSettings.OmitOldPreacherTime = (int)numOmitOldPreachers.Value;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
