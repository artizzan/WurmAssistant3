using System;
using System.Drawing;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public partial class UpdaterGui : UserControl, IGui, IProgressReporter
    {
        readonly IGuiHost host;

        public UpdaterGui()
        {
            InitializeComponent();
        }

        public UpdaterGui(IGuiHost host)
            : this()
        {
            if (host == null) throw new ArgumentNullException("host");
            this.host = host;
        }

        public void ShowGui()
        {
            host.ShowHostWindow();
        }

        public void SetProgressStatus(string message)
        {
            AddUserMessage(message);
        }

        public void SetProgressPercent(byte? percent)
        {
            if (percent == null)
            {
                progressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                if (percent > 100) percent = 100;
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = percent.Value;
            }
        }

        public void AddUserMessage(string message)
        {
            textBox.Text = textBox.Text + "\r\n" + DateTime.Now + " > " + message;
            textBox.SelectionStart = textBox.Text.Length == 0 ? 0 : textBox.Text.Length - 1;
            textBox.SelectionLength = 0;
            textBox.ScrollToCaret();
        }

        public void HideGui()
        {
            host.HideHostWindow();
        }

        public void SetState(LauncherState state)
        {
            if (state == LauncherState.Error)
            {
                label1.Text = "Error";
                label1.ForeColor = Color.Red;
                if (progressBar.Style == ProgressBarStyle.Marquee)
                {
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Value = 0;
                }
            }
        }
    }
}
