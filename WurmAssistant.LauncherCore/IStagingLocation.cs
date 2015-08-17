using System;
using System.IO;
using System.Linq;

namespace AldursLab.WurmAssistant.LauncherCore
{
    public interface IStagingLocation
    {
        bool AnyPackageStaged { get; }
        IStagedPackage CreatePackageFromSevenZipByteArray(byte[] zipFileAsBytes, Version version);
        IStagedPackage GetLatestStagedPackage();
        void ClearStagingArea();
        void ClearExtractionDir();
        void ExtractIntoExtractionDir(IStagedPackage package);
        void MoveExtractionDir(string newPath);
        FileInfo CreateTempFile();
    }

    public class StagingLocation : IStagingLocation
    {
        readonly string stagingDirPath;
        readonly string extractionDirPath;

        public StagingLocation(string stagingDirPath)
        {
            if (stagingDirPath == null)
                throw new ArgumentNullException("stagingDirPath");
            this.stagingDirPath = stagingDirPath;

            if (!Path.IsPathRooted(stagingDirPath))
            {
                throw new InvalidOperationException("rootPath must be absolute");
            }

            if (!Directory.Exists(stagingDirPath))
            {
                Directory.CreateDirectory(stagingDirPath);
            }

            extractionDirPath = Path.Combine(stagingDirPath, "Extracted");
        }

        public bool AnyPackageStaged
        {
            get
            {
                return Directory.GetFiles(stagingDirPath).Any();
            }
        }

        public IStagedPackage GetLatestStagedPackage()
        {
            var allStagedFiles = Directory.GetFiles(stagingDirPath).Select(s => new FileInfo(s)).ToArray();
            var latestVersion = allStagedFiles
                .Select(info => new Version(Path.GetFileNameWithoutExtension(info.Name)))
                .OrderBy(version => version)
                .First();
            var latestFile = allStagedFiles.Single(info => info.Name.StartsWith(latestVersion.ToString()));
            return new SevenZipStagedPackage(latestFile.FullName);
        }

        public void ClearStagingArea()
        {
            var allStagedFiles = Directory.GetFiles(stagingDirPath).Select(s => new FileInfo(s)).ToArray();
            foreach (var file in allStagedFiles)
            {
                file.Delete();
            }
        }

        public IStagedPackage CreatePackageFromSevenZipByteArray(byte[] zipFileAsBytes, Version version)
        {
            var targetPath = Path.Combine(stagingDirPath, version.ToString() + ".7z");

            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            var tempPath = CreateTempFile().FullName;

            File.WriteAllBytes(tempPath, zipFileAsBytes);
            File.Move(tempPath, targetPath);
            return new SevenZipStagedPackage(targetPath);
        }

        public FileInfo CreateTempFile()
        {
            var tempPath = Path.Combine(stagingDirPath, Guid.NewGuid().ToString());
            return new FileInfo(tempPath);
        }

        public void ClearExtractionDir()
        {
            if (Directory.Exists(extractionDirPath))
            {
                Directory.Delete(extractionDirPath, true);
            }
        }

        public void ExtractIntoExtractionDir(IStagedPackage package)
        {
            ClearExtractionDir();
            package.ExtractIntoDirectory(extractionDirPath);
        }

        public void MoveExtractionDir(string newPath)
        {
            Directory.Move(extractionDirPath, newPath);
        }
    }
}