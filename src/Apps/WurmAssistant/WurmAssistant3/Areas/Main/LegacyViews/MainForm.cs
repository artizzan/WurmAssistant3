using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Services;
using AldursLab.WurmAssistant3.Areas.MainMenu.Services;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main.LegacyViews
{
    [KernelBind(BindingHint.Singleton)]
    public partial class MainForm : UserControl
    {
        readonly CombinedLogsUserControl combinedLogsUserControl;
        readonly MainMenuUserControl mainMenuUserControl;
        readonly IFeaturesManager featuresManager;
        readonly IChangelogManager changelogManager;
        readonly IUserNotifier userNotifier;
        readonly ILogger logger;

        public MainForm(
            [NotNull] IConsoleArgs consoleArgs,
            [NotNull] CombinedLogsUserControl combinedLogsUserControl,
            [NotNull] MainMenuUserControl mainMenuUserControl,
            [NotNull] IFeaturesManager featuresManager,
            [NotNull] IChangelogManager changelogManager,
            [NotNull] IUserNotifier userNotifier,
            [NotNull] ILogger logger)
        {
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            if (combinedLogsUserControl == null) throw new ArgumentNullException(nameof(combinedLogsUserControl));
            if (mainMenuUserControl == null) throw new ArgumentNullException(nameof(mainMenuUserControl));
            if (featuresManager == null) throw new ArgumentNullException(nameof(featuresManager));
            if (changelogManager == null) throw new ArgumentNullException(nameof(changelogManager));
            if (userNotifier == null) throw new ArgumentNullException(nameof(userNotifier));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.combinedLogsUserControl = combinedLogsUserControl;
            this.mainMenuUserControl = mainMenuUserControl;
            this.featuresManager = featuresManager;
            this.changelogManager = changelogManager;
            this.userNotifier = userNotifier;
            this.logger = logger;

            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
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
                    BackgroundImage = feature.Icon,
                    BackColor = Color.Gainsboro
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
    }
}
