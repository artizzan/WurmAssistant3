using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Calendar.Contracts;
using AldursLab.WurmAssistant3.Areas.Calendar.Singletons;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Calendar.Parts
{
    public partial class SeasonsEditForm : Form
    {
        readonly WurmSeasonsManager seasonsManager;
        readonly BindingList<WurmSeasonDefinition> tempSeasons;

        public SeasonsEditForm([NotNull] WurmSeasonsManager seasonsManager)
        {
            if (seasonsManager == null) throw new ArgumentNullException("seasonsManager");
            this.seasonsManager = seasonsManager;
            InitializeComponent();

            tempSeasons = new BindingList<WurmSeasonDefinition>(seasonsManager.All.Select(
                definition => definition.CreateCopy()).ToList());

            dataGridView1.DataSource = tempSeasons;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            seasonsManager.ReplaceReasons(tempSeasons);
            this.DialogResult = DialogResult.OK;
        }

        private void restoreDefaultsButton_Click(object sender, EventArgs e)
        {
            tempSeasons.Clear();
            foreach (var wurmSeasonDefinition in seasonsManager.GetDefaults())
            {
                tempSeasons.Add(wurmSeasonDefinition);
            }
        }
    }
}
