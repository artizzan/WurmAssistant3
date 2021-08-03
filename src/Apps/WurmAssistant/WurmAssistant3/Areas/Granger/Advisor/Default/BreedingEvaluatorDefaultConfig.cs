using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Advisor.Default
{
    public partial class BreedingEvaluatorDefaultConfig : ExtendedForm
    {
        private readonly DefaultBreedingEvaluatorOptions options;
        readonly ILogger logger;

        const string SkipPregnantCreaturesTooltip = "Pregnancy is hard to track accurately, the error can be up to a day, Assistant always assumes longest time. Creature needs to be smilexamined after breeding, else Assistant will asume longest possible pregnancy.";
        const string SkipPregnantLast24HTooltip = "This adds 24h to last creature pregnancy";
        const string BadTraitWeightTooltip = "Bad trait value will be multiplied by this weight";
        const string DiscardWithAnyBadTraitsTooltip = "Skips a creature that has one or more bad traits. This does not include potential bad traits. The selected creature is not discarded, you can still click one with bad traits and get breeding suggestions against any non-bad traited creatures.";
        const string IncludePotentialValueTooltip = "This will asume that creature has ALL traits that were not visible upon last inspection (based on inspector animal husbandry skill at that time). It is far from very useful and it is suggested to have skilled player inspect creature for you and then update it manually.";
        const string PotentialGoodWeightTooltip = "Each potential good trait will be multiplied by this weight";
        const string PotentialBadWeightTooltip = "Each potential bad trait will be multiplied by this weight";
        const string PreferUniqueTraitsTooltop = "This will adjust good trait value, if it's present only on one creature. This option is useful for improving your chances to get a creature with more good traits, at expense of getting overall worse creatures";
        const string UniqTraitWeightTooltip = "Unique good trait value will be multiplied by this value";
        const string InbreedWeightTooltip = "Fictional bad trait will be multiplied by this value. Check assistant wiki for a current list of which traits are considered for inbreeding";
        const string SkipPairedCreaturesTooltip = "If you have created any creature pairs, they will be completely excluded from breeding results";
        const string AgeKeepComparingOtherTooltip = "Normally age excluded creatures will not be compared at all. This will let you still see comparisons for the selected underage creature.";

        public BreedingEvaluatorDefaultConfig(
            [NotNull] DefaultBreedingEvaluatorOptions options, 
            [NotNull] ILogger logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.options = options;
            this.logger = logger;
            InitializeComponent();

            checkBoxSkipNotInMood.Checked = this.options.IgnoreNotInMood;
            checkBoxSkipPregnant.Checked = this.options.IgnorePregnant;
            checkBoxSkipGaveBirthInLast24h.Checked = this.options.IgnoreRecentlyPregnant;
            checkBoxSkipCreaturesInOtherHerds.Checked = this.options.IgnoreOtherHerds;
            checkBoxSkipPaired.Checked = this.options.IgnorePairedCreatures;

            checkBoxIgnoreSold.Checked = this.options.IgnoreSold;
            checkBoxIgnoreDead.Checked = this.options.IgnoreDead;

            checkBoxExcludeFoals.Checked = this.options.IgnoreFoals;
            checkBoxExcludeYoung.Checked = this.options.IgnoreYoung;
            checkBoxExcludeAdolescent.Checked = this.options.IgnoreAdolescent;

            checkBoxKeepComparingSelected.Checked = this.options.AgeIgnoreOnlyOtherCreatures;

            numericUpDownBadTraitWeight.Value = ((decimal)this.options.BadTraitWeight).ConstrainToRange(0, 100);
            checkBoxDiscardWithBadTraits.Checked = this.options.DiscardOnAnyNegativeTraits;

            checkBoxPreferUniqueTraits.Checked = this.options.PreferUniqueTraits;
            numericUpDownUniqueTraitWeight.Value = ((decimal)this.options.UniqueTraitWeight).ConstrainToRange(0, 100);

            numericUpDownInbreedPenaltyWeight.Value = ((decimal)this.options.InbreedingPenaltyWeight.ConstrainToRange(0, 100));
            checkBoxDiscardAllCausingInbreed.Checked = this.options.DiscardOnInbreeding;

            checkBoxExcludeExactAge.Checked = this.options.ExcludeExactAgeEnabled;
            UpdateCheckBoxEcludeExactAge();
            timeSpanInputExcludeExactAge.Value = this.options.ExcludeExactAgeValue;

            olvColumnColorWeight.AspectPutter +=
                ((rowObject, value) =>
                {
                    var colorWeight = (ColorWeight)rowObject;
                    try
                    {
                        var val = float.Parse(value.ToString());
                        colorWeight.Weight = val;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Could not insert new value: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Info(exception, "Attempted to insert invalid value to color weight");
                    }
                });

            objectListViewColorWeights.SetObjects(this.options.CreatureColorValues);

            UpdateCtrlsEnabledStates();

            Tip(checkBoxSkipPregnant, SkipPregnantCreaturesTooltip);
            Tip(checkBoxSkipGaveBirthInLast24h, SkipPregnantLast24HTooltip);
            Tip(numericUpDownBadTraitWeight, BadTraitWeightTooltip);
            Tip(checkBoxDiscardWithBadTraits, DiscardWithAnyBadTraitsTooltip);
            Tip(checkBoxPreferUniqueTraits, PreferUniqueTraitsTooltop);
            Tip(numericUpDownUniqueTraitWeight, UniqTraitWeightTooltip);
            Tip(numericUpDownInbreedPenaltyWeight, InbreedWeightTooltip);
            Tip(checkBoxSkipPaired, SkipPairedCreaturesTooltip);
        }

        private void UpdateCheckBoxEcludeExactAge()
        {
            timeSpanInputExcludeExactAge.Enabled = checkBoxExcludeExactAge.Checked;
        }

        void Tip(Control ctrl, string tip)
        {
            toolTip1.SetToolTip(ctrl, tip.WrapLines());
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            options.IgnoreNotInMood = checkBoxSkipNotInMood.Checked;
            options.IgnorePregnant = checkBoxSkipPregnant.Checked;
            options.IgnoreRecentlyPregnant = checkBoxSkipGaveBirthInLast24h.Checked;
            options.IgnoreOtherHerds = checkBoxSkipCreaturesInOtherHerds.Checked;
            options.IgnorePairedCreatures = checkBoxSkipPaired.Checked;

            options.IgnoreSold = checkBoxIgnoreSold.Checked;
            options.IgnoreDead = checkBoxIgnoreDead.Checked;

            options.IgnoreFoals = checkBoxExcludeFoals.Checked;
            options.IgnoreYoung = checkBoxExcludeYoung.Checked;
            options.IgnoreAdolescent = checkBoxExcludeAdolescent.Checked;

            options.AgeIgnoreOnlyOtherCreatures = checkBoxKeepComparingSelected.Checked;

            options.BadTraitWeight = (double)numericUpDownBadTraitWeight.Value;
            options.DiscardOnAnyNegativeTraits = checkBoxDiscardWithBadTraits.Checked;

            options.PreferUniqueTraits = checkBoxPreferUniqueTraits.Checked;
            options.UniqueTraitWeight = (double)numericUpDownUniqueTraitWeight.Value;

            options.InbreedingPenaltyWeight = (double)numericUpDownInbreedPenaltyWeight.Value;
            options.DiscardOnInbreeding = checkBoxDiscardAllCausingInbreed.Checked;

            options.ExcludeExactAgeEnabled = checkBoxExcludeExactAge.Checked;
            options.ExcludeExactAgeValue = timeSpanInputExcludeExactAge.Value;

            options.CreatureColorValues =
                ((IEnumerable<ColorWeight>)objectListViewColorWeights.Objects).ToArray();
        }

        void UpdateCtrlsEnabledStates()
        {
            numericUpDownBadTraitWeight.Enabled = !checkBoxDiscardWithBadTraits.Checked;
            numericUpDownUniqueTraitWeight.Enabled = checkBoxPreferUniqueTraits.Checked;
            numericUpDownInbreedPenaltyWeight.Enabled = !checkBoxDiscardAllCausingInbreed.Checked;
        }

        private void checkBoxDiscardWithBadTraits_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCtrlsEnabledStates();
        }

        private void checkBoxPreferUniqueTraits_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCtrlsEnabledStates();
        }

        private void checkBoxDiscardAllCausingInbreed_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCtrlsEnabledStates();
        }

        private void checkBoxSkipGaveBirthInLast24h_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSkipGaveBirthInLast24h.Checked)
                checkBoxSkipPregnant.Checked = true;
        }

        private void checkBoxSkipPregnant_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxSkipPregnant.Checked)
            {
                checkBoxSkipGaveBirthInLast24h.Checked = false;
            }
        }

        private void objectListViewColorWeights_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Column == olvColumnColorType)
            {
                var cweight = (ColorWeight)e.Model;
                var color = cweight.Color.SystemDrawingColor;
                e.SubItem.BackColor = color;
                e.SubItem.ForeColor = e.SubItem.BackColor.GetContrastingBlackOrWhiteColor();
            }
        }

        private void checkBoxExcludeExactAge_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxEcludeExactAge();
        }
    }
}
