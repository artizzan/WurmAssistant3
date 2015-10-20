using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Calendar.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Legacy;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Calendar.Views
{
    public partial class FormCalendar : ExtendedForm
    {
        CalendarFeature ParentModule;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly ISoundEngine soundEngine;

        bool _WindowInitCompleted = false;
        bool serverListCreated = false;


        public FormCalendar([NotNull] CalendarFeature parentModule, [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger,
            [NotNull] ISoundEngine soundEngine)
        {
            if (parentModule == null) throw new ArgumentNullException("parentModule");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            InitializeComponent();
            this.ParentModule = parentModule;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.soundEngine = soundEngine;

            this.Size = ParentModule.Settings.MainWindowSize;
            radioButtonWurmTime.Checked = ParentModule.Settings.UseWurmTimeForDisplay;
            radioButtonRealTime.Checked = !ParentModule.Settings.UseWurmTimeForDisplay;
            checkBoxSoundWarning.Checked = ParentModule.Settings.SoundWarning;
            checkBoxPopupWarning.Checked = ParentModule.Settings.PopupWarning;
            textBoxChosenSound.Text = ParentModule.Settings.Sound.Name;

            CreateServerListAsync();
            _WindowInitCompleted = true;
        }

        private async Task CreateServerListAsync()
        {
            try
            {
                string[] allServers = wurmApi.Servers.All.Select(server => server.ServerName.Original).ToArray(); 
                comboBoxChooseServer.Items.AddRange(allServers);
                serverListCreated = true;
                comboBoxChooseServer.Enabled = true;
                comboBoxChooseServer.Text = ParentModule.Settings.ServerName;
            }
            catch (Exception _e)
            {
                logger.Error(_e, "CreateServerList problem");
            }
        }

        private void comboBoxChooseServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParentModule.Settings.ServerName = comboBoxChooseServer.Text;
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
                if (serverListCreated) textBoxWurmDate.Text = ParentModule.cachedWDT.ToString();
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
                ParentModule.Settings.UseWurmTimeForDisplay = true;
                labelDisplayTimeMode.Text = "Showing times as wurm time";
            }
        }

        private void radioButtonRealTime_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRealTime.Checked)
            {
                ParentModule.Settings.UseWurmTimeForDisplay = false;
                labelDisplayTimeMode.Text = "Showing times as real time";
            }
        }

        private void buttonChooseSeasons_Click(object sender, EventArgs e)
        {
            ParentModule.ChooseTrackedSeasons();
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
            var result = soundEngine.ChooseSound(this);
            if (result.ActionResult == ActionResult.Ok)
            {
                ParentModule.Settings.SoundId = result.SoundResource.Id;
                textBoxChosenSound.Text = result.SoundResource.Name;
            }

        }

        private void buttonClearSound_Click(object sender, EventArgs e)
        {
            ParentModule.Settings.SoundId = Guid.Empty;
            textBoxChosenSound.Text = ParentModule.Settings.Sound.Name;
        }

        private void checkBoxSoundWarning_CheckedChanged(object sender, EventArgs e)
        {
            ParentModule.Settings.SoundWarning = checkBoxSoundWarning.Checked;
        }

        private void checkBoxPopupWarning_CheckedChanged(object sender, EventArgs e)
        {
            ParentModule.Settings.PopupWarning = checkBoxPopupWarning.Checked;
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
            if (OperatingSystemInfo.RunningOS == OperatingSystemInfo.OStype.WinXP)
            {
                logger.Info("adjusting layout for WinXP");
                this.Size = new Size(this.Size.Width + 110, this.Size.Height);
            }
        }

        private void buttonModSeasonList_Click(object sender, EventArgs e)
        {
            ParentModule.ModifySeasons();
        }

        private void FormCalendar_Resize(object sender, EventArgs e)
        {
            try
            {
                if (_WindowInitCompleted)
                {
                    ParentModule.Settings.MainWindowSize = this.Size;
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
