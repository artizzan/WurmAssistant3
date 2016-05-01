using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Views
{
    public partial class VisualCppMissingHelperView : Form
    {
        readonly Exception exception;

        public VisualCppMissingHelperView([NotNull] Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            this.exception = exception;
            InitializeComponent();

            SystemSounds.Exclamation.Play();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string link = "https://www.microsoft.com/en-us/download/details.aspx?id=8328";
            try
            {
                Process.Start(link);
            }
            catch (Exception ohcrap)
            {
                MessageBox.Show(
                    $"I'm sorry, failed to start process for link: {link}. Batman can't help this. Error: {ohcrap}",
                    "Oops",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(exception.ToString());
        }
    }
}
