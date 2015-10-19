using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Serialization;

namespace AldursLab.WurmAssistant3.Core.Areas.Persistence.Views
{
    public partial class DeserializationErrorResolverView : Form
    {
        readonly object o;
        readonly ErrorEventArgs args;

        public DeserializationErrorResolverView(object o, ErrorEventArgs args)
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
