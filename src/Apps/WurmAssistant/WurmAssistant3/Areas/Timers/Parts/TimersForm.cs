using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers.Custom;
using AldursLab.WurmAssistant3.Areas.Timers.Singletons;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Timers.Parts
{
    public partial class TimersForm : ExtendedForm
    {
        readonly TimersFeature timersFeature;
        readonly ILogger logger;
        readonly IWurmApi wurmApi;
        readonly TimerDefinitions timerDefinitions;

        private bool formInited = false;
        private readonly WidgetModeManager widgetManager;

        public TimersForm([NotNull] TimersFeature timersFeature, [NotNull] ILogger logger, [NotNull] IWurmApi wurmApi,
            [NotNull] TimerDefinitions timerDefinitions)
        {
            if (timersFeature == null) throw new ArgumentNullException("timersFeature");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            this.timersFeature = timersFeature;
            this.logger = logger;
            this.wurmApi = wurmApi;
            this.timerDefinitions = timerDefinitions;
            InitializeComponent();
            widgetManager = new WidgetModeManager(this);
            widgetManager.WidgetModeChanging += (sender, args) =>
            {
                buttonAddRemoveChars.Visible
                    = buttonCustomTimers.Visible
                        = buttonOptions.Visible
                            = label1.Visible
                                = !args.WidgetMode;
            };
        }

        private void FormTimers_Load(object sender, EventArgs e)
        {
            if (this.Visible)
                this.Size = new Size(timersFeature.SavedWindowSize);
            try
            {
                widgetManager.Set(timersFeature.WidgetModeEnabled);

                formInited = true;
            }
            catch (Exception exception)
            {
                logger.Error(exception, "form load error");
            }
        }

        private void FormTimers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        internal void RegisterTimersGroup(PlayerLayoutView layoutControl)
        {
            if (!flowLayoutPanel1.Controls.Contains(layoutControl))
            {
                layoutControl.WidgetManager = widgetManager;
                flowLayoutPanel1.Controls.Add(layoutControl);
                ApplyTimerGroupsOrdering();
                widgetManager.ResetMouseEvents();
            }
        }

        internal void UnregisterTimersGroup(PlayerLayoutView layoutControl)
        {
            flowLayoutPanel1.Controls.Remove(layoutControl);
            ApplyTimerGroupsOrdering();
            widgetManager.ResetMouseEvents();
        }

        private void buttonAddRemoveChars_Click(object sender, EventArgs e)
        {
            var view = new EditTimerGroups(timersFeature, wurmApi, logger);
            view.ShowDialogCenteredOnForm(this);
            ApplyTimerGroupsOrdering();
        }

        private CustomTimersManagerForm customTimersManagerUi;

        private void buttonCustomTimers_Click(object sender, EventArgs e)
        {
            try
            {
                if (customTimersManagerUi.WindowState == FormWindowState.Minimized)
                    customTimersManagerUi.WindowState = FormWindowState.Normal;
                customTimersManagerUi.Show();
                customTimersManagerUi.BringToFront();
            }
            catch
            {
                customTimersManagerUi = new CustomTimersManagerForm(wurmApi, timerDefinitions);
                customTimersManagerUi.ShowCenteredOnForm(this);
            }
        }

        private void FormTimers_Resize(object sender, EventArgs e)
        {
            if (formInited)
            {
                timersFeature.SavedWindowSize = new Point(this.Size.Width, this.Size.Height);
            }
        }

        private void buttonOptions_Click(object sender, EventArgs e)
        {
            var ui = new GlobalTimerSettingsForm(this);
            ui.ShowDialogCenteredOnForm(this);
        }

        public bool WidgetModeEnabled
        {
            get { return timersFeature.WidgetModeEnabled; }
            set
            {
                timersFeature.WidgetModeEnabled = value;
                widgetManager.Set(value);
            }
        }

        public Color WidgetBgColor
        {
            get { return timersFeature.WidgetBgColor; }
            set
            {
                timersFeature.WidgetBgColor = value;
            }
        }

        public Color WidgetForeColor
        {
            get { return timersFeature.WidgetForeColor; }
            set
            {
                timersFeature.WidgetForeColor = value;
            }
        }

        public bool ShowEndDate
        {
            get { return timersFeature.ShowEndDate; }
            set { timersFeature.ShowEndDate = value; }
        }

        public bool ShowEndDateInsteadOfTimeRemaining
        {
            get { return timersFeature.ShowEndDateInsteadOfTimeRemaining; }
            set { timersFeature.ShowEndDateInsteadOfTimeRemaining = value; }
        }


        void ApplyTimerGroupsOrdering()
        {
            List<PlayerLayoutView> views = new List<PlayerLayoutView>();
            foreach (var control in flowLayoutPanel1.Controls)
            {
                views.Add((PlayerLayoutView)control);
            }
            views = views.OrderBy(view => view.SortingOrder).ToList();
            flowLayoutPanel1.Controls.Clear();
            foreach (var playerLayoutView in views)
            {
                flowLayoutPanel1.Controls.Add(playerLayoutView);
            }
        }
    }
}
