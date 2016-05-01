using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Calendar.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Calendar.Modules;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Calendar.Views
{
    public partial class SeasonsEditView : Form
    {
        readonly WurmSeasonsManager seasonsManager;
        readonly BindingList<WurmSeasonDefinition> tempSeasons;

        public SeasonsEditView([NotNull] WurmSeasonsManager seasonsManager)
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
