using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;
using ILogger = AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts.ILogger;

namespace AldursLab.WurmAssistant3.Core.Areas.LogSearcher.Views
{
    public partial class LogSearchView : ExtendedForm, IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        List<SingleSearchMatch> lastMatches = new List<SingleSearchMatch>();
        bool searching = false;
        FormWindowState lastVisibleWindowState;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private IEnumerable<SearchTypeId> SearchTypes
        {
            get { return new[] { SearchTypeId.RegexEscapedCaseIns, SearchTypeId.RegexCustom }; }
        }

        public LogSearchView([NotNull] IWurmApi wurmApi, [NotNull] ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wurmApi = wurmApi;
            this.logger = logger;

            InitializeComponent();
            lastVisibleWindowState = this.WindowState;

            comboBoxPlayerName.Items.AddRange(
                wurmApi.Characters.All.Select(character => (object)character.Name).ToArray());
            comboBoxLogType.Items.AddRange(wurmApi.LogDefinitions.AllLogTypes.Select(type => (object)type.ToString()).ToArray());
            comboBoxSearchType.Items.AddRange(SearchTypes.Select(type => (object)type.ToString()).ToArray());

            dateTimePickerTimeFrom.Value = DateTime.Now;
            dateTimePickerTimeTo.Value = DateTime.Now;

            comboBoxLogType.Text = LogType.Event.ToString();
            comboBoxSearchType.Text = SearchTypeId.RegexEscapedCaseIns.ToString();

            if (comboBoxPlayerName.Items.Count > 0) comboBoxPlayerName.Text = comboBoxPlayerName.Items[0].ToString();
        }

        #region IFeature

        void IFeature.Show()
        {
            this.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
            this.Hide();
        }

        string IFeature.Name { get { return "Log Searcher"; } }

        Image IFeature.Icon { get { return Resources.LogSearcherIcon; } }

        async Task IFeature.InitAsync()
        {
            // no initialization required
            await Task.FromResult(true);
        }

        #endregion

        async void PerformSearch()
        {
            LogSearchParameters searchParams = null;

            try
            {
                if (searching) throw new InvalidOperationException("Search already running");
                searching = true;

                cancellationTokenSource = new CancellationTokenSource();

                buttonCancelSearch.Visible = true;

                richTextBoxAllLines.Clear();
                listBoxAllResults.Items.Clear();

                dateTimePickerTimeFrom.Value = new DateTime(
                    dateTimePickerTimeFrom.Value.Year,
                    dateTimePickerTimeFrom.Value.Month,
                    dateTimePickerTimeFrom.Value.Day,
                    0,
                    0,
                    0);
                dateTimePickerTimeTo.Value = new DateTime(
                    dateTimePickerTimeTo.Value.Year,
                    dateTimePickerTimeTo.Value.Month,
                    dateTimePickerTimeTo.Value.Day,
                    23,
                    59,
                    59);

                var pmCharacter = GetPmCharacter();

                searchParams = new LogSearchParameters()
                {
                    LogType = GetLogType(),
                    CharacterName = GetCharacter(),
                    MinDate = dateTimePickerTimeFrom.Value,
                    MaxDate = dateTimePickerTimeTo.Value,
                    PmRecipientName = pmCharacter
                };

                var searchType = GetSearchType();
                var searchPhrase = GetPhrase();

                var result = await Search(searchParams, cancellationTokenSource.Token);

                ParseAndDisplay(result, searchType, searchPhrase, searchParams);
            }
            catch (OperationCanceledException exception)
            {
                // cancelled
                logger.Info(exception, "Search cancelled.");
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Search error, params: " + (searchParams != null ? searchParams.ToString() : "NULL"));
                MessageBox.Show(exception.ToString());
            }
            finally
            {
                searching = false;
                labelWorking.Hide();
                buttonCancelSearch.Visible = false;
                listBoxAllResults.Visible = true;
                richTextBoxAllLines.Visible = true;
                buttonCommitSearch.Text = "Search";
                richTextBoxAllLines.Select(0, 0);
                richTextBoxAllLines.ScrollToCaret();
            }
        }

