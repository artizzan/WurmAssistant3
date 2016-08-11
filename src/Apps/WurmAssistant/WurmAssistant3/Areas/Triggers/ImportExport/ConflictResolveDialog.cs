using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    public partial class ConflictResolveDialog : Form
    {
        public ConflictResolveDialog()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            labelText.Text = text;
        }

        public ConflictResolution ConflictResolution { get; set; } = ConflictResolution.Skip;
        public bool UseThisResolutionForAllConflicts => checkBoxDoForAll.Checked;

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            SetResult(ConflictResolution.Skip);
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            SetResult(ConflictResolution.Replace);
        }

        private void buttonImportAsNew_Click(object sender, EventArgs e)
        {
            SetResult(ConflictResolution.ImportAsNew);
        }

        void SetResult(ConflictResolution conflictResolution)
        {
            ConflictResolution = conflictResolution;
            this.DialogResult = DialogResult.OK;
        }
    }
}
