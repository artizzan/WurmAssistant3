using System;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormChooseHerd : ExtendedForm
    {
        public FormChooseHerd([NotNull] GrangerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            InitializeComponent();

            listBox1.Items.AddRange(context.Herds.Cast<object>().ToArray());
        }

        public string Result => listBox1.SelectedItem?.ToString();

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = listBox1.SelectedItem != null;
        }
    }
}
