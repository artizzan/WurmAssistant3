using System;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers
{
    public partial class PlayerLayoutView : UserControl
    {
        private readonly PlayerTimersGroup parentGroup;
        private WidgetModeManager widgetManager;

        public PlayerLayoutView()
        {
            InitializeComponent();
        }

        public PlayerLayoutView(PlayerTimersGroup playerTimersGroup)
            : this()
        {
            this.parentGroup = playerTimersGroup;
            this.label1.Text = parentGroup.CharacterName + " (conjuring, please wait)";
        }

        public WidgetModeManager WidgetManager
        {
            private get { return widgetManager; }
            set
            {
                widgetManager = value;
                widgetManager.WidgetModeChanging += _widgetManager_WidgetModeChanging;
            }
        }

        public int SortingOrder { get { return parentGroup.SortingOrder; } }

        void _widgetManager_WidgetModeChanging(object sender, WidgetModeEventArgs e)
        {
            buttonAdd.Visible = !e.WidgetMode;
            if (e.WidgetMode)
            {
                this.BackColor = parentGroup.TimersFeature.WidgetBgColor;
                this.ForeColor = parentGroup.TimersFeature.WidgetForeColor;
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
            parentGroup.AddNewTimer();
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

        internal void SetInitializationError()
        {
            this.label1.Text = string.Format("{0} - ERROR!", parentGroup.CharacterName);
        }

        internal void EnableAddingTimers()
        {
            this.label1.Text = string.Format("{0} ({1})", parentGroup.CharacterName, SimplifyServerGroup(parentGroup.ServerGroupId));
            buttonAdd.Enabled = true; 
        }

        private string SimplifyServerGroup(string serverGroupId)
        {
            string simpleName = serverGroupId;
            bool isServerScoped = false;
            if (serverGroupId.StartsWith("SERVERSCOPED:", StringComparison.InvariantCultureIgnoreCase))
            {
                isServerScoped = true;
                simpleName = serverGroupId.Remove(0, 13);
            }
            simpleName = simpleName.ToLowerInvariant().Capitalize();
            return isServerScoped ? ("S:" + simpleName) : simpleName;
        }
    }
}
