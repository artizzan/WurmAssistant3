using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Parts.TriggersManager
{
    public partial class SimpleConditionTriggerBaseConfig : UserControl, ITriggerConfig
    {
        private readonly SimpleConditionTriggerBase _simpleConditionTriggerBase;

        private readonly bool initComplete = false;
        public SimpleConditionTriggerBaseConfig(SimpleConditionTriggerBase simpleConditionTriggerBase)
        {
            _simpleConditionTriggerBase = simpleConditionTriggerBase;
            InitializeComponent();
            ConditionTbox.Text = simpleConditionTriggerBase.Condition;
            DescLabel.Text = simpleConditionTriggerBase.ConditionHelp;
            checkBoxMatchEveryLine.Checked = _simpleConditionTriggerBase.MatchEveryLine;
            sourceTbox.Text = _simpleConditionTriggerBase.Source;
            SourceHelpLabel.Text = simpleConditionTriggerBase.SourceHelp;
            UpdateControls();
            initComplete = true;
        }

        public UserControl ControlHandle { get { return this; } }

        private void ConditionTbox_TextChanged(object sender, EventArgs e)
        {
            if (initComplete) _simpleConditionTriggerBase.Condition = ConditionTbox.Text;
        }

        private void sourceTbox_TextChanged(object sender, EventArgs e)
        {
            if (initComplete) _simpleConditionTriggerBase.Source = sourceTbox.Text;
        }

        private void checkBoxMatchEveryLine_CheckedChanged(object sender, EventArgs e)
        {
            if (initComplete)
            {
                _simpleConditionTriggerBase.MatchEveryLine = checkBoxMatchEveryLine.Checked;
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            ConditionTbox.Enabled = !checkBoxMatchEveryLine.Checked;
        }
    }
}
