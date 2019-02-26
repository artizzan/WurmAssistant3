using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmAssistant3.Areas.Insights;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.SkillStats
{
    public partial class SkillStatsFeatureForm : ExtendedForm
    {
        readonly SkillStatsFeature feature;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly ITelemetry telemetry;

        public SkillStatsFeatureForm(SkillStatsFeature feature, IWurmApi wurmApi, ILogger logger, [NotNull] ITelemetry telemetry)
        {
            if (feature == null) throw new ArgumentNullException("feature");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.feature = feature;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            InitializeComponent();

            var allChars = this.wurmApi.Characters.All.Select(character => character).ToArray();
            liveMonCharacterCbox.Items.AddRange(
                allChars.Select(character => character.Name.Capitalized).Cast<object>().ToArray());
            queryGameCharsCblist.Items.AddRange(allChars.Cast<object>().ToArray());
            var serverGroups = this.wurmApi.ServerGroups.AllKnown;
            serverGroupCbox.Items.AddRange(serverGroups.Select(group => group.ServerGroupId).Cast<object>().ToArray());

            SetToday();
        }

        private void SkillStatsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
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

        private void startLiveSessionBtn_Click(object sender, EventArgs e)
        {
            try
            {
                telemetry.TrackEvent("Skill Stats: starting live session");

                var gameChar = liveMonCharacterCbox.Text;
                if (string.IsNullOrWhiteSpace(gameChar))
                {
                    MessageBox.Show("Please choose game character");
                    return;
                }
                var view = new LiveSessionForm(gameChar, wurmApi, logger);
                view.ShowCenteredOnForm(this);
            }
            catch (Exception exception)
            {
                ShowError(exception);
                logger.Error(exception, "Live session start failed");
            }
        }

        private async void generateQueryBtn_Click(object sender, EventArgs e)
        {
            try
            {
                telemetry.TrackEvent("Skill Stats: generating query");

                ThrowIfNoServerGroup();

                generateQueryBtn.Enabled = false;
                var queryParams = BuildQueryParams();

                List<SkillGainReportItem> reportItems = new List<SkillGainReportItem>();
                var serverGroup = queryParams.ServerGroupId;
                if (!string.IsNullOrWhiteSpace(serverGroup))
                {
                    var parser = new SkillEntryParser(wurmApi);
                    foreach (var character in queryParams.GameCharacters)
                    {
                        var wurmChar = wurmApi.Characters.Get(character);
                        var entries = await wurmChar.Logs.ScanLogsServerGroupRestrictedAsync(fromDtpick.Value,
                            toDtpick.Value,
                            LogType.Skills,
                            new ServerGroup(serverGroup));
                        var characterReportItems = new List<SkillGainReportItem>();
                        foreach (
                            var logEntry in
                                entries.Where(
                                    entry => entry.Timestamp >= fromDtpick.Value && entry.Timestamp <= toDtpick.Value))
                        {
                            var skillEntry = parser.TryParseSkillInfoFromLogLine(logEntry);
                            if (skillEntry != null)
                            {
                                var item = characterReportItems.FirstOrDefault(x => skillEntry.IsSkillName(x.Name));
                                if (item == null)
                                {
                                    item = new SkillGainReportItem()
                                    {
                                        Name = skillEntry.NameNormalized.ToLowerInvariant().Capitalize(),
                                        GameCharacter = character,
                                        StartValue = skillEntry.Value - (skillEntry.Gain ?? 0f)
                                    };
                                    characterReportItems.Add(item);
                                }
                                item.CurrentValue = skillEntry.Value;
                            }
                        }
                        reportItems.AddRange(characterReportItems);
                    }

                    var view = new SkillGainsForm(queryParams, reportItems);
                    view.ShowCenteredOnForm(this);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
                logger.Error(exception, "Historic skills query build failed");
            }
            finally
            {
                generateQueryBtn.Enabled = true;
            }
        }

        private async void totalSkillReportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                telemetry.TrackEvent("Skill Stats: showing total skills");

                ThrowIfNoServerGroup();

                totalSkillReportBtn.Enabled = false;
                var queryParams = BuildQueryParams();
                queryParams.QueryKind = QueryKind.TotalSkills;

                var reportItems = await GetSkillLevelsAsync(queryParams);

                var view = new SkillLevelsForm(queryParams, reportItems);
                view.ShowCenteredOnForm(this);

            }
            catch (Exception exception)
            {
                ShowError(exception);
                logger.Error(exception, "Total skills query build failed");
            }
            finally
            {
                totalSkillReportBtn.Enabled = true;
            }
        }

        private async void bestSkillsReportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                telemetry.TrackEvent("Skill Stats: showing best skills");

                ThrowIfNoServerGroup();

                bestSkillsReportBtn.Enabled = false;
                var queryParams = BuildQueryParams();
                queryParams.QueryKind = QueryKind.BestSkill;

                var reportItems = await GetSkillLevelsAsync(queryParams);
                reportItems =
                    reportItems.GroupBy(item => item.Name)
                               .Select(items => items.OrderByDescending(item => item.CurrentValue).First())
                               .ToList();

                var view = new SkillLevelsForm(queryParams, reportItems);
                view.ShowCenteredOnForm(this);
            }
            catch (Exception exception)
            {
                ShowError(exception);
                logger.Error(exception, "Best skills query build failed");
            }
            finally
            {
                bestSkillsReportBtn.Enabled = true;
            }
        }

        void ThrowIfNoServerGroup()
        {
            if (string.IsNullOrWhiteSpace(serverGroupCbox.Text))
            {
                throw new ArgumentException("Server group must be selected");
            }
        }

        async Task<List<SkillLevelReportItem>> GetSkillLevelsAsync(QueryParams queryParams)
        {
            List<SkillLevelReportItem> reportItems = new List<SkillLevelReportItem>();
            var serverGroup = queryParams.ServerGroupId;
            if (!string.IsNullOrWhiteSpace(serverGroup))
            {
                var parser = new SkillEntryParser(wurmApi);
                foreach (var character in queryParams.GameCharacters)
                {
                    var wurmChar = wurmApi.Characters.Get(character);

                    var characterReportItems = new List<SkillLevelReportItem>();

                    var latestSkillDump = await wurmChar.Skills.GetLatestSkillDumpAsync(serverGroup);
                    if (!latestSkillDump.IsNull)
                    {
                        foreach (var skillInfo in latestSkillDump.All)
                        {
                            var item = characterReportItems.FirstOrDefault(x => skillInfo.IsSkillName(x.Name));
                            if (item == null)
                            {
                                item = new SkillLevelReportItem()
                                {
                                    Name = skillInfo.NameNormalized.ToLowerInvariant().Capitalize(),
                                    GameCharacter = character,
                                };
                                characterReportItems.Add(item);
                            }
                            item.CurrentValue = skillInfo.Value;
                        }
                    }

                    var entries =
                        await
                            wurmChar.Logs.ScanLogsServerGroupRestrictedAsync(
                                !latestSkillDump.IsNull ? latestSkillDump.Stamp : DateTime.Now - TimeSpan.FromDays(365),
                                DateTime.Now,
                                LogType.Skills,
                                new ServerGroup(serverGroup));

                    foreach (var logEntry in entries)
                    {
                        var skillEntry = parser.TryParseSkillInfoFromLogLine(logEntry);
                        if (skillEntry != null)
                        {
                            var item = characterReportItems.FirstOrDefault(x => skillEntry.IsSkillName(x.Name));
                            if (item == null)
                            {
                                item = new SkillLevelReportItem()
                                {
                                    Name = skillEntry.NameNormalized.ToLowerInvariant().Capitalize(),
                                    GameCharacter = character,
                                };
                                characterReportItems.Add(item);
                            }
                            item.CurrentValue = skillEntry.Value;
                        }
                    }
                    reportItems.AddRange(characterReportItems);
                }
            }

            return reportItems;
        }

        void ShowError(Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        QueryParams BuildQueryParams()
        {
            return new QueryParams()
            {
                To = toDtpick.Value,
                From = fromDtpick.Value,
                GameCharacters =
                    queryGameCharsCblist.CheckedItems.Cast<IWurmCharacter>()
                                        .Select(character => character.Name.Capitalized)
                                        .ToArray(),
                ServerGroupId = serverGroupCbox.Text
            };
        }
    }
}
