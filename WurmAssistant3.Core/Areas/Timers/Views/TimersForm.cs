using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Custom;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views
{
    public partial class TimersForm : ExtendedForm
    {
        readonly TimersFeature timersFeature;
        readonly ILogger logger;
        readonly IWurmApi wurmApi;
        readonly TimerDefinitions timerDefinitions;

        private readonly Dictionary<string, int> playerToListBoxIndexMap = new Dictionary<string, int>();
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
                if (panel1.Visible) panel1.Visible = false;
                string[] players = wurmApi.Characters.All.Select(character => character.Name.Capitalized).ToArray();

                int index = 0;
                foreach (var player in players)
                {
                    checkedListBoxPlayers.Items.Add(player);
                    playerToListBoxIndexMap.Add(player, index);
                    index++;
                }

                UpdateSelectedPlayers();

                widgetManager.Set(timersFeature.WidgetModeEnabled);

                formInited = true;
            }
            catch (Exception exception)
            {
                logger.Error(exception, "form load error");
            }
        }

        private void UpdateSelectedPlayers()
        {
            try
            {
                var selectedPlayers = timersFeature.GetActivePlayerGroups();
                for (int i = 0; i < checkedListBoxPlayers.Items.Count; i++)
                {
                    object item = checkedListBoxPlayers.Items[i];
                    try
                    {
                        if (selectedPlayers.Contains(item.ToString()))
                        {
                            checkedListBoxPlayers.SetItemChecked(playerToListBoxIndexMap[item.ToString()], true);
                        }
                        else
                        {
                            checkedListBoxPlayers.SetItemChecked(playerToListBoxIndexMap[item.ToString()], false);
                        }
                    }
                    catch (Exception _e)
                    {
                        logger.Error(_e, "problem updating player list");
                    }
                }
            }
            catch (Exception _e)
            {
                logger.Error(_e, "form load error");
            }
        }

        private void checkedListBoxPlayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (formInited)
            {
                string player = checkedListBoxPlayers.Items[e.Index].ToString();
                if (e.NewValue == CheckState.Checked)
                {
                    timersFeature.AddNewPlayerGroup(player);
                }
                else
                {
                    timersFeature.RemovePlayerGroup(player);
                }
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

        public void RestoreFromMin()
        {
            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
        }

        internal void RegisterTimersGroup(PlayerLayoutView layoutControl)
        {
            layoutControl.WidgetManager = widgetManager;
            flowLayoutPanel1.Controls.Add(layoutControl);
            widgetManager.ResetMouseEvents();
        }

        internal void UnregisterTimersGroup(PlayerLayoutView layoutControl)
        {
            flowLayoutPanel1.Controls.Remove(layoutControl);
            widgetManager.ResetMouseEvents();
        }

        private void buttonAddRemoveChars_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
        }

        private void checkedListBoxPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {

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
    }
}
