using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Views
{
    public partial class CombatStatsFeatureView : ExtendedForm
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly FeatureSettings featureSettings;
        readonly IHostEnvironment hostEnvironment;
        readonly IProcessStarter processStarter;

        public CombatStatsFeatureView(IWurmApi wurmApi, ILogger logger, FeatureSettings featureSettings,
            IHostEnvironment hostEnvironment, IProcessStarter processStarter)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (featureSettings == null) throw new ArgumentNullException("featureSettings");
            if (hostEnvironment == null) throw new ArgumentNullException("hostEnvironment");
            if (processStarter == null) throw new ArgumentNullException("processStarter");
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.featureSettings = featureSettings;
            this.hostEnvironment = hostEnvironment;
            this.processStarter = processStarter;

            InitializeComponent();
            var characters =
                wurmApi.Characters.All.Select(character => character.Name.Capitalized).Cast<object>().ToArray();
            wurmCharacterCbox.Items.AddRange(characters);
            historicCharacterCbox.Items.AddRange(characters);
        }

        private void CombatAssistFeatureView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void createLiveSessionBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var character = wurmCharacterCbox.Text;

                if (string.IsNullOrWhiteSpace(character)) return;

                var monitor = new LiveLogsEventsMonitor(character, wurmApi, logger);
                monitor.Start();
                var view = new CombatResultsView(monitor, featureSettings, hostEnvironment, processStarter, logger);
                view.Text = "Live combat stats session for " + character;
                view.ShowCenteredOnForm(this);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error at begin live combat stats monitor.");
            }
        }

        private async void generateStatsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                generateStatsBtn.Enabled = false;

                var character = historicCharacterCbox.Text;

                if (string.IsNullOrWhiteSpace(character))
                    return;

                var parser = new LogSearchEventsParser(new LogSearchParameters()
                {
                    CharacterName = character,
                    LogType = LogType.Combat,
                    MinDate = fromDtpick.Value,
                    MaxDate = toDtpick.Value,
                    ScanResultOrdering = ScanResultOrdering.Ascending
                },
                    wurmApi,
                    logger);
                await parser.Process();
                var view = new CombatResultsView(parser, featureSettings, hostEnvironment, processStarter, logger);
                view.Text = string.Format("Aggregated combat results for {0} between {1} and {2}",
                    character,
                    fromDtpick.Value,
                    toDtpick.Value);
                view.ShowCenteredOnForm(this);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error at generate historic combat stats.");
            }
            finally
            {
                generateStatsBtn.Enabled = true;
            }
        }

        private void todayBtn_Click(object sender, EventArgs e)
        {
            SetToday();
        }

        private void last7DaysBtn_Click(object sender, EventArgs e)
        {
            SetLast7Days();
        }

        private void last30DaysBtn_Click(object sender, EventArgs e)
        {
            SetLast30Days();
        }

        private void SetToday()
        {
            fromDtpick.Value = DateTime.Now.Date;
            toDtpick.Value = DateTime.Now.Date.AddDays(1);
        }

        private void SetLast7Days()
        {
            fromDtpick.Value = DateTime.Now.Date.AddDays(-6);
            toDtpick.Value = DateTime.Now.Date.AddDays(1);
        }

        private void SetLast30Days()
        {
            fromDtpick.Value = DateTime.Now.Date.AddDays(-29);
            toDtpick.Value = DateTime.Now.Date.AddDays(1);
        }

        private void createAssistantBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var character = wurmCharacterCbox.Text;

                if (string.IsNullOrWhiteSpace(character))
                    return;

                var monitor = new LiveLogsEventsMonitor(character, wurmApi, logger);
                monitor.Start();
                var view = new CombatAssistantView(monitor);
                view.Text = character;
                view.ShowCenteredOnForm(this);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error at begin live combat stats monitor.");
            }
        }
    }
}
