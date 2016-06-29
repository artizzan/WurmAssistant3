using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Properties;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    [KernelBind(BindingHint.Singleton)]
    class TrayMenu : ISystemTrayContextMenu, IDisposable
    {
        readonly NotifyIcon notifyIcon;
        readonly ContextMenuStrip contextMenuStrip;

        public event EventHandler<EventArgs> ExitWurmAssistantClicked;
        public event EventHandler<EventArgs> ShowMainWindowClicked;

        public TrayMenu(IConsoleArgs consoleArgs)
        {
            contextMenuStrip = new ContextMenuStrip();
            notifyIcon = new NotifyIcon
            {
                BalloonTipText = "I\'ll be here if you need me!",
                BalloonTipTitle = "Wurm Assistant 3",
                ContextMenuStrip = contextMenuStrip,
                Icon = Resources.WurmAssistantIcon,
                Text = "Wurm Assistant 3",
                Visible = true
            };

            if (consoleArgs.WurmUnlimitedMode)
            {
                notifyIcon.BalloonTipTitle = "Wurm Assistant 3 Unlimited";
                notifyIcon.Text = "Wurm Assistant 3 Unlimited";
                notifyIcon.Icon = Resources.WurmAssistantUnlimitedIcon;
            }

            notifyIcon.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    OnShowMainWindowClicked();
                }
                else if (args.Button == MouseButtons.Right)
                {
                    contextMenuStrip.Show();
                }
            };

            var tsmi = new ToolStripMenuItem()
            {
                Text = "Show Main Window"
            };
            tsmi.Click += (sender, args) => OnShowMainWindowClicked();
            contextMenuStrip.Items.Add(tsmi);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            var tsmi2 = new ToolStripMenuItem()
            {
                Text = "Exit"
            };
            tsmi2.Click += (sender, args) => OnExitWurmAssistantClicked();
            contextMenuStrip.Items.Add(tsmi2);
        }

        public void AddMenuItem(string text, Action onClick, Image image)
        {
            // insert just above last separator and close option
            var insertionIndex = (contextMenuStrip.Items.Count - 2).EnsureNonNegative();
            var item = new ToolStripMenuItem()
            {
                ImageAlign = ContentAlignment.MiddleLeft,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = image,
                Text = text
            };
            item.Click += (sender, args) => onClick();
            contextMenuStrip.Items.Insert(insertionIndex, item);
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }

        protected virtual void OnExitWurmAssistantClicked()
        {
            ExitWurmAssistantClicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnShowMainWindowClicked()
        {
            ShowMainWindowClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
