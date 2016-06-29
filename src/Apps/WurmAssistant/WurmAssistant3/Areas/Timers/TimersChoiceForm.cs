using System;
using System.Collections.Generic;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers
{
    public partial class TimersChoiceForm : ExtendedForm
    {
        public TimersChoiceForm()
        {
            InitializeComponent();
        }

        TimersForm parentForm;

        public TimersChoiceForm(IEnumerable<TimerDefinition> availableTypes, TimersForm parent)
            : this()
        {
            parentForm = parent;
            foreach (var type in availableTypes)
            {
                checkedListBox1.Items.Add(type);
            }
        }

        public HashSet<TimerDefinition> Result = new HashSet<TimerDefinition>();

        private void button1_Click(object sender, EventArgs e)
        {
            Result = new HashSet<TimerDefinition>();
            foreach (var item in checkedListBox1.CheckedItems)
            {
                Result.Add((TimerDefinition)item);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
