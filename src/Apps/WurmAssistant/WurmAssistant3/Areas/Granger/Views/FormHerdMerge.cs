using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Views
{
    public partial class FormHerdMerge : ExtendedForm
    {
        private GrangerContext Context;
        private FormGrangerMain MainForm;
        private string SourceHerdName;
        readonly ILogger logger;

        public FormHerdMerge(GrangerContext context, FormGrangerMain mainForm, string sourceHerdName,
            [NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.Context = context;
            this.MainForm = mainForm;
            this.SourceHerdName = sourceHerdName;
            this.logger = logger;
            InitializeComponent();
            textBoxFromHerd.Text = SourceHerdName;
            comboBoxToHerd.Items.AddRange(Context.Herds.Where(x => x.HerdID != sourceHerdName).ToArray());
            listBoxFromHerd.Items.AddRange(Context.Creatures.Where(x => x.Herd == SourceHerdName).ToArray());
        }

        private void comboBoxToHerd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxToHerd.SelectedIndex > -1)
            {
                listBoxToHerd.Items.Clear();
                listBoxToHerd.Items.AddRange(
                    Context.Creatures.Where(x => x.Herd == comboBoxToHerd.SelectedItem.ToString()).ToArray());
                buttonOK.Enabled = true;
            }
            else buttonOK.Enabled = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                Context.MergeHerds(textBoxFromHerd.Text, comboBoxToHerd.Text);
            }
            catch (Exception _e)
            {
                MessageBox.Show("there was a problem with merging herds:\r\n" + _e.Message);
                if (_e is GrangerContext.DuplicateCreatureIdentityException)
                {
                    logger.Info(_e, "merging herds failed due non-unique creatures");
                }
                else
                {
                    logger.Error(_e, "merge herd problem");
                }
            }
        }

        private void listBoxFromHerd_DoubleClick(object sender, EventArgs e)
        {
            //TODO show creature info
        }

        private void listBoxToHerd_DoubleClick(object sender, EventArgs e)
        {
            //TODO show creature info
        }
    }
}
