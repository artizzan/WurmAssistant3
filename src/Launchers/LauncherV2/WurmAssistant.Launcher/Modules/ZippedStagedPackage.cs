using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class ZippedStagedPackage : IStagedPackage
    {
        readonly FileInfo fileInfo;

        public ZippedStagedPackage(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");

            this.fileInfo = new FileInfo(filePath);
        }

        public void ExtractIntoDirectory(string targetDir)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(fileInfo.FullName, targetDir, null);
        }
    }
}