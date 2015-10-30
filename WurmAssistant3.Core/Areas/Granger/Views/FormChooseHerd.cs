using System;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
{
    public partial class FormChooseHerd : ExtendedForm
    {
        private GrangerContext Context;
        private UCGrangerHorseList ControlGrangerHorseList;
        private FormGrangerMain MainForm;
        public string Result
        {
            get
            {
                if (listBox1.SelectedItem == null) return null;
                return listBox1.SelectedItem.ToString();
            }
        }

        public FormChooseHerd(FormGrangerMain mainForm, GrangerContext Context)
        {
            this.MainForm = mainForm;
            this.Context = Context;
            InitializeComponent();

            var herds = Context.Herds.ToArray();

            listBox1.Items.AddRange(herds);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                buttonOK.Enabled = true;
            }
            else
            {
                buttonOK.Enabled = false;
            }
        }

    }
}
