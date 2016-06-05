using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.CreatureEdit;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.LogFeedManager;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Parts.HorseEdit
{
    public partial class FormCreatureViewEdit : ExtendedForm
    {
        Creature creature;
        GrangerContext Context;
        FormGrangerMain MainForm;

        CreatureViewEditOpType _opType;
        private string HerdID;
        readonly ILogger logger;
        readonly IWurmApi wurmApi;

        CreatureViewEditOpType OpMode
        {
            get { return _opType; }
            set
            {
                _opType = value;
                if (value == CreatureViewEditOpType.Edit)
                {
                    prepareFieldsForEdit();
                }
                else if (value == CreatureViewEditOpType.New)
                {
                    prepareFieldsForNew();
                }
                else if (value == CreatureViewEditOpType.View)
                {
                    prepareFieldsForView();
                }
                else throw new InvalidOperationException("bad op type");
            }
        }

        public FormCreatureViewEdit(FormGrangerMain mainForm, Creature creature, GrangerContext context,
            CreatureViewEditOpType optype, string herdId, [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.MainForm = mainForm;
            this.creature = creature;
            this.Context = context;
            this.HerdID = herdId;
            this.logger = logger;
            this.wurmApi = wurmApi;
            InitializeComponent();

            disableAllFields();

            comboBoxServerName.Items.AddRange(
                wurmApi.Servers.All.Select(server => server.ServerName.Original).Cast<object>().ToArray());

            List<string> list = new List<string>();
            list.AddRange(Context.Creatures.Select(x => x.Name));
            list.AddRange(Context.Creatures.Select(x => x.MotherName));
            list.AddRange(Context.Creatures.Select(x => x.FatherName));
            string[] allCreatureNamesInDatabase = list.Distinct().Where(x => x != null).ToArray();

            comboBoxFather.Items.AddRange(allCreatureNamesInDatabase);
            comboBoxMother.Items.AddRange(allCreatureNamesInDatabase);

            comboBoxColor.Items.AddRange(CreatureColor.GetColorsEnumStrArray());
            comboBoxColor.Text = CreatureColor.GetDefaultColorStr();
            comboBoxAge.Items.AddRange(CreatureAge.GetColorsEnumStrArray());
            comboBoxAge.Text = CreatureAge.GetDefaultAgeStr();

            this.OpMode = optype;
        }

        void disableAllFields()
        {
            panel1.Enabled = false;
            dateTimePickerBred.Value = DateTimePicker.MinimumDateTime;
            dateTimePickerGroomed.Value = DateTimePicker.MinimumDateTime;
            dateTimePickerPregnant.Value = DateTimePicker.MinimumDateTime;
            dateTimePickerBirthDate.Value = DateTimePicker.MinimumDateTime;
            buttonEdit.Visible = true;
        }

        void enableAllFields()
        {
            panel1.Enabled = true;
            buttonEdit.Visible = false;
        }

        void prepareFieldsForView()
        {
            buildTraits();

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

            this.Text = "Viewing creature: " + creature.NameAspect + " in herd: " + HerdID;
        }

        void prepareFieldsForEdit()
        {
            prepareFieldsForView();
            enableAllFields();
            this.Text = "Editing creature: " + creature.NameAspect + " in herd: " + HerdID;
            ValidateCreatureIdentity();
        }

        void buildTraits()
        {
            checkedListBoxTraits.Items.Clear();

            var traits = creature.Traits;
            var allTraits = CreatureTrait.GetAllTraitEnums();

            foreach (var trait in allTraits)
            {
                checkedListBoxTraits.Items.Add(
                    CreatureTrait.GetWurmTextForTrait(trait),
                    traits.Where(x => x.Trait == trait).Count() == 1);
            }
        }

        void prepareFieldsForNew()
        {
            checkedListBoxTraits.Items.Clear();
            checkedListBoxTraits.Items.AddRange(CreatureTrait.GetAllTraitWurmText());

            enableAllFields();
            this.Text = "Adding new creature to herd: " + HerdID;
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
                        var newEntity = new CreatureEntity() { Id = CreatureEntity.GenerateNewCreatureId(Context) };
                        creature = new Creature(MainForm, newEntity, Context);
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
                    creature.Color = CreatureColor.CreateColorFromEnumString(comboBoxColor.Text);
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
                        creature.Herd = HerdID;
                        Context.InsertCreature(creature.Entity);
                    }
                    else
                    {
                        Context.SubmitChanges();
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
            prepareFieldsForEdit();
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

            var otherCreatures = Context.Creatures.Where(x => x.Herd == HerdID).ToArray();

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

        bool _textBoxPasteUpdate_selfUpdating = false;
        CreatureTrait[] CurrentParsedTraits = null;
        private void textBoxPasteUpdate_TextChanged(object sender, EventArgs e)
        {
            if (!_textBoxPasteUpdate_selfUpdating)
            {
                _textBoxPasteUpdate_selfUpdating = true;
                CreatureTrait[] parsedTraits = GrangerHelpers.GetTraitsFromLine(textBoxPasteUpdate.Text);
                if (parsedTraits.Length == 0)
                {
                    textBoxPasteUpdate.Text = "> no traits found in text <";
                    buttonApplyTraitsFromPasteText.Enabled = false;
                    CurrentParsedTraits = null;
                }
                else
                {
                    CurrentParsedTraits = parsedTraits;
                    buttonApplyTraitsFromPasteText.Enabled = true;
                    textBoxPasteUpdate.Text = "Found traits: " + string.Join(", ", parsedTraits.Select(x => x.ToCompactString()));
                }

                _textBoxPasteUpdate_selfUpdating = false;
            }
        }

        private void buttonApplyTraitsFromPasteText_Click(object sender, EventArgs e)
        {
            if (CurrentParsedTraits != null)
            {
                var parsedTraitsToString = CurrentParsedTraits.Select(x => x.ToString()).ToArray();
                for (int i = 0; i < checkedListBoxTraits.Items.Count; i++)
                {
                    string currentTraitText = checkedListBoxTraits.Items[i].ToString();
                    if (parsedTraitsToString.Contains(currentTraitText))
                    {
                        checkedListBoxTraits.SetItemChecked(i, true);
                    }
                }
            }
            CurrentParsedTraits = null;
            buttonApplyTraitsFromPasteText.Enabled = false;
        }

        private void comboBoxServerName_Validating(object sender, CancelEventArgs e)
        {
            ValidateCreatureIdentity();
        }
    }
}
