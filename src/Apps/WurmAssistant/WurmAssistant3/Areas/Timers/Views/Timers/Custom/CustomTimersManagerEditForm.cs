using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Modules;
using AldursLab.WurmAssistant3.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Timers.Views.Timers.Custom
{
    public partial class CustomTimersManagerEditForm : ExtendedForm
    {
        readonly IWurmApi wurmApi;
        readonly TimerDefinitions timerDefinitions;
        readonly TimerDefinition definition;

        public CustomTimersManagerEditForm([NotNull] IWurmApi wurmApi, [NotNull] TimerDefinition definition,
            [NotNull] TimerDefinitions timerDefinitions)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (definition == null) throw new ArgumentNullException("definition");
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            this.wurmApi = wurmApi;
            this.timerDefinitions = timerDefinitions;
            this.definition = definition;

            InitializeComponent();

            foreach (var type in wurmApi.LogDefinitions.AllLogTypes)
            {
                comboBoxLogType.Items.Add(type);
            }
            comboBoxLogType.SelectedItem = LogType.Event;

            CustomTimerDefinition options = definition.CustomTimerConfig ?? new CustomTimerDefinition();
            textBoxTimerName.Text = definition.Name;
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
            timeInputUControl2.Value = options.Duration;
            checkBoxUptimeReset.Checked = options.ResetOnUptime;
        }

        private void CustomTimersManagerEditWindow_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(textBoxCond, "when Regex is disabled, timer will trigger if this text is logged in any selected log");
            toolTip1.SetToolTip(checkBoxAsRegex,
                "tip: use Log Searcher to test your Regex patterns.\r\nRegex pattern uses default C# settings, eg. is case sensitive (same as in Log Searcher)");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //validate
            if (IsValidData())
            {
                if (definition.CustomTimerConfig == null) definition.CustomTimerConfig = new CustomTimerDefinition();

                definition.Name = textBoxTimerName.Text;
                definition.CustomTimerConfig.ClearTriggers();
                definition.CustomTimerConfig.AddTrigger(textBoxCond.Text, (LogType)comboBoxLogType.SelectedItem, checkBoxAsRegex.Checked);
                definition.CustomTimerConfig.Duration = timeInputUControl2.Value;
                definition.CustomTimerConfig.ResetOnUptime = checkBoxUptimeReset.Checked;
                timerDefinitions.AddTimerDefinitionIfNotExists(definition);
                this.Close();
            }
        }

        bool IsValidData()
        {
            bool valid = true;
            if (textBoxTimerName.Text.Trim() == string.Empty)
            {
                valid = false;
                MessageBox.Show("Give your timer a name.");
            }
            return valid;
        }
    }
}
