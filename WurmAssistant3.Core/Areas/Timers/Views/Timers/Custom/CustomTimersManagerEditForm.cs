using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Custom;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Custom
{
    public partial class CustomTimersManagerEditForm : ExtendedForm
    {
        string EditingNameID = null;
        Form parentForm;
        readonly TimerDefinitions timerDefinitions;

        public CustomTimersManagerEditForm(Form parent, IWurmApi wurmApi,
            [NotNull] TimerDefinitions timerDefinitions)
        {
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            this.parentForm = parent;
            this.timerDefinitions = timerDefinitions;
            InitializeComponent();
            foreach (var type in  wurmApi.LogDefinitions.AllLogTypes)
                comboBoxLogType.Items.Add(type);
            comboBoxLogType.SelectedItem = LogType.Event;
        }

        public CustomTimersManagerEditForm(Form parent, IWurmApi wurmApi, TimerDefinitions timerDefinitions, string nameID)
            : this(parent, wurmApi, timerDefinitions)
        {

            EditingNameID = nameID;
            CustomTimerOptionsTemplate options = timerDefinitions.GetOptionsTemplateForCustomTimer(nameID);
            textBoxNameID.Text = nameID;
            textBoxNameID.Enabled = false;
            if (options.TriggerConditions != null)
            {
                textBoxCond.Text = options.TriggerConditions[0].RegexPattern;
                if (!options.IsRegex)
                {
                    textBoxCond.Text = Regex.Unescape(textBoxCond.Text);
                }
                else checkBoxAsRegex.Checked = true;
                comboBoxLogType.SelectedItem = options.TriggerConditions[0].LogType;
            }
            if (options.Duration != null) timeInputUControl2.Value = options.Duration;
            checkBoxUptimeReset.Checked = options.ResetOnUptime;
        }

        private void CustomTimersManagerEditWindow_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(textBoxCond, "if not used as Regex, timer will start if this text is found in chosen log");
            toolTip1.SetToolTip(checkBoxAsRegex, "tip: use Log Searcher to test your Regex patterns.\r\nRegex pattern is raw and thus CASE-SENSITIVE (same as in Log Searcher)");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //validate
            if (IsValidData())
            {
                CustomTimerOptionsTemplate options = new CustomTimerOptionsTemplate();
                options.AddTrigger(textBoxCond.Text, (LogType)comboBoxLogType.SelectedItem, checkBoxAsRegex.Checked);
                options.Duration = timeInputUControl2.Value;
                options.ResetOnUptime = checkBoxUptimeReset.Checked;
                if (EditingNameID != null)
                {
                    timerDefinitions.RemoveCustomTimerDefinition(EditingNameID);
                }
                timerDefinitions.AddCustomTimerDefinition(textBoxNameID.Text, options);
                this.Close();
            }
        }

        bool IsValidData()
        {
            bool valid = true;
            if (textBoxNameID.Text.Trim() == string.Empty)
            {
                valid = false;
                MessageBox.Show("Timer name cannot be empty");
            }
            else if (EditingNameID == null && !timerDefinitions.IsNameUnique(textBoxNameID.Text))
            {
                valid = false;
                MessageBox.Show("Timer with this name already exists");
            }
            return valid;
        }
    }
}
