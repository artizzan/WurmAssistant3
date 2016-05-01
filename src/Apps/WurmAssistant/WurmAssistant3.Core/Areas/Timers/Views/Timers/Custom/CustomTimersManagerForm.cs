using System;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Custom
{
    public partial class CustomTimersManagerForm : ExtendedForm
    {
        readonly IWurmApi wurmApi;
        readonly TimerDefinitions timerDefinitions;

        public CustomTimersManagerForm([NotNull] IWurmApi wurmApi,
            [NotNull] TimerDefinitions timerDefinitions)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            this.wurmApi = wurmApi;
            this.timerDefinitions = timerDefinitions;
            InitializeComponent();
            ReloadList();
        }

        void ReloadList()
        {
            listBox1.Items.Clear();
            var definitions = timerDefinitions.GetCustomTimerDefinitions();
            foreach (var definition in definitions)
            {
                listBox1.Items.Add(definition);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            //display window to add
            CustomTimersManagerEditForm ui = new CustomTimersManagerEditForm(wurmApi,
                new TimerDefinition(Guid.NewGuid())
                {
                    RuntimeTypeId = RuntimeTypeId.LegacyCustom
                }, timerDefinitions);
            ui.FormClosed += OnTimerAdded;
            ui.ShowCenteredOnForm(this);
        }

        private void OnTimerAdded(object sender, EventArgs e)
        {
            ReloadList();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                CustomTimersManagerEditForm ui = new CustomTimersManagerEditForm(wurmApi,
                    (TimerDefinition)listBox1.SelectedItem, timerDefinitions);
                ui.FormClosed += OnTimerAdded;
                ui.ShowCenteredOnForm(this);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                if (
                    MessageBox.Show(
                        "Selected timer will be removed, along with any such timers among all groups, together with their settings. Confirm?",
                        "Confirm removal",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    timerDefinitions.RemoveCustomTimerDefinition(((TimerDefinition) listBox1.SelectedItem).Id);
                    ReloadList();
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
