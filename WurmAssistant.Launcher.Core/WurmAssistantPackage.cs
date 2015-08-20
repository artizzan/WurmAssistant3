using System;
using System.IO;
using SevenZip;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IStagedPackage
    {
        Version Version { get; }

        void ExtractIntoDirectory(string targetDir);
    }

    public class SevenZipStagedPackage : IStagedPackage
    {
        readonly FileInfo fileInfo;

        public SevenZipStagedPackage(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");

            if (!filePath.EndsWith(".7z"))
            {
                throw new InvalidOperationException("Only *.7z packages are supported");
            }

            SevenZipManager.EnsurePathSet();

            this.fileInfo = new FileInfo(filePath);
            Version = Version.Parse(Path.GetFileNameWithoutExtension(filePath));
        }

        public Version Version { get; private set; }

        public void ExtractIntoDirectory(string targetDir)
        {
            var extractor = new SevenZipExtractor(fileInfo.FullName);
            extractor.ExtractArchive(targetDir);
        }
    }
}