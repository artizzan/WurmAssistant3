using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
{
    public partial class FormGrangerGeneralOptions : ExtendedForm
    {
        private readonly GrangerSettings _settings; 

        public FormGrangerGeneralOptions(GrangerSettings settings)
        {
            _settings = settings;
            InitializeComponent();
            InitGuiValues();
        }

        private void InitGuiValues()
        {
            checkBoxAlwaysUpdateUnlessMultiples.Checked = _settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb;
            timeSpanInputGroomingTime.Value = _settings.ShowGroomingTime;
            checkBoxUpdateAgeHealthAllEvents.Checked = _settings.UpdateCreatureDataFromAnyEventLine;
            checkBoxDisableRowColoring.Checked = _settings.DisableRowColoring;
            checkBoxAdjustForDarkThemes.Checked = _settings.AdjustForDarkThemes;
        }

        private void CommitChanges()
        {
            _settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb = checkBoxAlwaysUpdateUnlessMultiples.Checked;
            _settings.ShowGroomingTime = timeSpanInputGroomingTime.Value;
            _settings.UpdateCreatureDataFromAnyEventLine = checkBoxUpdateAgeHealthAllEvents.Checked;
            _settings.DisableRowColoring = checkBoxDisableRowColoring.Checked;
            _settings.AdjustForDarkThemes = checkBoxAdjustForDarkThemes.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            CommitChanges();
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
