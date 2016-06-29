using System;
using System.Drawing;
using System.Linq;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant
{
    public partial class CombatWidgetForm : ExtendedForm
    {
        readonly ICombatDataSource combatData;
        static readonly TimeSpan Treshhold = TimeSpan.FromSeconds(6);

        WidgetModeManager widgetModeManager;

        public CombatWidgetForm(ICombatDataSource combatData)
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
