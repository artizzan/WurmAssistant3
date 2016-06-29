using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormEditComments : ExtendedForm
    {
        private string CreatureComments;
        private FormGrangerMain formGrangerMain;

        public FormEditComments(FormGrangerMain formGrangerMain, string creatureComments, string creatureName)
        {
            this.formGrangerMain = formGrangerMain;
            this.CreatureComments = creatureComments;
            InitializeComponent();
            this.Text = "Edit comments for: " + creatureName;
            this.textBox1.Text = CreatureComments;
            textBox1.Select();
        }

        public string Result
        {
            get
            {
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    return textBox1.Text;
                }
                else throw new InvalidOperationException("Dialog result not OK");
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                buttonOK.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                buttonCancel.PerformClick();
            }
        }
    }
}
