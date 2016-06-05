using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmAssistant3.Areas.Granger.Singletons;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Parts.Advisor.Default
{
    public partial class BreedingEvaluatorDefaultConfig : ExtendedForm
    {
        private DefaultBreedingEvaluatorOptions OptionsRef;
        readonly ILogger logger;

        const string SKIP_PREGNANT_CREATURES_TOOLTIP = "Pregnancy is hard to track accurately, the error can be up to a day, Assistant always assumes longest time. Creature needs to be smilexamined after breeding, else Assistant will asume longest possible pregnancy.";
        const string SKIP_PREGNANT_LAST_24H_TOOLTIP = "This adds 24h to last creature pregnancy";
        const string BAD_TRAIT_WEIGHT_TOOLTIP = "Bad trait value will be multiplied by this weight";
        const string DISCARD_WITH_ANY_BAD_TRAITS_TOOLTIP = "Skips a creature that has one or more bad traits. This does not include potential bad traits. The selected creature is not discarded, you can still click one with bad traits and get breeding suggestions against any non-bad traited creatures.";
        const string INCLUDE_POTENTIAL_VALUE_TOOLTIP = "This will asume that creature has ALL traits that were not visible upon last inspection (based on inspector animal husbandry skill at that time). It is far from very useful and it is suggested to have skilled player inspect creature for you and then update it manually.";
        const string POTENTIAL_GOOD_WEIGHT_TOOLTIP = "Each potential good trait will be multiplied by this weight";
        const string POTENTIAL_BAD_WEIGHT_TOOLTIP = "Each potential bad trait will be multiplied by this weight";
        const string PREFER_UNIQUE_TRAITS_TOOLTOP = "This will adjust good trait value, if it's present only on one creature. This option is useful for improving your chances to get a creature with more good traits, at expense of getting overall worse creatures";
        const string UNIQ_TRAIT_WEIGHT_TOOLTIP = "Unique good trait value will be multiplied by this value";
        const string INBREED_WEIGHT_TOOLTIP = "Fictional bad trait will be multiplied by this value. Check assistant wiki for a current list of which traits are considered for inbreeding";
        const string SKIP_PAIRED_CREATURES_TOOLTIP = "If you have created any creature pairs, they will be completely excluded from breeding results";
        const string AGE_KEEP_COMPARING_OTHER_TOOLTIP = "Normally age excluded creatures will not be compared at all. This will let you still see comparisons for the selected underage creature.";

        public BreedingEvaluatorDefaultConfig(DefaultBreedingEvaluatorOptions options, [NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            OptionsRef = options;
            this.logger = logger;
            InitializeComponent();

            checkBoxSkipNotInMood.Checked = OptionsRef.IgnoreNotInMood;
            checkBoxSkipPregnant.Checked = OptionsRef.IgnorePregnant;
            checkBoxSkipGaveBirthInLast24h.Checked = OptionsRef.IgnoreRecentlyPregnant;
            checkBoxSkipCreaturesInOtherHerds.Checked = OptionsRef.IgnoreOtherHerds;
            checkBoxSkipPaired.Checked = OptionsRef.IgnorePairedCreatures;

            checkBoxIgnoreSold.Checked = OptionsRef.IgnoreSold;
            checkBoxIgnoreDead.Checked = OptionsRef.IgnoreDead;

            checkBoxExcludeFoals.Checked = OptionsRef.IgnoreFoals;
            checkBoxExcludeYoung.Checked = OptionsRef.IgnoreYoung;
            checkBoxExcludeAdolescent.Checked = OptionsRef.IgnoreAdolescent;

            checkBoxKeepComparingSelected.Checked = OptionsRef.AgeIgnoreOnlyOtherCreatures;

            numericUpDownBadTraitWeight.Value = ((decimal)OptionsRef.BadTraitWeight).ConstrainToRange(0, 100);
            checkBoxDiscardWithBadTraits.Checked = OptionsRef.DiscardOnAnyNegativeTraits;
            checkBoxIncludePotentialValue.Checked = OptionsRef.IncludePotentialValue;
            numericUpDownPotValGoodWeight.Value = ((decimal)OptionsRef.PotentialValuePositiveWeight).ConstrainToRange(0, 100);
            numericUpDownPotValBadWeight.Value = ((decimal)OptionsRef.PotentialValueNegativeWeight).ConstrainToRange(0, 100);

            checkBoxPreferUniqueTraits.Checked = OptionsRef.PreferUniqueTraits;
            numericUpDownUniqueTraitWeight.Value = ((decimal)OptionsRef.UniqueTraitWeight).ConstrainToRange(0, 100);

            numericUpDownInbreedPenaltyWeight.Value = ((decimal)OptionsRef.InbreedingPenaltyWeight.ConstrainToRange(0, 100));
            checkBoxDiscardAllCausingInbreed.Checked = OptionsRef.DiscardOnInbreeding;

            checkBoxExcludeExactAge.Checked = OptionsRef.ExcludeExactAgeEnabled;
            UpdateCheckBoxEcludeExactAge();
            timeSpanInputExcludeExactAge.Value = OptionsRef.ExcludeExactAgeValue;

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

            objectListViewColorWeights.SetObjects(OptionsRef.CreatureColorValues);

            UpdateCtrlsEnabledStates();

            Tip(checkBoxSkipPregnant, SKIP_PREGNANT_CREATURES_TOOLTIP);
            Tip(checkBoxSkipGaveBirthInLast24h, SKIP_PREGNANT_LAST_24H_TOOLTIP);
            Tip(numericUpDownBadTraitWeight, BAD_TRAIT_WEIGHT_TOOLTIP);
            Tip(checkBoxDiscardWithBadTraits, DISCARD_WITH_ANY_BAD_TRAITS_TOOLTIP);
            Tip(checkBoxIncludePotentialValue, INCLUDE_POTENTIAL_VALUE_TOOLTIP);
            Tip(numericUpDownPotValGoodWeight, POTENTIAL_GOOD_WEIGHT_TOOLTIP);
            Tip(numericUpDownPotValBadWeight, POTENTIAL_BAD_WEIGHT_TOOLTIP);
            Tip(checkBoxPreferUniqueTraits, PREFER_UNIQUE_TRAITS_TOOLTOP);
            Tip(numericUpDownUniqueTraitWeight, UNIQ_TRAIT_WEIGHT_TOOLTIP);
            Tip(numericUpDownInbreedPenaltyWeight, INBREED_WEIGHT_TOOLTIP);
            Tip(checkBoxSkipPaired, SKIP_PAIRED_CREATURES_TOOLTIP);
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
            OptionsRef.IgnoreNotInMood = checkBoxSkipNotInMood.Checked;
            OptionsRef.IgnorePregnant = checkBoxSkipPregnant.Checked;
            OptionsRef.IgnoreRecentlyPregnant = checkBoxSkipGaveBirthInLast24h.Checked;
            OptionsRef.IgnoreOtherHerds = checkBoxSkipCreaturesInOtherHerds.Checked;
            OptionsRef.IgnorePairedCreatures = checkBoxSkipPaired.Checked;

            OptionsRef.IgnoreSold = checkBoxIgnoreSold.Checked;
            OptionsRef.IgnoreDead = checkBoxIgnoreDead.Checked;

            OptionsRef.IgnoreFoals = checkBoxExcludeFoals.Checked;
            OptionsRef.IgnoreYoung = checkBoxExcludeYoung.Checked;
            OptionsRef.IgnoreAdolescent = checkBoxExcludeAdolescent.Checked;

            OptionsRef.AgeIgnoreOnlyOtherCreatures = checkBoxKeepComparingSelected.Checked;

            OptionsRef.BadTraitWeight = (double)numericUpDownBadTraitWeight.Value;
            OptionsRef.DiscardOnAnyNegativeTraits = checkBoxDiscardWithBadTraits.Checked;
            OptionsRef.IncludePotentialValue = checkBoxIncludePotentialValue.Checked;
            OptionsRef.PotentialValuePositiveWeight = (double)numericUpDownPotValGoodWeight.Value;
            OptionsRef.PotentialValueNegativeWeight = (double)numericUpDownPotValBadWeight.Value;

            OptionsRef.PreferUniqueTraits = checkBoxPreferUniqueTraits.Checked;
            OptionsRef.UniqueTraitWeight = (double)numericUpDownUniqueTraitWeight.Value;

            OptionsRef.InbreedingPenaltyWeight = (double)numericUpDownInbreedPenaltyWeight.Value;
            OptionsRef.DiscardOnInbreeding = checkBoxDiscardAllCausingInbreed.Checked;

            OptionsRef.ExcludeExactAgeEnabled = checkBoxExcludeExactAge.Checked;
            OptionsRef.ExcludeExactAgeValue = timeSpanInputExcludeExactAge.Value;

            OptionsRef.CreatureColorValues =
                ((IEnumerable<ColorWeight>)objectListViewColorWeights.Objects).ToArray();
        }

        void UpdateCtrlsEnabledStates()
        {
            numericUpDownBadTraitWeight.Enabled = !checkBoxDiscardWithBadTraits.Checked;
            numericUpDownPotValBadWeight.Enabled = checkBoxIncludePotentialValue.Checked;
            numericUpDownPotValGoodWeight.Enabled = checkBoxIncludePotentialValue.Checked;
            numericUpDownUniqueTraitWeight.Enabled = checkBoxPreferUniqueTraits.Checked;
            numericUpDownInbreedPenaltyWeight.Enabled = !checkBoxDiscardAllCausingInbreed.Checked;
        }

        private void checkBoxDiscardWithBadTraits_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCtrlsEnabledStates();
            //numericUpDownBadTraitWeight.Enabled = !checkBoxDiscardWithBadTraits.Checked;
        }

        private void checkBoxIncludePotentialValue_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCtrlsEnabledStates();
            //numericUpDownPotValBadWeight.Enabled = checkBoxIncludePotentialValue.Checked;
            //numericUpDownPotValGoodWeight.Enabled = checkBoxIncludePotentialValue.Checked;
        }

        private void checkBoxPreferUniqueTraits_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCtrlsEnabledStates();
            //numericUpDownUniqueTraitWeight.Enabled = checkBoxPreferUniqueTraits.Checked;
        }

        private void checkBoxDiscardAllCausingInbreed_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCtrlsEnabledStates();
            //numericUpDownInbreedPenaltyWeight.Enabled = checkBoxDiscardAllCausingInbreed.Checked;
        }

        private void checkBoxSkipPaired_CheckedChanged(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Still not written after 3 years! Lazy Aldur!");
        }

        private void checkBoxExcludeFoals_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBoxExcludeYoung_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBoxExcludeAdolescent_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {
        }

        private void checkBoxKeepComparingSelected_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void objectListViewColorWeights_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Column == olvColumnColorType)
            {
                var cweight = (ColorWeight)e.Model;
                var color = cweight.Color.ToSystemDrawingColor();
                if (color != null)
                {
                    e.SubItem.BackColor = color.Value;
                    e.SubItem.ForeColor = e.SubItem.BackColor.GetContrastingBlackOrWhiteColor();
                }
            }
        }

        private void checkBoxExcludeExactAge_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxEcludeExactAge();
        }
    }
}
