using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers
{
    public partial class PlayerLayoutView : UserControl
    {
        private PlayerTimersGroup ParentGroup;
        private WidgetModeManager _widgetManager;

        public PlayerLayoutView()
        {
            InitializeComponent();
            //this.BackColor = DefaultBackColor;
        }

        public PlayerLayoutView(PlayerTimersGroup playerTimersGroup)
            : this()
        {
            this.ParentGroup = playerTimersGroup;
            this.label1.Text = ParentGroup.CharacterName + " (conjuring, please wait)";
        }

        public WidgetModeManager WidgetManager
        {
            private get { return _widgetManager; }
            set
            {
                _widgetManager = value;
                _widgetManager.WidgetModeChanging += _widgetManager_WidgetModeChanging;
            }
        }

        void _widgetManager_WidgetModeChanging(object sender, WidgetModeEventArgs e)
        {
            buttonAdd.Visible = !e.WidgetMode;
            if (e.WidgetMode)
            {
                this.BackColor = ParentGroup.TimersFeature.WidgetBgColor;
                this.ForeColor = ParentGroup.TimersFeature.WidgetForeColor;
            }
            else
            {
                this.BackColor = DefaultBackColor;
                this.ForeColor = DefaultForeColor;
            }
        }

        private void UControlPlayerLayout_Load(object sender, EventArgs e)
        {
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            ParentGroup.AddNewTimer();
        }

        internal void RegisterNewTimerDisplay(TimerDisplayView ControlTimer)
        {
            ControlTimer.WidgetManager = WidgetManager;
            this.flowLayoutPanel1.Controls.Add(ControlTimer);
            if (WidgetManager != null) WidgetManager.ResetMouseEvents();
        }

        internal void UnregisterTimerDisplay(TimerDisplayView ControlTimer)
        {
            this.flowLayoutPanel1.Controls.Remove(ControlTimer);
        }

        internal void EnableAddingTimers()
        {
            this.label1.Text = ParentGroup.CharacterName;
            buttonAdd.Enabled = true;
        }
    }
}
