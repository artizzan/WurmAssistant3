using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Views
{
    public partial class FormGrangerGeneralOptions : ExtendedForm
    {
        private readonly GrangerSettings settings; 

        public FormGrangerGeneralOptions(GrangerSettings settings)
        {
            this.settings = settings;
            InitializeComponent();
            InitGuiValues();
        }

        private void InitGuiValues()
        {
            checkBoxAlwaysUpdateUnlessMultiples.Checked = settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb;
            timeSpanInputGroomingTime.Value = settings.ShowGroomingTime;
            checkBoxUpdateAgeHealthAllEvents.Checked = settings.UpdateCreatureDataFromAnyEventLine;
            checkBoxDisableRowColoring.Checked = settings.DisableRowColoring;
            checkBoxAdjustForDarkThemes.Checked = settings.AdjustForDarkThemes;
            checkBoxUseServerNameAsIdComponent.Checked = settings.UseServerNameAsCreatureIdComponent;
        }

        private void CommitChanges()
        {
            settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb = checkBoxAlwaysUpdateUnlessMultiples.Checked;
            settings.ShowGroomingTime = timeSpanInputGroomingTime.Value;
            settings.UpdateCreatureDataFromAnyEventLine = checkBoxUpdateAgeHealthAllEvents.Checked;
            settings.DisableRowColoring = checkBoxDisableRowColoring.Checked;
            settings.AdjustForDarkThemes = checkBoxAdjustForDarkThemes.Checked;
            settings.UseServerNameAsCreatureIdComponent = checkBoxUseServerNameAsIdComponent.Checked;
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
