using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.RevealCreatures.Data;
using AldursLab.WurmAssistant3.WinForms;
using BrightIdeasSoftware;

namespace AldursLab.WurmAssistant3.Areas.RevealCreatures.Views
{
    public partial class RevealCreaturesView : ExtendedForm
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        readonly TextMatchFilter filter;

        public RevealCreaturesView(IWurmApi wurmApi, ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmApi = wurmApi;
            this.logger = logger;
            InitializeComponent();

            ClearOutput();

            filter = new TextMatchFilter(resultsView,
                "",
                StringComparison.InvariantCultureIgnoreCase);

            resultsView.DefaultRenderer =
                new HighlightTextRenderer(filter);

            gameChar.Items.AddRange(
                wurmApi.Characters.All.Select(character => character.Name.Capitalized).Cast<object>().ToArray());
        }

        private async void findLatest_Click(object sender, EventArgs e)
        {
            try
            {
                findLatest.Enabled = false;
                var gamechar = gameChar.Text;
                if (!string.IsNullOrWhiteSpace(gamechar))
                {
                    var apichar = wurmApi.Characters.Get(gamechar);

                    var results = await
                        apichar.Logs.ScanLogsAsync(
                            DateTime.Now.Subtract(TimeSpan.FromDays(90)),
                            DateTime.Now,
                            LogType.Event);

                    LogEntry latestCastEntry =
                        results.Where(entry => entry.Content.Equals("You receive insights about the area."))
                               .OrderBy(entry => entry.Timestamp)
                               .LastOrDefault();
                    if (latestCastEntry != null)
                    {
                        var parsedResults =
                            results.Where(
                                entry =>
                                    entry.Timestamp >= latestCastEntry.Timestamp
                                    && entry.Timestamp <= latestCastEntry.Timestamp + TimeSpan.FromSeconds(5))
                                   .Select(entry =>
                                   {
                                       var match = Regex.Match(entry.Content,
                                           @"The (.+?) is (.+)",
                                           RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                       if (match.Success)
                                       {
                                           return new FindResult()
                                           {
                                               Creature = match.Groups[1].Value,
                                               Direction = Direction.CreateFromLogEntryString(match.Groups[2].Value),
                                               Distance = Distance.CreateFromLogEntryString(match.Groups[2].Value)
                                           };
                                       }
                                       else
                                       {
                                           return null;
                                       }
                                   })
                                   .Where(result => result != null)
                                   .ToList();
                        resultsView.SetObjects(parsedResults);
                        castDate.Text = "Found: " + latestCastEntry.Timestamp.ToString(CultureInfo.CurrentCulture);
                        var ago = TryCalculateHowLongAgo(latestCastEntry.Timestamp);
                        howLongAgoLabel.Text = ago != TimeSpan.Zero ? $"({ago.ToStringCompact()} ago)" : string.Empty;
                    }
                    else
                    {
                        ClearOutput();
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Reveal creatures parsing failed");
            }
            finally
            {
                findLatest.Enabled = true;
            }
        }

        TimeSpan TryCalculateHowLongAgo(DateTime dateTime)
        {
            try
            {
                return DateTime.Now - dateTime;
            }
            catch (Exception)
            {
                return TimeSpan.Zero;
            }
        }

        void ClearOutput()
        {
            resultsView.ClearObjects();
            castDate.Text = string.Empty;
            howLongAgoLabel.Text = string.Empty;
        }

        private void RevealCreaturesView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void find_Click(object sender, EventArgs e)
        {
            
        }

        private void findText_TextChanged(object sender, EventArgs e)
        {
            var text = findText.Text ?? string.Empty;

            filter.ContainsStrings = new List<string>() { text };

            resultsView.BuildList(true);
        }

        private void castDate_Click(object sender, EventArgs e)
        {

        }
    }
}
