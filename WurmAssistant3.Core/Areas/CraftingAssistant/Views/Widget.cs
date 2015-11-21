using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.CraftingAssistant.Views
{
    public partial class Widget : ExtendedForm
    {
        readonly string characterName;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        bool requireRepair;
        string currentActionNeeded;

        WidgetModeManager widgetModeManager;

        public Widget(string characterName, IWurmApi wurmApi, ILogger logger)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.characterName = characterName;
            this.wurmApi = wurmApi;
            this.logger = logger;
            InitializeComponent();

            actionLbl.Text = string.Empty;
            widgetModeManager = new WidgetModeManager(this);
            widgetModeManager.Set(true);
            widgetModeManager.WidgetModeChanging += (sender, args) =>
            {
                widgetHelpLbl.Visible = !args.WidgetMode;
            };
        }

        private void Widget_Load(object sender, EventArgs e)
        {
            Text = characterName + " (CraftAssist)";
            wurmApi.LogsMonitor.Subscribe(characterName, LogType.Event, EventHandler);
            timer.Enabled = true;
        }

        void EventHandler(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            foreach (var entry in logsMonitorEventArgs.WurmLogEntries)
            {
                // The pendulum could be improved with some more lump.
                // The scissors could be improved with some more lump.
                // The torch lamp (lit) could be improved with some more lump.
                // The clay jar could be improved with some more clay.
                // The forge could be improved with some more rock shards.
                // The wagon could be improved with some more log.
                // The dioptra could be improved with a lump.
                // The wagon could be improved with a log.
                {
                    var match = Regex.Match(entry.Content, @"The .+ could be improved with (.+)\.", RegexOptions.Compiled);
                    if (match.Success) currentActionNeeded = match.Groups[1].Value;
                }

                // The clay flowerpot needs water.
                {
                    var match = Regex.Match(entry.Content, @"The .+ needs (.+)\.", RegexOptions.Compiled);
                    if (match.Success)
                        currentActionNeeded = match.Groups[1].Value;
                }

                // The lamp has some dents that must be flattened by a hammer.
                {
                    var match = Regex.Match(entry.Content,
                        @"The .+ has some dents that must be flattened by (.+)\.",
                        RegexOptions.Compiled);
                    if (match.Success)
                        currentActionNeeded = match.Groups[1].Value;
                }

                // The clay flowerpot has some flaws that must be fixed with a spatula.
                // The clay flowerpot has some flaws that must be fixed with a clay shaper.
                {
                    var match = Regex.Match(entry.Content,
                        @"The .+ has some flaws that must be fixed with (.+)\.",
                        RegexOptions.Compiled);
                    if (match.Success)
                        currentActionNeeded = match.Groups[1].Value;
                }

                // You will want to polish the lamp with a pelt before you improve it.
                {
                    var match = Regex.Match(entry.Content,
                        @"You will want to polish the .+ with a (.+) before you improve it\.",
                        RegexOptions.Compiled);
                    if (match.Success)
                        currentActionNeeded = match.Groups[1].Value;
                }

                // You need to temper the lamp by dipping it in water while it's hot.
                {
                    var match = Regex.Match(entry.Content,
                        @"You need to temper the .+ by dipping it in (.+) while it's hot\.",
                        RegexOptions.Compiled);
                    if (match.Success)
                        currentActionNeeded = match.Groups[1].Value;
                }

                // A tool for digging. It could be improved with a lump.
                {
                    var match = Regex.Match(entry.Content,
                        @".+It could be improved with (.+)\.",
                        RegexOptions.Compiled);
                    if (match.Success)
                        currentActionNeeded = match.Groups[1].Value;
                }

                // You damage the lamp a little.
                if (entry.Content.Contains("You damage")) requireRepair = true;

                // You repair the lamp.
                if (entry.Content.Contains("You repair")) requireRepair = false;

            }
        }

        private void Widget_FormClosing(object sender, FormClosingEventArgs e)
        {
            wurmApi.LogsMonitor.Unsubscribe(characterName, EventHandler);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Act = requireRepair ? "Repair" : currentActionNeeded;
        }

        public string Act { get { return actionLbl.Text; } set { actionLbl.Text = value ?? string.Empty; } }
    }
}
