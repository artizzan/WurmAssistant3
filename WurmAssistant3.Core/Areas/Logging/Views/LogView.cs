using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Logging.ViewModels;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Views
{
    public partial class LogView : UserControl
    {
        readonly LogViewModel logViewModel;
        bool logChanged = false;

        public LogView([NotNull] LogViewModel logViewModel) 
        {
            if (logViewModel == null) throw new ArgumentNullException("logViewModel");
            this.logViewModel = logViewModel;

            InitializeComponent();

            logViewModel.Messages.CollectionChanged += (sender, args) =>
            {
                logChanged = true;
            };

            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (logChanged)
            {
                textBox1.Text = string.Join("\r\n", logViewModel.Messages);
                logChanged = false;
                textBox1.SelectionStart = textBox1.Text.Length - 1;
                textBox1.SelectionLength = 0;
                textBox1.ScrollToCaret();
            }
        }
    }
}
