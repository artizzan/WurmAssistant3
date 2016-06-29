using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Main.Data;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main
{
    [KernelBind(BindingHint.Singleton)]
    public class NewsService
    {
        readonly IBinDirectory binDirectory;
        readonly ILogger logger;
        readonly MainDataContext mainDataContext;

        readonly List<NewsInstance> newsInstances;

        public NewsService(
            [NotNull] IBinDirectory binDirectory, 
            [NotNull] ILogger logger,
            [NotNull] MainDataContext mainDataContext)
        {
            if (binDirectory == null) throw new ArgumentNullException(nameof(binDirectory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (mainDataContext == null) throw new ArgumentNullException(nameof(mainDataContext));
            this.binDirectory = binDirectory;
            this.logger = logger;
            this.mainDataContext = mainDataContext;

            var newsDir = new DirectoryInfo(Path.Combine(binDirectory.FullPath, "Releases"));
            if (newsDir.Exists)
            {
                var allVersions =
                    newsDir.GetDirectories()
                           .Select(info => new DirectoryNewsInstance(info, logger))
                           .Cast<NewsInstance>()
                           .ToArray();
                newsInstances = allVersions.OrderByDescending(instance => instance.Version).ToList();
                var errors = allVersions.Where(instance => !instance.VersionParsed);
                foreach (var newsInstance in errors)
                {
                    logger.Error("Found invalid release directory at: " + newsInstance.Path);
                }
            }
            else
            {
                logger.Error("Releases root directory does not exist.");
            }
        }

        public IEnumerable<NewsInstance> GetNews(Version fromVersion)
        {
            return newsInstances.Where(instance => instance.Version > fromVersion);
        }

        public IEnumerable<NewsInstance> GetNewsSinceLastShown()
        {
            var oldVersion = mainDataContext.News.LastShownNewsVersion;
            return GetNews(oldVersion ?? new Version(0,0,0,0));
        }

        public void SetAllNewsShown()
        {
            var maxVersion = newsInstances
                .Select(instance => instance.Version)
                .DefaultIfEmpty(new Version(0, 0, 0, 0))
                .Max();
            mainDataContext.News.LastShownNewsVersion = maxVersion;
        }
    }
}
