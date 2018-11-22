using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormGrangerGeneralOptions : ExtendedForm
    {
        readonly GrangerSettings settings;

        public FormGrangerGeneralOptions(
            [NotNull] GrangerSettings settings,
            [NotNull] IFormEditCreatureColorsFactory formEditCreatureColorsFactory)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (formEditCreatureColorsFactory == null)
                throw new ArgumentNullException(nameof(formEditCreatureColorsFactory));
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
            checkBoxhideLiveTrackerPopups.Checked = settings.HideLiveTrackerPopups;
            checkBoxDoNotMatchCreaturesByBrandName.Checked = settings.DoNotMatchCreaturesByBrandName;
            checkBoxUpdateCreatureColorsOnSmilexamines.Checked = settings.UpdateCreatureColorOnSmilexamines;
            checkBoxRequireServerAndSkillToBeKnownForSmilexamine.Checked = settings.RequireServerAndSkillToBeKnownForSmilexamine;
        }

        private void CommitChanges()
        {
            settings.DoNotBlockDataUpdateUnlessMultiplesInEntireDb = checkBoxAlwaysUpdateUnlessMultiples.Checked;
            settings.ShowGroomingTime = timeSpanInputGroomingTime.Value;
            settings.UpdateCreatureDataFromAnyEventLine = checkBoxUpdateAgeHealthAllEvents.Checked;
            settings.DisableRowColoring = checkBoxDisableRowColoring.Checked;
            settings.AdjustForDarkThemes = checkBoxAdjustForDarkThemes.Checked;
            settings.UseServerNameAsCreatureIdComponent = checkBoxUseServerNameAsIdComponent.Checked;
            settings.HideLiveTrackerPopups = checkBoxhideLiveTrackerPopups.Checked;
            settings.DoNotMatchCreaturesByBrandName = checkBoxDoNotMatchCreaturesByBrandName.Checked;
            settings.UpdateCreatureColorOnSmilexamines = checkBoxUpdateCreatureColorsOnSmilexamines.Checked;
            settings.RequireServerAndSkillToBeKnownForSmilexamine = checkBoxRequireServerAndSkillToBeKnownForSmilexamine.Checked;
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
