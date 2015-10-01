﻿using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Features.Views;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Views;
using AldursLab.WurmAssistant3.Core.Areas.MainMenu.Views;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Views;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AldursLab.WurmAssistant3.Core.Root
{
    [PersistentObject("MainForm")]
    public partial class MainForm : PersistentForm, IUpdateLoop, IHostEnvironment, ISystemTrayContextMenu
    {
        [JsonObject(MemberSerialization.OptIn)]
        class Settings
        {
            [JsonProperty]
            readonly MainForm mainForm;

            [JsonProperty]
            int savedWidth;
            [JsonProperty]
            int savedHeight;
            [JsonProperty]
            bool baloonTrayTooltipShown;

            public Settings([NotNull] MainForm mainForm)
            {
                if (mainForm == null)
                    throw new ArgumentNullException("mainForm");
                this.mainForm = mainForm;
            }

            public int SavedWidth
            {
                get { return savedWidth; }
                set { savedWidth = value; mainForm.FlagAsChanged(); }
            }

            public int SavedHeight
            {
                get { return savedHeight; }
                set { savedHeight = value; mainForm.FlagAsChanged(); }
            }

            public bool BaloonTrayTooltipShown
            {
                get { return baloonTrayTooltipShown; }
                set { baloonTrayTooltipShown = value; mainForm.FlagAsChanged(); }
            }
        }

        class TrayManager
        {
            readonly MainForm mainForm;

            public TrayManager([NotNull] MainForm mainForm)
            {
                if (mainForm == null) throw new ArgumentNullException("mainForm");
                this.mainForm = mainForm;
            }

            public void SetupTrayContextMenu()
            {
                mainForm.systemTrayNotifyIcon.MouseClick += (sender, args) =>
                {
                    if (args.Button == MouseButtons.Left)
                    {
                        mainForm.ShowRestore(new object(), EventArgs.Empty);
                    }
                    else if (args.Button == MouseButtons.Right)
                    {
                        mainForm.TrayContextMenuStrip.Show();
                    }
                };

                var tsmi = new ToolStripMenuItem()
                {
                    Text = "Show Main Window"
                };
                tsmi.Click += mainForm.ShowRestore;
                mainForm.TrayContextMenuStrip.Items.Add(tsmi);
                mainForm.TrayContextMenuStrip.Items.Add(new ToolStripSeparator());
                mainForm.TrayContextMenuStrip.Items.Add(new ToolStripSeparator());
                var tsmi2 = new ToolStripMenuItem()
                {
                    Text = "Exit"
                };
                tsmi2.Click += (sender, args) => mainForm.Shutdown();
                mainForm.TrayContextMenuStrip.Items.Add(tsmi2);
            }

            public void AddMenuItem(string text, Action onClick, Image image)
            {
                // insert just above last separator and close option
                var insertionIndex = (mainForm.TrayContextMenuStrip.Items.Count - 2).EnsureNonNegative();
                var item = new ToolStripMenuItem()
                {
                    ImageAlign = ContentAlignment.MiddleLeft,
                    ImageScaling = ToolStripItemImageScaling.SizeToFit,
                    Image = image,
                    Text = text
                };
                item.Click += (sender, args) => onClick();
                mainForm.TrayContextMenuStrip.Items.Insert(insertionIndex, item);
            }

            public void HandleMinimizeToTray()
            {
                if (FormWindowState.Minimized == mainForm.WindowState)
                {
                    if (mainForm.wurmAssistantConfig.MinimizeToTrayEnabled)
                    {
                        if (!mainForm.settings.BaloonTrayTooltipShown)
                        {
                            mainForm.systemTrayNotifyIcon.ShowBalloonTip(5000);
                            mainForm.settings.BaloonTrayTooltipShown = true;
                        }
                        mainForm.Hide();
                    }
                }
            }
        }

        readonly CoreBootstrapper bootstrapper;
        bool bootstrapped = false;

        readonly IWurmAssistantConfig wurmAssistantConfig;

        readonly MinimizationManager minimizationManager;
        readonly TrayManager trayManager;

        [JsonProperty]
        readonly Settings settings;

        bool persistentStateLoaded;

        public MainForm()
        {
            InitializeComponent();

            settings = new Settings(this);

            minimizationManager = new MinimizationManager(this);
            trayManager = new TrayManager(this);

            bootstrapper = new CoreBootstrapper(this);
            wurmAssistantConfig = bootstrapper.WurmAssistantConfig;
            InitTimer.Enabled = true;

            trayManager.SetupTrayContextMenu();

            systemTrayNotifyIcon.Visible = true;
        }

        public void AfterPersistentStateLoaded()
        {
            persistentStateLoaded = true;
            RestoreSizeFromSaved();
        }

        void RestoreSizeFromSaved()
        {
            if (settings.SavedWidth != default(int) && settings.SavedHeight != default(int))
            {
                this.Size = new Size()
                {
                    Height = settings.SavedHeight,
                    Width = settings.SavedWidth
                };
                //this.Width = settings.SavedWidth;
                //this.Height = settings.SavedHeight;
            }
        }

        public void SetLogView(LogView logView)
        {
            LogViewPanel.Controls.Clear();
            logView.Dock = DockStyle.Fill;
            LogViewPanel.Controls.Add(logView);
        }

        public void SetModulesView(FeaturesView featuresView)
        {
            ModulesViewPanel.Controls.Clear();
            featuresView.Dock = DockStyle.Top;
            ModulesViewPanel.Controls.Add(featuresView);
        }

        public void SetMenuView(MenuView menuView)
        {
            MenuViewPanel.Controls.Clear();
            menuView.Dock = DockStyle.Fill;
            MenuViewPanel.Controls.Add(menuView);
        }

        private void InitTimer_Tick(object sender, EventArgs e)
        {
            if (!bootstrapped)
            {
                InitTimer.Enabled = false;

                try
                {
                    bootstrapper.Bootstrap();
                }
                catch (Exception exception)
                {
                    var view = new UniversalTextDisplayView()
                    {
                        Text = "OH NO!!",
                        ContentText = 
                            "Application startup was interrupted by an ugly error! " + Environment.NewLine
                            + Environment.NewLine + exception.ToString()
                    };
                    view.ShowDialog();
                    Shutdown();
                    return;
                }

                bootstrapped = true;
                UpdateTimer.Enabled = true;
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // guard against running update on scheduled ticks, after beginning shutdown
            if (!AppClosing)
            {
                OnUpdate();
            }
        }

        public event EventHandler<EventArgs> Updated;

        void OnUpdate()
        {
            var handler = Updated;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateTimer.Enabled = false;
        }

        public event EventHandler<EventArgs> HostClosing;

        bool AppClosing { get; set; }
        bool IHostEnvironment.Closing
        {
            get { return this.AppClosing; }
        }

        public void Restart()
        {
            AppClosing = true;
            OnHostClosing();
            Application.Restart();
        }

        public void Shutdown()
        {
            AppClosing = true;
            OnHostClosing();
            Application.Exit();
        }

        public Platform Platform { get { return Platform.Unknown;} }

        protected virtual void OnHostClosing()
        {
            var handler = HostClosing;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void MainView_FormClosed(object sender, FormClosedEventArgs e)
        {
            bootstrapper.Dispose();
        }

        public void AddMenuItem(string text, Action onClick, Image image)
        {
            trayManager.AddMenuItem(text, onClick, image);
        }

        void ShowRestore(object sender, EventArgs e)
        {
            minimizationManager.Restore();
            systemTrayNotifyIcon.Visible = true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                if (persistentStateLoaded)
                {
                    settings.SavedWidth = this.Width;
                    settings.SavedHeight = this.Height;
                }
            }
            trayManager.HandleMinimizeToTray();
        }
    }
}
