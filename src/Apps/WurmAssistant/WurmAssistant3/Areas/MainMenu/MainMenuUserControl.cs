using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Debugging.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Main;
using AldursLab.WurmAssistant3.Areas.Main.ViewModels;
using AldursLab.WurmAssistant3.Areas.WurmApi;
using AldursLab.WurmAssistant3.Properties;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.MainMenu
{
    [KernelBind(BindingHint.Singleton)]
    public partial class MainMenuUserControl : UserControl
    {
        readonly IProcessStarter processStarter;
        readonly IUserNotifier userNotifier;
        readonly IServersEditorViewFactory serversEditorViewFactory;
        readonly IDebuggingWindowFactory debuggingWindowFactory;
        readonly INewsViewModelFactory newsViewModelFactory;
        readonly IWurmAssistantDataDirectory wurmAssistantDataDirectory;
        readonly IWurmAssistantConfig wurmAssistantConfig;
        readonly IEnvironment environment;
        readonly IWurmClientValidatorFactory wurmClientValidatorFactory;
        readonly ILogger logger;
        readonly IOptionsFormFactory optionsFormFactory;
        readonly IDataBackupsViewModelFactory dataBackupsViewModelFactory;
        readonly IWindowManager windowManager;

        public MainMenuUserControl(
            [NotNull] IProcessStarter processStarter, 
            [NotNull] IUserNotifier userNotifier,
            [NotNull] IServersEditorViewFactory serversEditorViewFactory,
            [NotNull] IDebuggingWindowFactory debuggingWindowFactory,
            [NotNull] INewsViewModelFactory newsViewModelFactory,
            [NotNull] IWurmAssistantDataDirectory wurmAssistantDataDirectory,
            [NotNull] IWurmAssistantConfig wurmAssistantConfig,
            [NotNull] IEnvironment environment,
            [NotNull] IWurmClientValidatorFactory wurmClientValidatorFactory,
            [NotNull] ILogger logger,
            [NotNull] IOptionsFormFactory optionsFormFactory,
            [NotNull] IDataBackupsViewModelFactory dataBackupsViewModelFactory,
            [NotNull] IWindowManager windowManager)
        {
            if (processStarter == null) throw new ArgumentNullException(nameof(processStarter));
            if (userNotifier == null) throw new ArgumentNullException(nameof(userNotifier));
            if (serversEditorViewFactory == null) throw new ArgumentNullException(nameof(serversEditorViewFactory));
            if (debuggingWindowFactory == null) throw new ArgumentNullException(nameof(debuggingWindowFactory));
            if (newsViewModelFactory == null) throw new ArgumentNullException(nameof(newsViewModelFactory));
            if (wurmAssistantDataDirectory == null) throw new ArgumentNullException(nameof(wurmAssistantDataDirectory));
            if (wurmAssistantConfig == null) throw new ArgumentNullException(nameof(wurmAssistantConfig));
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (wurmClientValidatorFactory == null) throw new ArgumentNullException(nameof(wurmClientValidatorFactory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (optionsFormFactory == null) throw new ArgumentNullException(nameof(optionsFormFactory));
            if (dataBackupsViewModelFactory == null)
                throw new ArgumentNullException(nameof(dataBackupsViewModelFactory));
            if (windowManager == null) throw new ArgumentNullException(nameof(windowManager));
            this.processStarter = processStarter;
            this.userNotifier = userNotifier;
            this.serversEditorViewFactory = serversEditorViewFactory;
            this.debuggingWindowFactory = debuggingWindowFactory;
            this.newsViewModelFactory = newsViewModelFactory;
            this.wurmAssistantDataDirectory = wurmAssistantDataDirectory;
            this.wurmAssistantConfig = wurmAssistantConfig;
            this.environment = environment;
            this.wurmClientValidatorFactory = wurmClientValidatorFactory;
            this.logger = logger;
            this.optionsFormFactory = optionsFormFactory;
            this.dataBackupsViewModelFactory = dataBackupsViewModelFactory;
            this.windowManager = windowManager;

            InitializeComponent();

            debugToolStripMenuItem.Visible = false;
#if DEBUG
            debugToolStripMenuItem.Visible = true;
#endif
        }

        private void officialForumToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            processStarter.StartSafe(
                "http://forum.wurmonline.com/index.php?/topic/68031-wurm-assistant-enrich-your-wurm-experience/?view=getnewpost");
        }

        private void wikiToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            processStarter.StartSafe(
                "http://wurmassistant.wikia.com/wiki/Wurm_Assistant_Wiki");
        }

        private void pMAldurToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            processStarter.StartSafe(
                "http://forum.wurmonline.com/index.php?/user/6302-aldur/");
        }

        void modifyServersListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var view = serversEditorViewFactory.CreateServersEditorView();
            view.ShowDialog();
        }

        private void videoSoundManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.SoundManagerVideoUrl);
        }

        private void videoLogSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.LogSearcherVideoUrl);
        }

        private void videoCalendarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.CalendarVideoUrl);
        }

        private void videoTimersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.TimersVideoUrl);
        }

        private void videoTriggersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.TriggersVideoUrl);
        }

        private void videoGrangerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.GrangerVideoUrl);
        }

        private void videoCraftingAssistantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.CraftingAssistantVideoUrl);
        }

        private void videoRevealCreaturesParserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.RevealCreaturesVideoUrl);
        }

        private void videoSkillStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.SkillStatsVideoUrl);
        }

        private void videoCombatStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Resources.CombatStatsVideoUrl);
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debuggingWindowFactory.CreateDebuggingWindow().Show();
        }

        private void showNewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vm = newsViewModelFactory.CreateNewsViewModel();
            vm.ShowForAllNews();
        }

        private void viewRoadmapToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            processStarter.StartSafe(
                "https://trello.com/b/FlIPQ7TW/wurm-assistant-3-roadmap");
        }

        private void manageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe(Path.Combine(
                wurmAssistantDataDirectory.DirectoryPath,
                "Plugins"));
        }

        private void validateWurmGameClientConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var validator = wurmClientValidatorFactory.CreateWurmClientValidator();
                var result = validator.Validate();
                if (result.Any())
                {
                    validator.ShowSummaryWindow(result);
                }
                else
                {
                    userNotifier.NotifyWithMessageBox("No issues found");
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Unable to validate game client configs.");
                userNotifier.NotifyWithMessageBox(
                    $"Unable to validate game client configs due to error: {exception.Message}\r\nPlease check logs for details.");

            }
        }

        private void clearWurmApiCachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wurmAssistantConfig.DropAllWurmApiCachesToggle = true;
            environment.Restart();
        }

        private void changeGameClientPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wurmAssistantConfig.WurmApiResetRequested = true;
            environment.Restart();
        }

        private void howtoManagePluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe("https://github.com/mdsolver/WurmAssistant3/wiki/Managing-Plugins");
        }

        private void howtoCreatePluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processStarter.StartSafe("https://github.com/mdsolver/WurmAssistant3/wiki/Plugin-Quick-Start");
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form = optionsFormFactory.CreateOptionsForm();
            if (this.ParentForm != null)
            {
                form.ShowDialogCenteredOnForm(this.ParentForm);
            }
            else
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void dataBackupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vm = dataBackupsViewModelFactory.CreateDataBackupsViewModel();
            windowManager.ShowWindow(vm);
        }
    }
}
