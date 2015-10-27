using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.LogFeedManager;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.HorseEdit
{
    public partial class FormHorseViewEdit : ExtendedForm
    {
        Horse horse;
        GrangerContext Context;
        FormGrangerMain MainForm;

        HorseViewEditOpType _opType;
        private string HerdID;
        readonly ILogger logger;

        HorseViewEditOpType OpMode
        {
            get { return _opType; }
            set
            {
                _opType = value;
                if (value == HorseViewEditOpType.Edit)
                {
                    prepareFieldsForEdit();
                }
                else if (value == HorseViewEditOpType.New)
                {
                    prepareFieldsForNew();
                }
                else if (value == HorseViewEditOpType.View)
                {
                    prepareFieldsForView();
                }
                else throw new InvalidOperationException("bad op type");
            }
        }

        public FormHorseViewEdit(FormGrangerMain mainForm, Horse horse, GrangerContext context, HorseViewEditOpType optype, string herdID,
            [NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.MainForm = mainForm;
            this.horse = horse;
            this.Context = context;
            this.HerdID = herdID;
            this.logger = logger;
            InitializeComponent();

            disableAllFields();

            List<string> list = new List<string>();
            list.AddRange(Context.Horses.Select(x => x.Name));
            list.AddRange(Context.Horses.Select(x => x.MotherName));
            list.AddRange(Context.Horses.Select(x => x.FatherName));
            string[] allHorseNamesInDatabase = list.Distinct().Where(x => x != null).ToArray();

            comboBoxFather.Items.AddRange(allHorseNamesInDatabase);
            comboBoxMother.Items.AddRange(allHorseNamesInDatabase);

            comboBoxColor.Items.AddRange(HorseColor.GetColorsEnumStrArray());
            comboBoxColor.Text = HorseColor.GetDefaultColorStr();
            comboBoxAge.Items.AddRange(HorseAge.GetColorsEnumStrArray());
            comboBoxAge.Text = HorseAge.GetDefaultAgeStr();

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

            textBoxName.Text = horse.NameAspect;
            comboBoxMother.Text = horse.MotherAspect;
            comboBoxFather.Text = horse.FatherAspect;
            if (horse.IsMale) radioButtonMale.Checked = true; else radioButtonFemale.Checked = true;
            try { dateTimePickerBred.Value = horse.NotInMoodUntil; }
            catch { dateTimePickerBred.Value = DateTimePicker.MinimumDateTime; }
            try { dateTimePickerGroomed.Value = horse.GroomedOn; }
            catch { dateTimePickerGroomed.Value = DateTimePicker.MinimumDateTime; }
            try { dateTimePickerPregnant.Value = horse.PregnantUntil; }
            catch { dateTimePickerPregnant.Value = DateTimePicker.MinimumDateTime; }
            try { dateTimePickerBirthDate.Value = horse.BirthDate; }
            catch { dateTimePickerBirthDate.Value = DateTimePicker.MinimumDateTime; }
            checkBoxDead.Checked = horse.CheckTag("dead");
            checkBoxDiseased.Checked = horse.CheckTag("diseased");
            checkBoxSold.Checked = horse.CheckTag("sold");
            numericUpDownAHskill.Value = (decimal)(horse.TraitsInspectSkill.ConstrainToRange(0F, 100F));
            checkBoxEpic.Checked = horse.EpicCurve;
            textBoxBrandedFor.Text = horse.BrandedForAspect;
            textBoxCaredForBy.Text = horse.TakenCareOfByAspect;
            textBoxComment.Text = horse.CommentsAspect;
            comboBoxColor.Text = horse.Color.HorseColorId.ToString();
            comboBoxAge.Text = horse.Age.HorseAgeId.ToString();
            Horse mate = horse.GetMate();
            if (mate != null) textBoxMate.Text = mate.ToString();

            this.Text = "Viewing creature: " + horse.NameAspect + " in herd: " + HerdID;
        }

        void prepareFieldsForEdit()
        {
            prepareFieldsForView();
            enableAllFields();
            this.Text = "Editing creature: " + horse.NameAspect + " in herd: " + HerdID;
            ValidateHorseIdentity();
        }

        void buildTraits()
        {
            checkedListBoxTraits.Items.Clear();

            var traits = horse.Traits;
            var allTraits = HorseTrait.GetAllTraitEnums();

            foreach (var trait in allTraits)
            {
                checkedListBoxTraits.Items.Add(
                    HorseTrait.GetWurmTextForTrait(trait),
                    traits.Where(x => x.Trait == trait).Count() == 1);
            }
        }

        void prepareFieldsForNew()
        {
            checkedListBoxTraits.Items.Clear();
            checkedListBoxTraits.Items.AddRange(HorseTrait.GetAllTraitWurmText());

            enableAllFields();
            this.Text = "Adding new creature to herd: " + HerdID;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!ValidateHorseIdentity())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
            }
            else
            {
                try
                {
                    if (OpMode == HorseViewEditOpType.New)
                    {
                        var newEntity = new HorseEntity() { ID = HorseEntity.GenerateNewHorseID(Context) };
                        horse = new Horse(MainForm, newEntity, Context);
                    }

                    List<HorseTrait> traitlist = new List<HorseTrait>();
                    foreach (var item in checkedListBoxTraits.CheckedItems)
                    {
                        try
                        {
                            traitlist.Add(HorseTrait.FromWurmTextRepr(item.ToString()));
                        }
                        catch (Exception _e)
                        {
                            logger.Error(_e, "failed to create creature trait from text:" + item.ToString());
                        }
                    }
                    var traitlistArray = traitlist.ToArray();

                    horse.Name = textBoxName.Text;
                    horse.Father = comboBoxFather.Text;
                    horse.Mother = comboBoxMother.Text;
                    horse.TakenCareOfBy = textBoxCaredForBy.Text;
                    horse.BrandedFor = textBoxBrandedFor.Text;

                    horse.Traits = traitlistArray;

                    horse.TraitsInspectSkill = (float)numericUpDownAHskill.Value;
                    float traitSkill = HorseTrait.GetMinSkillForTraits(traitlistArray, checkBoxEpic.Checked);
                    if (horse.TraitsInspectSkill < traitSkill)
                        horse.TraitsInspectSkill = traitSkill;
                    horse.EpicCurve = checkBoxEpic.Checked;

                    horse.Comments = textBoxComment.Text;
                    horse.IsMale = radioButtonMale.Checked;
                    horse.Color = HorseColor.CreateColorFromEnumString(comboBoxColor.Text);
                    horse.Age = HorseAge.CreateAgeFromEnumString(comboBoxAge.Text);

                    horse.NotInMoodUntil = dateTimePickerBred.Value;
                    horse.GroomedOn = dateTimePickerGroomed.Value;
                    horse.PregnantUntil = dateTimePickerPregnant.Value;
                    horse.BirthDate = dateTimePickerBirthDate.Value;

                    horse.SetTag("diseased", checkBoxDiseased.Checked);
                    horse.SetTag("dead", checkBoxDead.Checked);
                    horse.SetTag("sold", checkBoxSold.Checked);

                    if (OpMode == HorseViewEditOpType.New)
                    {
                        horse.Herd = HerdID;
                        Context.InsertHorse(horse.Entity);
                    }
                    else
                    {
                        Context.SubmitChangesToHorses();
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
            OpMode = HorseViewEditOpType.Edit;
            prepareFieldsForEdit();
        }

        private void textBoxName_Validating(object sender, CancelEventArgs e)
        {
            ValidateHorseIdentity();
        }

        private bool IsMale
        {
            get
            {
                if (radioButtonFemale.Checked) return false;
                else return true;
            }
        }

        private bool ValidateHorseIdentity()
        {
            if (this.OpMode == HorseViewEditOpType.View) return false;

            labelWarn.Visible = false;

            if (textBoxName.Text == "")
            {
                labelWarn.Visible = true;
                labelWarn.Text = "Creature name can't be empty";
                buttonOK.Enabled = false;
                return false;
            }

            var otherHorses = Context.Horses.Where(x => x.Herd == HerdID).ToArray();
            var nonuniques = otherHorses.Where(x => x.Name == textBoxName.Text).ToArray();

            if (OpMode == HorseViewEditOpType.Edit)
                nonuniques = nonuniques.Where(x => x != horse.Entity).ToArray();

            if (nonuniques.Length > 0)
            {
                labelWarn.Visible = true;
                labelWarn.Text = "Creature with this identity already exists in this herd";
                buttonOK.Enabled = false;
                return false;
            }

            labelWarn.Visible = false;
            buttonOK.Enabled = true;
            return true;
        }

        private string RefactorHorseName(string input)
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
            ValidateHorseIdentity();
        }

        private void comboBoxFather_Validating(object sender, CancelEventArgs e)
        {
            ValidateHorseIdentity();
        }

        private void radioButtonMale_CheckedChanged(object sender, EventArgs e)
        {
            ValidateHorseIdentity();
        }

        private void radioButtonFemale_CheckedChanged(object sender, EventArgs e)
        {
            ValidateHorseIdentity();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        private void FormHorseViewEdit_Load(object sender, EventArgs e)
        {
            //TODO can't use this, top bar can appear above upper edge of screen, needs tweaking
            //this.Location = FormHelper.GetCenteredChildPositionRelativeToParentWorkAreaBound(this, MainForm);
        }

        bool _textBoxPasteUpdate_selfUpdating = false;
        HorseTrait[] CurrentParsedTraits = null;
        private void textBoxPasteUpdate_TextChanged(object sender, EventArgs e)
        {
            if (!_textBoxPasteUpdate_selfUpdating)
            {
                _textBoxPasteUpdate_selfUpdating = true;
                HorseTrait[] parsedTraits = GrangerHelpers.GetTraitsFromLine(textBoxPasteUpdate.Text);
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
    }
}
