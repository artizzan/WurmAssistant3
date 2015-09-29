using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public partial class UpdaterGui : UserControl, IGui, IProgressReporter
    {
        readonly IGuiHost host;
        readonly IDebug debug;

        delegate void InvokeDelegate();

        bool skipEnabled = false;

        class OutputMessage
        {
            public string Content { get; set; }
            public Color? Color { get; set; }
        }

        List<OutputMessage> messages = new List<OutputMessage>(); 

        public Action SkipAction { get; set; }

        public UpdaterGui()
        {
            InitializeComponent();
        }

        public UpdaterGui(IGuiHost host, [NotNull] IDebug debug)
            : this()
        {
            if (host == null) throw new ArgumentNullException("host");
            if (debug == null) throw new ArgumentNullException("debug");
            this.host = host;
            this.debug = debug;
        }

        public void ShowGui()
        {
            ThreadSafeInvoke(() => host.ShowHostWindow());
        }

        public void SetProgressStatus(string message)
        {
            ThreadSafeInvoke(() => AddUserMessage(message));
        }

        public void SetProgressPercent(byte? percent)
        {
            ThreadSafeInvoke(() =>
            {
                if (percent == null)
                {
                    progressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    if (percent > 100)
                        percent = 100;
                    progressBar.Style = ProgressBarStyle.Continuous;
                    progressBar.Value = percent.Value;
                }
            });
        }

        public void AddUserMessage(string message, Color? textColor = null)
        {
            ThreadSafeInvoke(() =>
            {
                debug.Write(message + Environment.NewLine);
                var newString = DateTime.Now + " > " + message + Environment.NewLine;
                messages.Add(new OutputMessage()
                {
                    Content = newString,
                    Color = textColor
                });
                
                RebuildOutput();
            });
        }

        void RebuildOutput()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var outputMessage in messages)
            {
                sb.Append(outputMessage.Content);
            }
            richTextBox1.Text = sb.ToString();
            int index = 0;
            foreach (var outputMessage in messages)
            {
                if (outputMessage.Color.HasValue)
                {
                    richTextBox1.Select(index, outputMessage.Content.Length - 1);
                    richTextBox1.SelectionColor = outputMessage.Color.Value;
                }
                index += outputMessage.Content.Length - 1;
            }
            richTextBox1.Select(index, 0);
        }

        public void HideGui()
        {
            ThreadSafeInvoke(() => host.HideHostWindow());
        }

        public void SetState(LauncherState state)
        {
            ThreadSafeInvoke(() =>
            {
                if (state == LauncherState.Error)
                {
                    if (progressBar.Style == ProgressBarStyle.Marquee)
                    {
                        progressBar.Style = ProgressBarStyle.Blocks;
                        progressBar.Value = 0;
                    }
                    label1.ForeColor = Color.Red;
                    label1.Text = "Launcher has encountered an error";
                }
            });
        }

        public void EnableSkip()
        {
            skipEnabled = true;
        }

        public void DisableSkip()
        {
            skipEnabled = false;
            SkipUpdateBtn.Visible = false;
        }

        private void ThreadSafeInvoke(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new InvokeDelegate(action));
            }
            else
            {
                action();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (skipEnabled && SkipAction != null)
            {
                SkipUpdateBtn.Visible = true;
            }
        }

        private void SkipUpdateBtn_Click(object sender, EventArgs e)
        {
            if (SkipAction != null)
            {
                SkipAction();
            }
        }
    }
}
