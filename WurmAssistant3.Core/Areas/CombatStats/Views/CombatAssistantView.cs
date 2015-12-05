using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Views
{
    public partial class CombatAssistantView : ExtendedForm
    {
        readonly ICombatDataSource combatData;
        static readonly TimeSpan Treshhold = TimeSpan.FromSeconds(6);

        WidgetModeManager widgetModeManager;

        public CombatAssistantView(ICombatDataSource combatData)
        {
            if (combatData == null) throw new ArgumentNullException("combatData");
            this.combatData = combatData;
            InitializeComponent();

            currentAttackersLbl.Text = string.Empty;
            currentFocusLbl.Text = string.Empty;

            widgetModeManager = new WidgetModeManager(this);
            widgetModeManager.Set(true);
            widgetModeManager.WidgetModeChanging += (sender, args) =>
            {
                widgetHelpLbl.Visible = !args.WidgetMode;
            };
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateAttackers();
            UpdateFocus();
            UpdateAnimation();
        }

        void UpdateFocus()
        {
            var focus = combatData.CombatStatus.CurrentFocus;
            currentFocusLbl.Text = string.Format("{0} ({1})", ((int) focus.FocusLevel).ToString(), focus.ToString());
        }

        void UpdateAttackers()
        {
            var currentAttackers = combatData.CombatStatus.CurrentAttackers.GetCurrentWithCleanup(Treshhold, Treshhold).ToArray();
            if (currentAttackers.Any())
            {
                currentAttackersLbl.Text = string.Join(", ", currentAttackers.Select(attacker => attacker.Name));
            }
            else
            {
                currentAttackersLbl.Text = string.Empty;
            }
        }

        void UpdateAnimation()
        {
            currentAttackersLbl.ForeColor = DateTime.Now.Second%2 == 0 ? Color.Black : Color.Orange;
        }
    }
}
