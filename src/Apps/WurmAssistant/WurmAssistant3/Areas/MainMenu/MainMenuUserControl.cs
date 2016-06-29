using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Debugging.Contracts;
using AldursLab.WurmAssistant3.Areas.Main;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.MainMenu
{
    [KernelBind(BindingHint.Singleton)]
    public partial class MainMenuUserControl : UserControl
    {
        readonly ISettingsEditViewFactory settingsEditViewFactory;
        readonly IProcessStarter processStarter;
        readonly IUserNotifier userNotifier;
        readonly IServersEditorViewFactory serversEditorViewFactory;
        readonly IDebuggingWindowFactory debuggingWindowFactory;
        readonly INewsViewModelFactory newsViewModelFactory;
        readonly IWurmAssistantDataDirectory wurmAssistantDataDirectory;

        public MainMenuUserControl(
            [NotNull] ISettingsEditViewFactory settingsEditViewFactory,
            [NotNull] IProcessStarter processStarter, 
            [NotNull] IUserNotifier userNotifier,
            [NotNull] IServersEditorViewFactory serversEditorViewFactory,
            [NotNull] IDebuggingWindowFactory debuggingWindowFactory,
            [NotNull] INewsViewModelFactory newsViewModelFactory,
            [NotNull] IWurmAssistantDataDirectory wurmAssistantDataDirectory)
        {
            if (settingsEditViewFactory == null) throw new ArgumentNullException("settingsEditViewFactory");
            if (processStarter == null) throw new ArgumentNullException("processStarter");
            if (userNotifier == null) throw new ArgumentNullException("userNotifier");
            if (serversEditorViewFactory == null) throw new ArgumentNullException("serversEditorViewFactory");
            if (debuggingWindowFactory == null) throw new ArgumentNullException(nameof(debuggingWindowFactory));
            if (newsViewModelFactory == null) throw new ArgumentNullException(nameof(newsViewModelFactory));
            if (wurmAssistantDataDirectory == null) throw new ArgumentNullException(nameof(wurmAssistantDataDirectory));
            this.settingsEditViewFactory = settingsEditViewFactory;
            this.processStarter = processStarter;
            this.userNotifier = userNotifier;
            this.serversEditorViewFactory = serversEditorViewFactory;
            this.debuggingWindowFactory = debuggingWindowFactory;
            this.newsViewModelFactory = newsViewModelFactory;
            this.wurmAssistantDataDirectory = wurmAssistantDataDirectory;

            InitializeComponent();

            debugToolStripMenuItem.Visible = false;
#if DEBUG
            debugToolStripMenuItem.Visible = true;
#endif
        }

        private void changeSettingsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var view = settingsEditViewFactory.CreateSettingsEditView();
            view.ShowDialog();
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
            processStarter.StartSafe(Resources.RevealCreaturesVideoUrl);
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
    }
}
