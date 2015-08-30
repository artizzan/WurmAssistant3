using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public partial class UpdaterGui : UserControl, IGui, IProgressReporter
    {
        // important: trying to set .Text property on TextBox or Label results in hangs and crashes under mono (ubuntu).
        // I have no clue why, it might be related in dynamically adding UserControl to the Host.
        // I think the simplest fix is to avoid doing this at all at runtime.

        readonly IGuiHost host;
        readonly IDebug debug;

        public UpdaterGui()
        {
            InitializeComponent();
            listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
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
                if (percent > 100)
                    percent = 100;
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = percent.Value;
            }
        }

        public void AddUserMessage(string message)
        {
            debug.Write(message + "\r\n");
            listBox.Items.Add(DateTime.Now + " > " + message);
        }

        public void HideGui()
        {
            host.HideHostWindow();
        }

        public void SetState(LauncherState state)
        {
            if (state == LauncherState.Error)
            {
                if (progressBar.Style == ProgressBarStyle.Marquee)
                {
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Value = 0;
                }
            }
        }

        private void listBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index > -1)
            {
                e.ItemHeight = (int)e.Graphics.MeasureString(listBox.Items[e.Index].ToString(), listBox.Font, listBox.Width).Height;
            }
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                e.Graphics.DrawString(listBox.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
            }
        }
    }
}
