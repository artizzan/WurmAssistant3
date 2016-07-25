using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Insights;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.MainMenu;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main
{
    [KernelBind(BindingHint.Singleton)]
    public partial class MainForm : UserControl
    {
        readonly CombinedLogsUserControl combinedLogsUserControl;
        readonly MainMenuUserControl mainMenuUserControl;
        readonly IFeaturesManager featuresManager;
        readonly ITelemetry telemetry;


        public MainForm(
            [NotNull] IConsoleArgs consoleArgs,
            [NotNull] CombinedLogsUserControl combinedLogsUserControl,
            [NotNull] MainMenuUserControl mainMenuUserControl,
            [NotNull] IFeaturesManager featuresManager,
            [NotNull] INewsViewModelFactory newsViewModelFactory,
            [NotNull] ITelemetry telemetry)
        {
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            if (combinedLogsUserControl == null) throw new ArgumentNullException(nameof(combinedLogsUserControl));
            if (mainMenuUserControl == null) throw new ArgumentNullException(nameof(mainMenuUserControl));
            if (featuresManager == null) throw new ArgumentNullException(nameof(featuresManager));
            if (newsViewModelFactory == null) throw new ArgumentNullException(nameof(newsViewModelFactory));
            if (telemetry == null) throw new ArgumentNullException(nameof(telemetry));
            this.combinedLogsUserControl = combinedLogsUserControl;
            this.mainMenuUserControl = mainMenuUserControl;
            this.featuresManager = featuresManager;
            this.telemetry = telemetry;

            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupFeaturesManager();
            SetupLogView();
            SetupMenuView();
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

                btn.Click += (o, args) =>
                {
                    telemetry.TrackEvent($"Feature clicked: " + f.Name);
                    feature.Show();
                };
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
