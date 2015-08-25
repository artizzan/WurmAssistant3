using System;
using System.IO;
using System.Linq;
using System.Text;
using AldursLab.WurmAssistant.PublishRobot.Actions;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.PublishRobot.Parts
{
    class ChangelogBuilder
    {
        readonly ReleaseDirInfo[] releaseDirs;
        readonly IOutput output;

        public ChangelogBuilder([NotNull] ReleaseDirInfo[] releaseDirs, [NotNull] IOutput output)
        {
            if (releaseDirs == null) throw new ArgumentNullException("releaseDirs");
            if (output == null) throw new ArgumentNullException("output");
            this.releaseDirs = releaseDirs;
            this.output = output;
        }

        public string Build()
        {
            var orderedDirs = releaseDirs.OrderByDescending(info => info.Version);
            StringBuilder sb = new StringBuilder();
            foreach (var info in orderedDirs)
            {
                sb.Append(string.Format("{0}\r\n{1}", info.Version.ToString(), GetPartialChangelog(info)));
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }

        string GetPartialChangelog(ReleaseDirInfo info)
        {
            var changelogFile = info.DirectoryInfo.GetFiles("changelog.txt").SingleOrDefault();
            if (changelogFile != null)
            {
                return File.ReadAllText(changelogFile.FullName).Trim();
            }
            else
            {
                output.Write("No changelog.txt in directory " + info.DirectoryInfo.FullName);
                return "- no changes have been noted for this version";
            }
        }
    }
}
