using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.CreatureEdit
{
    public partial class FormCreatureViewEdit : ExtendedForm
    {

        readonly GrangerContext context;
        readonly FormGrangerMain mainForm;
        readonly string herdId;
        readonly CreatureColorDefinitions creatureColorDefinitions;
        readonly ILogger logger;
        readonly IWurmApi wurmApi;

        Creature creature;
        CreatureViewEditOpType _opType;

        bool textBoxPasteUpdateSelfUpdating = false;
        CreatureTrait[] currentParsedTraits = null;

        CreatureViewEditOpType OpMode
        {
            get { return _opType; }
            set
            {
                _opType = value;
                if (value == CreatureViewEditOpType.Edit)
                {
                    PrepareFieldsForEdit();
                }
                else if (value == CreatureViewEditOpType.New)
                {
                    PrepareFieldsForNew();
                }
                else if (value == CreatureViewEditOpType.View)
                {
                    PrepareFieldsForView();
                }
                else throw new InvalidOperationException("bad op type");
            }
        }

        public FormCreatureViewEdit(
            [NotNull] FormGrangerMain mainForm,
            [NotNull] GrangerContext context,
            [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi, 
            [CanBeNull] Creature creature,
            CreatureViewEditOpType optype,
            string herdId,
            [NotNull] CreatureColorDefinitions creatureColorDefinitions)
        {
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (creatureColorDefinitions == null) throw new ArgumentNullException(nameof(creatureColorDefinitions));
            this.mainForm = mainForm;
            this.creature = creature;
            this.context = context;
            this.herdId = herdId;
            this.creatureColorDefinitions = creatureColorDefinitions;
            this.logger = logger;
            this.wurmApi = wurmApi;
            InitializeComponent();

            DisableAllFields();

            comboBoxServerName.Items.AddRange(
                wurmApi.Servers.All.Select(server => server.ServerName.Original).Cast<object>().ToArray());

            List<string> list = new List<string>();
            list.AddRange(this.context.Creatures.Select(x => x.Name));
            list.AddRange(this.context.Creatures.Select(x => x.MotherName));
            list.AddRange(this.context.Creatures.Select(x => x.FatherName));

            var allCreatureNamesInDatabase = list.Distinct().Where(x => x != null).Cast<object>().ToArray();

            comboBoxFather.Items.AddRange(allCreatureNamesInDatabase);
            comboBoxMother.Items.AddRange(allCreatureNamesInDatabase);
            ;
            comboBoxColor.Items.AddRange(
                creatureColorDefinitions.GetColors().Select(color => color.CreatureColorId).Cast<object>().ToArray());
            comboBoxColor.Text = CreatureColor.GetDefaultColor().CreatureColorId;
            comboBoxAge.Items.AddRange(CreatureAge.GetColorsEnumStrArray().Cast<object>().ToArray());
            comboBoxAge.Text = CreatureAge.GetDefaultAgeStr();

            this.OpMode = optype;
        }

        void DisableAllFields()
        {
            panel1.Enabled = false;
            dateTimePickerBred.Value = DateTimePicker.MinimumDateTime;
            dateTimePickerGroomed.Value = DateTimePicker.MinimumDateTime;
            dateTimePickerPregnant.Value = DateTimePicker.MinimumDateTime;
            dateTimePickerBirthDate.Value = DateTimePicker.MinimumDateTime;
            buttonEdit.Visible = true;
        }

        void EnableAllFields()
        {
            panel1.Enabled = true;
            buttonEdit.Visible = false;
        }

        void PrepareFieldsForView()
        {
            BuildTraits();

            textBoxName.Text = creature.NameAspect;
            comboBoxMother.Text = creature.MotherAspect;
            comboBoxFather.Text = creature.FatherAspect;
            if (creature.IsMale) radioButtonMale.Checked = true; else radioButtonFemale.Checked = true;
            try { dateTimePickerBred.Value = creature.NotInMoodUntil; }
            catch { dateTimePickerBred.Value = DateTimePicker.MinimumDateTime; }
            try { dateTimePickerGroomed.Value = creature.GroomedOn; }
            catch { dateTimePickerGroomed.Value = DateTimePicker.MinimumDateTime; }
            try { dateTimePickerPregnant.Value = creature.PregnantUntil; }
            catch { dateTimePickerPregnant.Value = DateTimePicker.MinimumDateTime; }
            try { dateTimePickerBirthDate.Value = creature.BirthDate; }
            catch { dateTimePickerBirthDate.Value = DateTimePicker.MinimumDateTime; }
            checkBoxDead.Checked = creature.CheckTag("dead");
            checkBoxDiseased.Checked = creature.CheckTag("diseased");
            checkBoxSold.Checked = creature.CheckTag("sold");
            numericUpDownAHskill.Value = (decimal)(creature.TraitsInspectSkill.ConstrainToRange(0F, 100F));
            checkBoxEpic.Checked = creature.EpicCurve;
            textBoxBrandedFor.Text = creature.BrandedForAspect;
            textBoxCaredForBy.Text = creature.TakenCareOfByAspect;
            textBoxComment.Text = creature.CommentsAspect;
            comboBoxColor.Text = creature.Color.CreatureColorId.ToString();
            comboBoxAge.Text = creature.Age.CreatureAgeId.ToString();
            comboBoxServerName.Text = creature.ServerName ?? string.Empty;
            Creature mate = creature.GetMate();
            if (mate != null) textBoxMate.Text = mate.ToString();

            this.Text = "Viewing creature: " + creature.NameAspect + " in herd: " + herdId;
        }

        void PrepareFieldsForEdit()
        {
            PrepareFieldsForView();
            EnableAllFields();
            this.Text = "Editing creature: " + creature.NameAspect + " in herd: " + herdId;
            ValidateCreatureIdentity();
        }

        void BuildTraits()
        {
            checkedListBoxTraits.Items.Clear();

            var traits = creature.Traits;
            var allTraits = CreatureTrait.GetAllTraitEnums();

            foreach (var trait in allTraits)
            {
                checkedListBoxTraits.Items.Add(
                    CreatureTrait.GetWurmTextForTrait(trait),
                    traits.Count(x => x.CreatureTraitId == trait) == 1);
            }
        }

        void PrepareFieldsForNew()
        {
            checkedListBoxTraits.Items.Clear();
            checkedListBoxTraits.Items.AddRange(CreatureTrait.GetAllTraitWurmText().Cast<object>().ToArray());

            EnableAllFields();
            this.Text = "Adding new creature to herd: " + herdId;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!ValidateCreatureIdentity())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
            }
            else
            {
                try
                {
                    if (OpMode == CreatureViewEditOpType.New)
                    {
                        var newEntity = new CreatureEntity() { Id = CreatureEntity.GenerateNewCreatureId(context) };
                        creature = new Creature(mainForm, newEntity, context, creatureColorDefinitions);
                    }

                    List<CreatureTrait> traitlist = new List<CreatureTrait>();
                    foreach (var item in checkedListBoxTraits.CheckedItems)
                    {
                        try
                        {
                            traitlist.Add(CreatureTrait.FromWurmTextRepr(item.ToString()));
                        }
                        catch (Exception _e)
                        {
                            logger.Error(_e, "failed to create creature trait from text:" + item.ToString());
                        }
                    }
                    var traitlistArray = traitlist.ToArray();

                    creature.Name = textBoxName.Text.Trim();
                    creature.Father = comboBoxFather.Text.Trim();
                    creature.Mother = comboBoxMother.Text.Trim();
                    creature.TakenCareOfBy = textBoxCaredForBy.Text.Trim();
                    creature.BrandedFor = textBoxBrandedFor.Text.Trim();

                    creature.Traits = traitlistArray;

                    creature.TraitsInspectSkill = (float)numericUpDownAHskill.Value;
                    float traitSkill = CreatureTrait.GetMinSkillForTraits(traitlistArray, checkBoxEpic.Checked);
                    if (creature.TraitsInspectSkill < traitSkill)
                        creature.TraitsInspectSkill = traitSkill;
                    creature.EpicCurve = checkBoxEpic.Checked;

                    creature.Comments = textBoxComment.Text;
                    creature.IsMale = radioButtonMale.Checked;
                    creature.Color = creatureColorDefinitions.GetForId(comboBoxColor.Text);
                    creature.Age = CreatureAge.CreateAgeFromEnumString(comboBoxAge.Text);

                    creature.NotInMoodUntil = dateTimePickerBred.Value;
                    creature.GroomedOn = dateTimePickerGroomed.Value;
                    creature.PregnantUntil = dateTimePickerPregnant.Value;
                    creature.BirthDate = dateTimePickerBirthDate.Value;

                    creature.SetTag("diseased", checkBoxDiseased.Checked);
                    creature.SetTag("dead", checkBoxDead.Checked);
                    creature.SetTag("sold", checkBoxSold.Checked);

                    creature.ServerName = comboBoxServerName.Text.Trim();

                    if (OpMode == CreatureViewEditOpType.New)
                    {
                        creature.Herd = herdId;
                        context.InsertCreature(creature.Entity);
                    }
                    else
                    {
                        context.SubmitChanges();
                    }
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                catch (Exception _e)
                {
                    logger.Error(_e, "problem while updating database, op: " + OpMode);
                    MessageBox.Show("There was a problem on submitting to database.\r\n" + _e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = System.Windows.Forms.DialogResult.None;
                }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            OpMode = CreatureViewEditOpType.Edit;
            PrepareFieldsForEdit();
        }

        private void textBoxName_Validating(object sender, CancelEventArgs e)
        {
            ValidateCreatureIdentity();
        }

        private bool IsMale
        {
            get
            {
                if (radioButtonFemale.Checked) return false;
                else return true;
            }
        }

        private bool ValidateCreatureIdentity()
        {
            if (this.OpMode == CreatureViewEditOpType.View) return false;

            labelWarn.Visible = false;

            if (textBoxName.Text == "")
            {
                labelWarn.Visible = true;
                labelWarn.Text = "Creature name can't be empty";
                buttonOK.Enabled = false;
                return false;
            }

            var otherCreatures = context.Creatures.Where(x => x.Herd == herdId).ToArray();

            var nonuniques = otherCreatures.Where(x =>
                x.Name == textBoxName.Text.Trim()).ToArray();

            if (OpMode == CreatureViewEditOpType.Edit)
                nonuniques = nonuniques.Where(x => x != creature.Entity).ToArray();

            if (nonuniques.Length > 0)
            {
                labelWarn.Visible = true;
                labelWarn.Text = "Creature with this name already exists in this herd";
                buttonOK.Enabled = false;
                return false;
            }

            labelWarn.Visible = false;
            buttonOK.Enabled = true;
            return true;
        }

        private string RefactorCreatureName(string input)
        {
            input = GrangerHelpers.RemoveAllPrefixes(input);
            input = input.Trim();
            input = input.ToLowerInvariant();
            string concatworker = "";
            if (input.Length > 0) concatworker = input[0].ToString().ToUpper();
            if (input.Length > 1) concatworker += input.Substring(1);
            return concatworker;
        }

        private void comboBoxMother_Validating(object sender, CancelEventArgs e)
        {
            ValidateCreatureIdentity();
        }

        private void comboBoxFather_Validating(object sender, CancelEventArgs e)
        {
            ValidateCreatureIdentity();
        }

        private void radioButtonMale_CheckedChanged(object sender, EventArgs e)
        {
            ValidateCreatureIdentity();
        }

        private void radioButtonFemale_CheckedChanged(object sender, EventArgs e)
        {
            ValidateCreatureIdentity();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
        }

        private void FormCreatureViewEdit_Load(object sender, EventArgs e)
        {
        }


        private void textBoxPasteUpdate_TextChanged(object sender, EventArgs e)
        {
            if (!textBoxPasteUpdateSelfUpdating)
            {
                textBoxPasteUpdateSelfUpdating = true;
                CreatureTrait[] parsedTraits = GrangerHelpers.ParseTraitsFromLine(textBoxPasteUpdate.Text);
                if (parsedTraits.Length == 0)
                {
                    textBoxPasteUpdate.Text = "> no traits found in text <";
                    buttonApplyTraitsFromPasteText.Enabled = false;
                    currentParsedTraits = null;
                }
                else
                {
                    currentParsedTraits = parsedTraits;
                    buttonApplyTraitsFromPasteText.Enabled = true;
                    textBoxPasteUpdate.Text = "Found traits: " + string.Join(", ", parsedTraits.Select(x => x.ToCompactString()));
                }

                textBoxPasteUpdateSelfUpdating = false;
            }
        }

        private void buttonApplyTraitsFromPasteText_Click(object sender, EventArgs e)
        {
            if (currentParsedTraits != null)
            {
                var parsedTraitsToString = currentParsedTraits.Select(x => x.ToString()).ToArray();
                for (int i = 0; i < checkedListBoxTraits.Items.Count; i++)
                {
                    string currentTraitText = checkedListBoxTraits.Items[i].ToString();
                    if (parsedTraitsToString.Contains(currentTraitText))
                    {
                        checkedListBoxTraits.SetItemChecked(i, true);
                    }
                }
            }
            currentParsedTraits = null;
            buttonApplyTraitsFromPasteText.Enabled = false;
        }

        private void comboBoxServerName_Validating(object sender, CancelEventArgs e)
        {
            ValidateCreatureIdentity();
        }
    }
}