        async Task<IEnumerable<LogEntry>> Search([NotNull] LogSearchParameters searchParams,
            CancellationToken cancellationToken)
        {
            if (searchParams == null)
                throw new ArgumentNullException("searchParams");
            var result = await wurmApi.LogsHistory.ScanAsync(searchParams, cancellationToken);

            return result;
        }

        SearchTypeId GetSearchType()
        {
            SearchTypeId typeId;
            Enum.TryParse(comboBoxSearchType.Text, out typeId);
            return typeId;
        }

        LogType GetLogType()
        {
            LogType type;
            Enum.TryParse(comboBoxLogType.Text, out type);
            return type;
        }

        string GetCharacter()
        {
            return comboBoxPlayerName.Text ?? string.Empty;
        }

        string GetPmCharacter()
        {
            if (GetLogType() == LogType.Pm)
            {
                return textBoxPM.Text ?? string.Empty;
            }
            else
            {
                return null;
            }
        }

        string GetPhrase()
        {
            return textBoxSearchKey.Text ?? string.Empty;
        }

        void ParseAndDisplay(IEnumerable<LogEntry> result, SearchTypeId searchTypeId, string searchPhrase, LogSearchParameters searchParams)
        {
            List<SingleSearchMatch> matches = new List<SingleSearchMatch>();

            result = FilterResults(result, searchParams);

            List<string> results = new List<string>();
            int currentLineBeginIndex = 0;
            var pattern = searchTypeId == SearchTypeId.RegexEscapedCaseIns
                ? ("(?i)" + Regex.Escape(searchPhrase))
                : searchPhrase;

            foreach (var logEntry in result)
            {
                // restoring entry as string, so legacy code can be used without major rewrite
                var line = RestoreLogEntry(logEntry);
                results.Add(line);

                if (!string.IsNullOrWhiteSpace(searchPhrase))
                {
                    MatchCollection matchcollection = Regex.Matches(line, pattern);
                    foreach (Match match in matchcollection)
                    {
                        long matchStart = currentLineBeginIndex + match.Index;
                        long matchLength = match.Length;

                        matches.Add(new SingleSearchMatch(matchStart, matchLength, BuildDateForMatch(line)));
                    }
                }
                currentLineBeginIndex += line.Length + 1; // richtextbox seems to always add 1-length eol ??
            }

            lastMatches = matches;

            buttonCommitSearch.Text = "Loading results...";

            labelAllResults.Text = "All results: " + results.Count;

            richTextBoxAllLines.Visible = false;
            listBoxAllResults.Visible = false;
            labelWorking.Show();
            this.Refresh();

            richTextBoxAllLines.Clear();
            listBoxAllResults.Items.Clear();
            richTextBoxAllLines.Lines = results.ToArray();
            if (matches.Any())
            {
                bool tooManyToProcess = false;
                bool tooManyToHighlight = false;
                if (results.Count > 20000)
                    tooManyToProcess = true;
                if (results.Count > 5000)
                    tooManyToHighlight = true;
                if (!tooManyToProcess)
                {
                    foreach (var searchmatch in matches)
                    {
                        string matchDesc = "";
                        matchDesc += searchmatch.MatchDate;
                        if (!tooManyToHighlight)
                        {
                            richTextBoxAllLines.Select((int)searchmatch.BeginCharPos, (int)searchmatch.LenghtChars);
                            richTextBoxAllLines.SelectionBackColor = Color.LightBlue;
                        }
                        listBoxAllResults.Items.Add(matchDesc);
                        Application.DoEvents();
                    }
                }
                else
                {
                    listBoxAllResults.Items.Add("too many matches");
                    listBoxAllResults.Items.Add("narrow the search");
                }
            }
        }

        /// <summary>
        /// Converts Log Entry into universal string representation resembling original log entry + date
        /// </summary>
        /// <returns></returns>
        public string RestoreLogEntry(LogEntry logentry)
        {
            var stamp = logentry.Timestamp;
            return string.Format("{0}{1} {2}",
                stamp.ToString("[yyyy-MM-dd] [HH:mm:ss]", CultureInfo.InvariantCulture),
                FormatSource(logentry),
                logentry.Content);
        }

