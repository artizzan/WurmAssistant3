using System;
using System.Windows.Forms;
using AldursLab.WurmApi;
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
            var customtimers = timerDefinitions.GetNamesOfAllCustomTimers();
            foreach (var timer in customtimers)
            {
                listBox1.Items.Add(timer);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            //display window to add
            CustomTimersManagerEditForm ui = new CustomTimersManagerEditForm(wurmApi, timerDefinitions);
            ui.ShowCenteredOnForm(this);
            ui.FormClosed += OnTimerAdded;
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
                    timerDefinitions,
                    listBox1.SelectedItem.ToString());
                ui.FormClosed += OnTimerAdded;
                ui.ShowCenteredOnForm(this);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            //remove timer
            if (listBox1.SelectedIndex > -1)
                timerDefinitions.RemoveCustomTimerDefinition(listBox1.SelectedItem.ToString());
            ReloadList();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
