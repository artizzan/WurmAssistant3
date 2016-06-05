using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Csv;
using AldursLab.WurmAssistant3.Areas.Triggers.Contracts.ActionQueueParsing;
using AldursLab.WurmAssistant3.Areas.Triggers.Singletons;
using AldursLab.WurmAssistant3.Properties;
using AldursLab.WurmAssistant3.Utils.WinForms.Reusables;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Parts.ActionQueueParsing
{
    public partial class EditGui : Form
    {
        readonly ConditionsManager conditionsManager;
        BindingList<Condition> conditions;

        public EditGui([NotNull] ConditionsManager conditionsManager)
        {
            if (conditionsManager == null) throw new ArgumentNullException("conditionsManager");
            this.conditionsManager = conditionsManager;
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;
            ConditionKind.DataSource = Enum.GetValues(typeof (ConditionKind));
            MatchingKind.DataSource = Enum.GetValues(typeof (MatchingKind));

            BuildGrid();
        }

        void BuildGrid()
        {
            conditions =
                new BindingList<Condition>(
                    conditionsManager.GetCurrentConditionsCopies()
                                     .OrderBy(condition => condition.ConditionKind)
                                     .ThenBy(condition => condition.MatchingKind)
                                     .ThenBy(condition => condition.Pattern)
                                     .ToList());
            dataGridView1.DataSource = conditions;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void restoreDefaultsButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will remove all customization and restore default list of conditions. Continue?",
                "Confirm",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation, 
                MessageBoxDefaultButton.Button3) == DialogResult.Yes)
            {
                conditionsManager.RestoreDefaults();
                BuildGrid();
            }
        }

        public IEnumerable<Condition> GetEditedConditions()
        {
            return conditions.ToArray();
        }

        private void helpBtn_Click(object sender, EventArgs e)
        {
            var form = new UniversalTextDisplayView
            {
                Text = "Action Queue Trigger Modding Help",
                ContentText = Resources.ActionQueueModdingHelp
            };
            form.ShowCenteredOnForm(this);
        }

        private void toTxtBtn_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                DefaultExt = "txt",
                Filter = "Text Files (*.txt)|*.txt",
                AddExtension = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, BuildTxt());
                try
                {
                    Process.Start(dialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }
        }

        string BuildTxt()
        {
            var csvBuilder = new EnumerableToCsvBuilder<Condition>(conditions);
            csvBuilder.AddMapping("ConditionId", condition => condition.ConditionId.ToString());
            csvBuilder.AddMapping("Default", condition => condition.Default.ToString());
            csvBuilder.AddMapping("Disabled", condition => condition.Disabled.ToString());
            csvBuilder.AddMapping("Pattern", condition => condition.Pattern.ToString());
            csvBuilder.AddMapping("ConditionKind", condition => condition.ConditionKind.ToString());
            csvBuilder.AddMapping("MatchingKind", condition => condition.MatchingKind.ToString());
            var output = csvBuilder.BuildCsv();
            return output;
        }
    }
}
