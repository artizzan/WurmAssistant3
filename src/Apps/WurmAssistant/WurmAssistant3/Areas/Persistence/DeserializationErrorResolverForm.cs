using System;
using System.Windows.Forms;
using Newtonsoft.Json.Serialization;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    public partial class DeserializationErrorResolverForm : Form
    {
        readonly object o;
        readonly ErrorEventArgs args;

        public DeserializationErrorResolverForm(object o, ErrorEventArgs args)
        {
            this.o = o;
            this.args = args;

            InitializeComponent();

            ErrorTextTb.Text = args.ErrorContext.Error.ToString();
        }

        private void IgnoreBtn_Click(object sender, EventArgs e)
        {
            args.ErrorContext.Handled = true;
            this.DialogResult = DialogResult.OK;
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
