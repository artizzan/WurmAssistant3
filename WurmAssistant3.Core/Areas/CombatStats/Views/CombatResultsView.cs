using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Views
{
    public partial class CombatResultsView : ExtendedForm
    {
        readonly ICombatDataSource combatDataSource;

        public CombatResultsView(ICombatDataSource combatDataSource)
        {
            if (combatDataSource == null) throw new ArgumentNullException("combatDataSource");
            this.combatDataSource = combatDataSource;
            InitializeComponent();

            combatDataSource.DataChanged += CombatResultsOnDataChanged;
        }

        void CombatResultsOnDataChanged(object sender, EventArgs eventArgs)
        {
            // todo
        }

        private void CombatResultsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            combatDataSource.DataChanged -= CombatResultsOnDataChanged;
        }
    }
}
