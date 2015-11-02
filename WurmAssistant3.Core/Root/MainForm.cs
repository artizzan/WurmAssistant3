using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Views;
using AldursLab.WurmAssistant3.Core.Areas.MainMenu.Views;
using AldursLab.WurmAssistant3.Core.Native.Win32;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.Root.Components;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Views;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Root
{
    [PersistentObject("MainForm")]
    public partial class MainForm : PersistentForm, IUpdateLoop, IHostEnvironment, ISystemTrayContextMenu
    {
        readonly MouseDragManager mouseDragManager;

        [JsonObject(MemberSerialization.OptIn)]
        class Settings
        {
            public MainForm MainForm { get; set; }

            [JsonProperty]
            int savedWidth;
            [JsonProperty]
            int savedHeight;
            [JsonProperty]
            bool baloonTrayTooltipShown;

            public int SavedWidth
            {
                get { return savedWidth; }
                set { savedWidth = value; MainForm.FlagAsChanged(); }
            }

            public int SavedHeight
            {
                get { return savedHeight; }
                set { savedHeight = value; MainForm.FlagAsChanged(); }
            }

            public bool BaloonTrayTooltipShown
            {
                get { return baloonTrayTooltipShown; }
                set { baloonTrayTooltipShown = value; MainForm.FlagAsChanged(); }
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

        readonly ILogger logger;

        readonly CoreBootstrapper bootstrapper;
        bool bootstrapped = false;

        readonly IWurmAssistantConfig wurmAssistantConfig;

        readonly MinimizationManager minimizationManager;
        readonly TrayManager trayManager;

        [JsonProperty]
        readonly Settings settings;

        bool persistentStateLoaded;

        public MainForm(string[] args)
        {
            InitializeComponent();

            var argsManager = new ConsoleArgsManager(args);
            if (argsManager.WurmUnlimitedMode)
            {
                this.Text = "Wurm Assistant Unlimited";
                this.Icon = Resources.WurmAssistantUnlimitedIcon;
                this.systemTrayNotifyIcon.Icon = Resources.WurmAssistantUnlimitedIcon;
            }
            else
            {
                this.Text = "Wurm Assistant";
            }

            mouseDragManager = new MouseDragManager(this);
            settings = new Settings();

            minimizationManager = new MinimizationManager(this);
            trayManager = new TrayManager(this);

            bootstrapper = new CoreBootstrapper(this, argsManager);
            wurmAssistantConfig = bootstrapper.WurmAssistantConfig;
            logger = bootstrapper.GetCoreLogger();
            InitTimer.Enabled = true;

            trayManager.SetupTrayContextMenu();

            systemTrayNotifyIcon.Visible = true;
        }

        protected override void OnPersistentDataLoaded()
        {
            settings.MainForm = this;
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

        public void SetFeaturesManager(IFeaturesManager featuresManager)
        {
            featuresFlowPanel.Controls.Clear();

            var features = featuresManager.Features;

            foreach (var f in features)
            {
                var feature = f;
                Button btn = new Button
                {
                    Size = new Size(80, 80),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    BackgroundImage = feature.Icon
                };

                btn.Click += (o, args) => feature.Show();
                toolTips.SetToolTip(btn, feature.Name);
                featuresFlowPanel.Controls.Add(btn);
            }
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
                catch (ConfigCancelledException)
                {
                    Shutdown();
                }
                catch (Exception exception)
                {
                    bool restart = false;
                    var btn1 = new Button()
                    {
                        Text = "Reset Wurm Assistant config",
                        Height = 28,
                        Width = 220
                    };
                    btn1.Click += (o, args) =>
                    {
                        if (bootstrapper.TryResetConfig())
                        {
                            MessageBox.Show("Reset complete, please restart.", "Done", MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show("Reset was not possible.", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                    var btn2 = new Button()
                    {
                        Text = "Restart Wurm Assistant",
                        Height = 28,
                        Width = 220,
                        DialogResult = DialogResult.OK
                    };
                    btn2.Click += (o, args) => restart = true;
                    var view = new UniversalTextDisplayView(btn2, btn1)
                    {
                        Text = "OH NO!!",
                        ContentText =
                            "Application startup was interrupted by an ugly error! " + Environment.NewLine
                            + Environment.NewLine + exception.ToString()
                    };
                    view.ShowDialog();
                    if (restart) Restart();
                    else Shutdown();
                    return;
                }
                finally
                {
                    mouseDragManager.Hook();
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
            OnHostClosing();
            UpdateTimer.Enabled = false;
        }

        public event EventHandler<EventArgs> HostClosing;
        public event EventHandler<EventArgs> LateHostClosing;

        bool AppClosing { get; set; }
        bool IHostEnvironment.Closing
        {
            get { return this.AppClosing; }
        }

        public void Restart()
        {
            OnHostClosing();
            Application.Restart();
        }

        public void Shutdown()
        {
            OnHostClosing();
            Application.Exit();
        }

        public Platform Platform { get { return Platform.Unknown;} }

        protected virtual void OnHostClosing()
        {
            if (AppClosing) return;

            try
            {
                AppClosing = true;
                var handler = HostClosing;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "HostClosing event has thrown an unhandled exception!");
            }
            finally
            {
                OnLateHostClosing();
            }
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

        protected virtual void OnLateHostClosing()
        {
            var handler = LateHostClosing;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        class MouseDragManager
        {
            [DllImport("user32.dll")]
            private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
            [DllImport("user32.dll")]
            private static extern bool ReleaseCapture();


            readonly MainForm mainForm;

            public MouseDragManager([NotNull] MainForm mainForm)
            {
                if (mainForm == null) throw new ArgumentNullException("mainForm");
                this.mainForm = mainForm;
            }

            public void Hook()
            {
                var logViewButtonsPanel = mainForm.LogViewPanel.Controls.Find("logViewButtonsFlowPanel", true).FirstOrDefault();

                WireControls(mainForm.featuresFlowPanel,
                    mainForm.LogViewPanel,
                    mainForm.MenuViewPanel,
                    logViewButtonsPanel);
            }

            void WireControls(params object[] objects)
            {
                foreach (var obj in objects)
                {
                    var control = obj as Control;
                    if (control != null)
                    {
                        control.MouseDown += OnMouseDownHandler;
                    }
                }
            }

            void OnMouseDownHandler(object sender, MouseEventArgs mouseEventArgs)
            {
                if (mouseEventArgs.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(mainForm.Handle, Win32Hooks.WM_NCLBUTTONDOWN, Win32Hooks.HT_CAPTION, 0);
                }
            }
        }
    }
}
