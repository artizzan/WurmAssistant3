using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
{
    public partial class FormEditComments : ExtendedForm
    {
        private string HorseComments;
        private FormGrangerMain formGrangerMain;

        public FormEditComments(FormGrangerMain formGrangerMain, string horseComments, string horseName)
        {
            this.formGrangerMain = formGrangerMain;
            this.HorseComments = horseComments;
            InitializeComponent();
            this.Text = "Edit comments for: " + horseName;
            this.textBox1.Text = HorseComments;
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
