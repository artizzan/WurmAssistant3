using System;
using System.IO;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.PublishRobot.Parts
{
    class PublishingWebService
    {
        readonly IOutput output;

        public PublishingWebService([NotNull] IOutput output)
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

        public Version GetLatestVersion(string buildType)
        {
            //todo
            return new Version();
        }
    }
}
