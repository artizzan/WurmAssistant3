using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Main.Services.NewsServices
{
    public class DirectoryNewsInstance : NewsInstance
    {
        public DirectoryNewsInstance([NotNull] DirectoryInfo directoryInfo, [NotNull] ILogger logger)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            Version version;
            VersionParsed = Version.TryParse(directoryInfo.Name, out version);
            if (version != null)
            {
                version = FixNegativeVersionNumbers(version);
            }
            Version = version ?? new Version(0,0,0,0);
            Path = directoryInfo.FullName;

            var releaseFile = directoryInfo.GetFiles("release.xml").FirstOrDefault();
            if (releaseFile != null)
            {
                try
                {
                    // ReSharper disable PossibleNullReferenceException
                    NewsUrl = XDocument.Load(releaseFile.FullName).Element("Release").Element("NewsLink").Attribute("Url").Value;
                    // ReSharper restore PossibleNullReferenceException
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Unable to parse release.xml in directory " + directoryInfo.FullName);
                }
            }
            else
            {
                logger.Error("Unable to find release.xml in directory " + directoryInfo.FullName);
            }
        }

        Version FixNegativeVersionNumbers(Version version)
        {
            return new Version(
                version.Major < 0 ? 0 : version.Major,
                version.Minor < 0 ? 0 : version.Minor,
                version.Build < 0 ? 0 : version.Build,
                version.Revision < 0 ? 0 : version.Revision);
        }

        public override string VersionString => $"{Version.Major}.{Version.Minor}";
    }
}