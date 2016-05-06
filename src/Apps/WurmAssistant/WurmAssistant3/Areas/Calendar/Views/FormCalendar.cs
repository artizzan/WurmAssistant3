using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Calendar.Modules;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Calendar.Views
{
    public partial class FormCalendar : ExtendedForm
    {
        readonly CalendarFeature parentModule;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly ISoundManager soundManager;

        readonly bool windowInitCompleted = false;
        bool serverListCreated = false;


        public FormCalendar([NotNull] CalendarFeature parentModule, [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger,
            [NotNull] ISoundManager soundManager)
        {
            if (parentModule == null) throw new ArgumentNullException("parentModule");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            this.parentModule = parentModule;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.soundManager = soundManager;

            InitializeComponent();

            this.Size = this.parentModule.MainWindowSize;
            radioButtonWurmTime.Checked = this.parentModule.UseWurmTimeForDisplay;
            radioButtonRealTime.Checked = !this.parentModule.UseWurmTimeForDisplay;
            checkBoxSoundWarning.Checked = this.parentModule.SoundWarning;
            checkBoxPopupWarning.Checked = this.parentModule.PopupWarning;
            textBoxChosenSound.Text = this.parentModule.Sound.Name;

            CreateServerListAsync();
            windowInitCompleted = true;
        }

        private async void CreateServerListAsync()
        {
            try
            {
                var allServers =
                    wurmApi.Servers.All.Select(server => server.ServerName.Original).Cast<object>().ToArray();
                comboBoxChooseServer.Items.AddRange(allServers);
                serverListCreated = true;
                comboBoxChooseServer.Enabled = true;
                comboBoxChooseServer.Text = parentModule.ServerName;
            }
            catch (Exception _e)
            {
                logger.Error(_e, "CreateServerList problem");
            }
        }

        private void comboBoxChooseServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            parentModule.ServerName = comboBoxChooseServer.Text;
        }

        public void UpdateSeasonOutput(List<CalendarFeature.WurmSeasonOutputItem> outputList, bool wurmTime)
        {
            //ListView + double buffering
            //http://stackoverflow.com/questions/442817/c-sharp-flickering-listview-on-update
            ////
            listViewNFSeasons.Items.Clear();
            foreach (var item in outputList)
            {
                listViewNFSeasons.Items.Add(new ListViewItem(new string[] {
                    item.BuildName(), item.BuildTimeData(wurmTime), item.BuildLengthData(wurmTime) }));
            }
            //wurm date debug
            try
            {
                if (serverListCreated) textBoxWurmDate.Text = parentModule.cachedWDT.ToString();
            }
            catch
            {
                textBoxWurmDate.Text = "error";
            }
        }

        private void radioButtonWurmTime_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonWurmTime.Checked)
            {
                parentModule.UseWurmTimeForDisplay = true;
                labelDisplayTimeMode.Text = "Showing times as wurm time";
            }
        }

        private void radioButtonRealTime_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRealTime.Checked)
            {
                parentModule.UseWurmTimeForDisplay = false;
                labelDisplayTimeMode.Text = "Showing times as real time";
            }
        }

        private void buttonChooseSeasons_Click(object sender, EventArgs e)
        {
            parentModule.ChooseTrackedSeasons();
        }

        public void UpdateTrackedSeasonsList(string[] trackedSeasons)
        {
            if (trackedSeasons.Length > 0)
            {
                StringBuilder builder = new StringBuilder(120);
                foreach (string str in trackedSeasons)
                {
                    builder.Append(str).Append(", ");
                }
                builder.Remove(builder.Length - 2, 2);
                textBoxChosenSeasons.Text = builder.ToString();
            }
            else textBoxChosenSeasons.Text = "none";
        }

        private void buttonChooseSound_Click(object sender, EventArgs e)
        {
            var result = soundManager.ChooseSound(this);
            if (result.ActionResult == ActionResult.Ok)
            {
                parentModule.SoundId = result.SoundResource.Id;
                textBoxChosenSound.Text = result.SoundResource.Name;
            }

        }

        private void buttonClearSound_Click(object sender, EventArgs e)
        {
            parentModule.SoundId = Guid.Empty;
            textBoxChosenSound.Text = parentModule.Sound.Name;
        }

        private void checkBoxSoundWarning_CheckedChanged(object sender, EventArgs e)
        {
            parentModule.SoundWarning = checkBoxSoundWarning.Checked;
        }

        private void checkBoxPopupWarning_CheckedChanged(object sender, EventArgs e)
        {
            parentModule.PopupWarning = checkBoxPopupWarning.Checked;
        }

        private void FormCalendar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void buttonConfigure_Click(object sender, EventArgs e)
        {
            panelOptions.Visible = !panelOptions.Visible;
        }

        private void FormCalendar_Load(object sender, EventArgs e)
        {
            if (panelOptions.Visible) panelOptions.Visible = false;
        }

        private void buttonModSeasonList_Click(object sender, EventArgs e)
        {
            parentModule.ModifySeasons();
        }

        private void FormCalendar_Resize(object sender, EventArgs e)
        {
            try
            {
                if (windowInitCompleted)
                {
                    parentModule.MainWindowSize = this.Size;
                }
            }
            catch (Exception _e)
            {
                logger.Error(_e, "FormCalendar_Resize");
                throw;
            }
        }
    }
}
