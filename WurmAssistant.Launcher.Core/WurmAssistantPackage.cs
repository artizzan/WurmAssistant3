using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

//using SevenZip;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IStagedPackage
    {
        Version Version { get; }

        void ExtractIntoDirectory(string targetDir);
    }

    public class StagedPackageMock : IStagedPackage
    {
        public Version Version { get; private set; }
        public void ExtractIntoDirectory(string targetDir)
        {
        }
    }

    public class ZippedStagedPackage : IStagedPackage
    {
        readonly FileInfo fileInfo;

        public ZippedStagedPackage(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");

            if (!filePath.EndsWith(".zip"))
            {
                throw new InvalidOperationException("Only *.zip packages are supported");
            }

            this.fileInfo = new FileInfo(filePath);
            Version = Version.Parse(Path.GetFileNameWithoutExtension(filePath));
        }

        public Version Version { get; private set; }

        public void ExtractIntoDirectory(string targetDir)
        {
            FastZip fastZip = new FastZip();

            fastZip.ExtractZip(fileInfo.FullName, targetDir, null);
        }
    }
}