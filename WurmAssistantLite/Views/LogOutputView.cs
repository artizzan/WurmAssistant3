using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.ViewModels;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistantLite.Views
{
    public partial class LogOutputView : UserControl
    {
        readonly LogOutputViewModel logOutputViewModel;
        bool logChanged = false;

        public LogOutputView()
        {
            InitializeComponent();
        }

        public LogOutputView([NotNull] LogOutputViewModel logOutputViewModel) 
            : this()
        {
            if (logOutputViewModel == null) throw new ArgumentNullException("logOutputViewModel");
            this.logOutputViewModel = logOutputViewModel;

            logOutputViewModel.Messages.CollectionChanged += (sender, args) =>
            {
                logChanged = true;
            };

            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (logChanged)
            {
                textBox1.Text = string.Join("\r\n", logOutputViewModel.Messages);
                logChanged = false;
                textBox1.SelectionStart = textBox1.Text.Length - 1;
                textBox1.SelectionLength = 0;
                textBox1.ScrollToCaret();
            }
        }
    }
}
