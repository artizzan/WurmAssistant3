using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AldursLab.WurmAssistant3.Areas.Core;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main.ViewModels
{
    [KernelBind]
    public class NewsViewModel : Screen
    {
        readonly IChangelogManager changelogManager;
        readonly IWindowManager windowManager;

        string _changelogText = string.Empty;
        int tabControlIndex;

        public NewsViewModel(
            [NotNull] IChangelogManager changelogManager,
            [NotNull] IWindowManager windowManager)
        {
            if (changelogManager == null) throw new ArgumentNullException(nameof(changelogManager));
            if (windowManager == null) throw new ArgumentNullException(nameof(windowManager));
            this.changelogManager = changelogManager;
            this.windowManager = windowManager;
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
            var newChangelogEntries = changelogManager.GetNewChanges() ?? string.Empty;

            bool anyNews = !string.IsNullOrWhiteSpace(newChangelogEntries);
            
            ChangelogText = newChangelogEntries;
            
            if (anyNews)
            {
                windowManager.ShowWindow(this);
                changelogManager.UpdateLastChangeDate();
            }
        }

        public void ShowForAllNews()
        {
            ChangelogText = changelogManager.GetChanges(DateTimeOffset.MinValue) ?? string.Empty;
            windowManager.ShowWindow(this);
        }
    }
}
