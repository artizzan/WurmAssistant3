using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.Services;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant.Parts
{
    public partial class CombatAssistantFeatureForm : ExtendedForm
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly FeatureSettings featureSettings;
        readonly IProcessStarter processStarter;

        public CombatAssistantFeatureForm(IWurmApi wurmApi, ILogger logger, FeatureSettings featureSettings,
            IProcessStarter processStarter)
        {
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (featureSettings == null) throw new ArgumentNullException(nameof(featureSettings));
            if (processStarter == null) throw new ArgumentNullException(nameof(processStarter));
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.featureSettings = featureSettings;
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
                var view = new CombatResultsForm(monitor, featureSettings, processStarter, logger);
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
                var view = new CombatResultsForm(parser, featureSettings, processStarter, logger);
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
                var view = new CombatWidgetForm(monitor);
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