        static string FormatSource(LogEntry logentry)
        {
            if (string.IsNullOrEmpty(logentry.Source)) return string.Empty;
            var result = " <" + logentry.Source + ">";
            if (
                logentry.PmConversationRecipient != CharacterName.Empty
                && !logentry.Source.Equals(logentry.PmConversationRecipient.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                result += " to <" + logentry.PmConversationRecipient + ">";
            }
            return result;
        }

        static IEnumerable<LogEntry> FilterResults(IEnumerable<LogEntry> result, LogSearchParameters searchParams)
        {
            if (searchParams.LogType == LogType.Pm)
            {
                if (!string.IsNullOrEmpty(searchParams.PmRecipientName))
                {
                    result =
                        result.Where(
                            s =>
                                s.PmConversationRecipient.Normalized.Equals(
                                    searchParams.PmRecipientName.ToUpperInvariant())).ToArray();
                }
            }
            return result;
        }

        static DateTime BuildDateForMatch(string line)
        {
            DateTime matchDate;
            try
            {
                matchDate = new DateTime(
                    Convert.ToInt32(line.Substring(1, 4)),
                    Convert.ToInt32(line.Substring(6, 2)),
                    Convert.ToInt32(line.Substring(9, 2)),
                    Convert.ToInt32(line.Substring(14, 2)),
                    Convert.ToInt32(line.Substring(17, 2)),
                    Convert.ToInt32(line.Substring(20, 2))
                    );
            }
            catch
            {
                matchDate = new DateTime(0);
            }

            return matchDate;
        }

        private void buttonCommitSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void buttonCancelSearch_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        private void listBoxAllResults_Click(object sender, EventArgs e)
        {
            if (listBoxAllResults.SelectedIndex > -1 && richTextBoxAllLines.Lines.Length >= listBoxAllResults.SelectedIndex)
            {
                if (lastMatches.Count > listBoxAllResults.SelectedIndex)
                {
                    var matchdata = lastMatches[listBoxAllResults.SelectedIndex];
                    richTextBoxAllLines.Select((int)matchdata.BeginCharPos, (int)matchdata.LenghtChars);
                    richTextBoxAllLines.Focus();
                }
            }
        }

        private void FormLogSearcher_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.textBoxSearchKey, "For \"Match\" search use * to indicate 1 or more of any characters");
            toolTip1.SetToolTip(this.comboBoxSearchType, "Match simply searches for the keyword,\r\nRegex searches for anything matching regex pattern");
            toolTip1.SetToolTip(this.textBoxPM, "Leave this empty to see all PM's sorted by day by recipient,\r\nor set a single name to look for");
        }

        private void richTextBoxAllLines_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch (Exception exception)
            {
                //todo proper
                MessageBox.Show(exception.ToString());
            }
        }

        private void textBoxSearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessTabKey(true);
            }
        }

        private void textBoxSearchKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }

        }

        private void dateTimePickerTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessTabKey(true);
            }
        }

        private void dateTimePickerTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessTabKey(true);
            }
        }

        private void comboBoxLogType_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessTabKey(true);
            }
        }

        private void comboBoxPlayerName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessTabKey(true);
            }
        }

        private void comboBoxSearchType_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessTabKey(true);
            }
        }

        private void textBoxPM_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessTabKey(true);
            }
        }

        private void comboBoxLogType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLogType.Text == LogType.Pm.ToString())
            {
                labelPM.Visible = true;
                textBoxPM.Visible = true;
                textBoxPM.TabStop = true;
            }
            else
            {
                labelPM.Visible = false;
                textBoxPM.Visible = false;
                textBoxPM.TabStop = false;
            }
        }

        public void Display()
        {
            this.Show();
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = lastVisibleWindowState;
            }
        }

        private void LogSearcherForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void LogSearcherForm_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized) lastVisibleWindowState = WindowState;
        }
    }
}
