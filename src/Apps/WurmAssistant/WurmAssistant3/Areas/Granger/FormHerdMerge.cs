using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormHerdMerge : ExtendedForm
    {
        private readonly GrangerContext context;
        private FormGrangerMain mainForm;
        private readonly string sourceHerdName;
        readonly ILogger logger;

        public FormHerdMerge(
            [NotNull] GrangerContext context,
            [NotNull] FormGrangerMain mainForm,
            [NotNull] string sourceHerdName,
            [NotNull] ILogger logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            if (sourceHerdName == null) throw new ArgumentNullException(nameof(sourceHerdName));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.context = context;
            this.mainForm = mainForm;
            this.sourceHerdName = sourceHerdName;
            this.logger = logger;

            InitializeComponent();

            textBoxFromHerd.Text = this.sourceHerdName;
            comboBoxToHerd.Items.AddRange(
                this.context.Herds.Where(x => x.HerdID != sourceHerdName).Cast<object>().ToArray());
            listBoxFromHerd.Items.AddRange(
                this.context.Creatures.Where(x => x.Herd == this.sourceHerdName).Cast<object>().ToArray());
        }

        private void comboBoxToHerd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxToHerd.SelectedIndex > -1)
            {
                listBoxToHerd.Items.Clear();
                listBoxToHerd.Items.AddRange(
                    context.Creatures.Where(x => x.Herd == comboBoxToHerd.SelectedItem.ToString())
                           .Cast<object>()
                           .ToArray());
                buttonOK.Enabled = true;
            }
            else buttonOK.Enabled = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                context.MergeHerds(textBoxFromHerd.Text, comboBoxToHerd.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Merging herds failed:\r\n{exception.Message}");
                if (exception is GrangerContext.DuplicateCreatureIdentityException)
                {
                    logger.Info(exception, "merging herds failed due to non-unique creature identities");
                }
                else
                {
                    logger.Error(exception, "merging herds failed");
                }
            }
        }
    }
}
