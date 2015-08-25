using System;
using System.IO;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.PublishRobot.Parts
{
    class PackageWebPublisher
    {
        readonly IOutput output;

        public PackageWebPublisher([NotNull] IOutput output)
        {
            if (output == null) throw new ArgumentNullException("output");
            this.output = output;
        }

        public void Publish(FileInfo zipFile, Version latestVersion, string buildType)
        {
            //todo
            output.Write(string.Format("Package uploading operation placeholder, args: {0}, {1}, {2}",
                zipFile.FullName,
                latestVersion,
                buildType));
        }
    }
}
