using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Calendar
{
    public partial class SeasonsEditForm : Form
    {
        readonly WurmSeasonsManager seasonsManager;
        readonly BindingList<WurmSeasonDefinition> tempSeasons;

        public SeasonsEditForm([NotNull] WurmSeasonsManager seasonsManager)
        {
            this.seasonsManager = seasonsManager ?? throw new ArgumentNullException(nameof(seasonsManager));
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
            foreach (var item in tempSeasons.Where(definition => definition.IsDefault).ToArray())
            {
                tempSeasons.Remove(item);
            }

            foreach (var wurmSeasonDefinition in seasonsManager.GetDefaults())
            {
                tempSeasons.Add(wurmSeasonDefinition);
            }
        }
    }
}
