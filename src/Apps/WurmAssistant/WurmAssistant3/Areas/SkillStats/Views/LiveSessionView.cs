using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.SkillStats.Data;
using AldursLab.WurmAssistant3.WinForms;
using ILogger = AldursLab.WurmAssistant3.Areas.Logging.Contracts.ILogger;

namespace AldursLab.WurmAssistant3.Areas.SkillStats.Views
{
    public partial class LiveSessionView : ExtendedForm
    {
        readonly string gameChar;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        bool _collecting = false;
        readonly string descBase;

        readonly IWurmCharacter wurmCharacter;

        readonly List<LiveSkillReportItem> reportItems = new List<LiveSkillReportItem>();

        readonly Stopwatch stopwatch = new Stopwatch();

        public LiveSessionView(string gameChar, IWurmApi wurmApi, ILogger logger)
        {
            if (gameChar == null) throw new ArgumentNullException("gameChar");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.gameChar = gameChar;
            this.wurmApi = wurmApi;
            this.logger = logger;
            InitializeComponent();

            Text = "Skills monitor " + gameChar;

            wurmCharacter = wurmApi.Characters.Get(gameChar);

            objectListView.SetObjects(reportItems);
            descBase = gameChar + " (since: " + DateTime.Now.ToString("g") + ")";
            stopwatch.Start();

            Collecting = true;
            timer.Start();

            wurmCharacter.Skills.SkillsChanged += SkillsOnSkillsChanged;
        }

        void SkillsOnSkillsChanged(object sender, SkillsChangedEventArgs skillsChangedEventArgs)
        {
            if (stopwatch.IsRunning)
            {
                foreach (var skillChange in skillsChangedEventArgs.SkillChanges)
                {
                    var reportItem =
                        reportItems.FirstOrDefault(
                            item =>
                                item.Name.Equals(skillChange.NameNormalized, StringComparison.InvariantCultureIgnoreCase));
                    if (reportItem == null)
                    {
                        reportItem = new LiveSkillReportItem()
                        {
                            StartValue = skillChange.Value - (skillChange.Gain ?? 0f),
                            Name = skillChange.NameNormalized.ToLowerInvariant().Capitalize()
                        };
                        reportItems.Add(reportItem);
                        objectListView.BuildList(true);
                    }
                    reportItem.CurrentValue = skillChange.Value;
                }
                RefreshList();
            }
        }

        void RefreshAverageGains()
        {
            var elapsedHours = (double)stopwatch.ElapsedMilliseconds / (1000D * 60D * 60D);
            reportItems.Apply(item => item.AverageGainPerHour = ((double)item.CurrentValue - (double)item.StartValue) / elapsedHours);
            RefreshList();
        }

        bool Collecting
        {
            get { return _collecting; }
            set
            {
                _collecting = value;
                if (_collecting) stopwatch.Start();
                else stopwatch.Stop();
                RefreshDescription();
            }
        }

        private void startPauseBtn_Click(object sender, EventArgs e)
        {
            Collecting = !Collecting;
        }

        void RefreshDescription()
        {
            statusLbl.Text = string.Format("{0} (Recorded minutes: {1}){2}",
                descBase,
                (int)TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalMinutes,
                (Collecting ? "" : " (paused)"));
            startPauseBtn.Text = Collecting ? "Pause" : "Resume";
        }

        private void LiveSessionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            wurmCharacter.Skills.SkillsChanged -= SkillsOnSkillsChanged;
        }

        private void LiveSessionView_VisibleChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        void RefreshList()
        {
            if (Visible)
            {
                objectListView.BuildList(true);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            RefreshAverageGains();
            RefreshDescription();
        }
    }
}
