using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Services;
using AldursLab.WurmAssistant3.Areas.Main.Contracts;
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


        public MainForm(
            [NotNull] IConsoleArgs consoleArgs,
            [NotNull] CombinedLogsUserControl combinedLogsUserControl,
            [NotNull] MainMenuUserControl mainMenuUserControl,
            [NotNull] IFeaturesManager featuresManager,
            [NotNull] INewsViewModelFactory newsViewModelFactory)
        {
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            if (combinedLogsUserControl == null) throw new ArgumentNullException(nameof(combinedLogsUserControl));
            if (mainMenuUserControl == null) throw new ArgumentNullException(nameof(mainMenuUserControl));
            if (featuresManager == null) throw new ArgumentNullException(nameof(featuresManager));
            if (newsViewModelFactory == null) throw new ArgumentNullException(nameof(newsViewModelFactory));
            this.combinedLogsUserControl = combinedLogsUserControl;
            this.mainMenuUserControl = mainMenuUserControl;
            this.featuresManager = featuresManager;

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
