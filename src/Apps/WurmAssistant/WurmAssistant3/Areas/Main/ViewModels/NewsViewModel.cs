using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Main.Services;
using AldursLab.WurmAssistant3.Areas.Main.Services.NewsServices;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main.ViewModels
{
    [KernelBind]
    public class NewsViewModel : Screen
    {
        readonly IChangelogManager changelogManager;
        readonly NewsService newsService;
        readonly IWindowManager windowManager;
        readonly IProcessStarter processStarter;

        string _changelogText = string.Empty;
        IEnumerable<NewsInstance> _newsInstances;
        int tabControlIndex;

        public NewsViewModel(
            [NotNull] IChangelogManager changelogManager, 
            [NotNull] NewsService newsService,
            [NotNull] IWindowManager windowManager,
            [NotNull] IProcessStarter processStarter)
        {
            if (changelogManager == null) throw new ArgumentNullException(nameof(changelogManager));
            if (newsService == null) throw new ArgumentNullException(nameof(newsService));
            if (windowManager == null) throw new ArgumentNullException(nameof(windowManager));
            if (processStarter == null) throw new ArgumentNullException(nameof(processStarter));
            this.changelogManager = changelogManager;
            this.newsService = newsService;
            this.windowManager = windowManager;
            this.processStarter = processStarter;
        }

        public string ChangelogText
        {
            get { return _changelogText; }
            private set
            {
                if (value == _changelogText) return;
                _changelogText = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<NewsInstance> NewsInstances
        {
            get { return _newsInstances; }
            private set
            {
                if (Equals(value, _newsInstances)) return;
                _newsInstances = value;
                NotifyOfPropertyChange();
            }
        }

        public string Title { get; } = "Wurm Assistant: What's New?";

        public double Width { get; } = 600D;
        public double Height { get; } = 400D;

        public int TabControlIndex
        {
            get { return tabControlIndex; }
            set
            {
                if (value == tabControlIndex) return;
                tabControlIndex = value;
                NotifyOfPropertyChange();
            }
        }

        public void ShowIfAnyUnshownNews()
        {
            bool anyNews = false;

            var newChangelogEntries = changelogManager.GetNewChanges() ?? string.Empty;
            var newNewsEntries = newsService.GetNewsSinceLastShown().ToArray();

            anyNews = !string.IsNullOrWhiteSpace(newChangelogEntries) || newNewsEntries.Any();
            
            ChangelogText = newChangelogEntries;
            NewsInstances = newNewsEntries;
            
            if (anyNews)
            {
                if (!NewsInstances.Any())
                {
                    NewsInstances = new[] { new PatchReleaseNewsInstance() };
                }
                windowManager.ShowWindow(this);
                changelogManager.UpdateLastChangeDate();
                newsService.SetAllNewsShown();
            }
        }

        public void ShowForAllNews()
        {
            ChangelogText = changelogManager.GetChanges(DateTimeOffset.MinValue) ?? string.Empty;
            NewsInstances = newsService.GetNews(new Version());
            windowManager.ShowWindow(this);
        }

        public void FollowLink(Uri uri)
        {
            processStarter.StartSafe(uri.ToString());
        }
    }
}
