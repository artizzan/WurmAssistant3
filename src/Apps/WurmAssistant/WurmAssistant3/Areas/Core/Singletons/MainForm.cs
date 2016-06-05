using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Transients;
using AldursLab.WurmAssistant3.Areas.MainMenu.Singletons;
using AldursLab.WurmAssistant3.Areas.Native.Constants;
using AldursLab.WurmAssistant3.Properties;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Core.Singletons
{
    [PersistentObject("MainForm")]
    public partial class MainForm : PersistentForm
    {
        readonly IConsoleArgs consoleArgs;
        readonly CombinedLogsUserControl combinedLogsUserControl;
        readonly MainMenuUserControl mainMenuUserControl;
        readonly IFeaturesManager featuresManager;
        readonly IWaVersionInfoProvider waVersionInfoProvider;
        readonly IChangelogManager changelogManager;
        readonly IUserNotifier userNotifier;
        readonly ILogger logger;
        readonly MouseDragManager mouseDragManager;
        readonly MinimizationManager minimizationManager;

        [JsonProperty] readonly Settings settings;
        
        bool persistentStateLoaded;

        public MainForm(
            [NotNull] IConsoleArgs consoleArgs,
            [NotNull] CombinedLogsUserControl combinedLogsUserControl,
            [NotNull] MainMenuUserControl mainMenuUserControl,
            [NotNull] IFeaturesManager featuresManager,
            [NotNull] ISystemTrayContextMenu systemTrayContextMenu,
            [NotNull] IWaVersionInfoProvider waVersionInfoProvider,
            [NotNull] IChangelogManager changelogManager,
            [NotNull] IUserNotifier userNotifier,
            [NotNull] ILogger logger)
        {
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            if (combinedLogsUserControl == null) throw new ArgumentNullException(nameof(combinedLogsUserControl));
            if (mainMenuUserControl == null) throw new ArgumentNullException(nameof(mainMenuUserControl));
            if (featuresManager == null) throw new ArgumentNullException(nameof(featuresManager));
            if (systemTrayContextMenu == null) throw new ArgumentNullException(nameof(systemTrayContextMenu));
            if (waVersionInfoProvider == null) throw new ArgumentNullException(nameof(waVersionInfoProvider));
            if (changelogManager == null) throw new ArgumentNullException(nameof(changelogManager));
            if (userNotifier == null) throw new ArgumentNullException(nameof(userNotifier));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.consoleArgs = consoleArgs;
            this.combinedLogsUserControl = combinedLogsUserControl;
            this.mainMenuUserControl = mainMenuUserControl;
            this.featuresManager = featuresManager;
            this.waVersionInfoProvider = waVersionInfoProvider;
            this.changelogManager = changelogManager;
            this.userNotifier = userNotifier;
            this.logger = logger;

            InitializeComponent();

            settings = new Settings();
            minimizationManager = new MinimizationManager(this);
            mouseDragManager = new MouseDragManager(this);
            mouseDragManager.Hook();

            systemTrayContextMenu.ShowMainWindowClicked += ShowRestore;
            systemTrayContextMenu.ExitWurmAssistantClicked += (sender, eventArgs) => this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            persistentStateLoaded = true;
            RestoreSizeFromSaved();

            Text += string.Format(" ({0})", waVersionInfoProvider.Get());

            if (consoleArgs.WurmUnlimitedMode)
            {
                this.Text = "Wurm Assistant Unlimited";
                this.Icon = Resources.WurmAssistantUnlimitedIcon;
            }
            else
            {
                this.Text = "Wurm Assistant";
            }

            SetupFeaturesManager();
            SetupLogView();
            SetupMenuView();

            ShowChangelog();
        }

        void ShowChangelog()
        {
            try
            {
                var changes = changelogManager.GetNewChanges();
                if (!string.IsNullOrWhiteSpace(changes))
                {
                    changelogManager.ShowChanges(changes);
                    changelogManager.UpdateLastChangeDate();
                }
            }
            catch (Exception exception)
            {
                logger.Warn(exception, "Error at parsing or opening changelog");
                userNotifier.NotifyWithMessageBox("Error opening changelog, see logs for details.", NotifyKind.Warning);
            }
        }

        protected override void OnPersistentDataLoaded()
        {
            settings.MainForm = this;
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
            }
        }

        public void SetupLogView()
        {
            LogViewPanel.Controls.Clear();
            combinedLogsUserControl.Dock = DockStyle.Fill;
            LogViewPanel.Controls.Add(combinedLogsUserControl);
        }

        public void SetupFeaturesManager()
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

        public void SetupMenuView()
        {
            MenuViewPanel.Controls.Clear();
            mainMenuUserControl.Dock = DockStyle.Fill;
            MenuViewPanel.Controls.Add(mainMenuUserControl);
        }

        void ShowRestore(object sender, EventArgs e)
        {
            minimizationManager.Restore();
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
        }

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
