using System;
using System.IO;
using System.Linq;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Dto;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class StagingLocation : IStagingLocation
    {
        readonly DirectoryInfo stageDir;
        readonly DirectoryInfo extractionDir;
        readonly DirectoryInfo tempDir;

        public StagingLocation(ControllerConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            var stagingDirPath = Path.Combine(config.LauncherBinDirFullPath, "stage");

            if (!Path.IsPathRooted(stagingDirPath))
            {
                throw new InvalidOperationException("rootPath must be absolute");
            }

            if (!Directory.Exists(stagingDirPath))
            {
                Directory.CreateDirectory(stagingDirPath);
            }

            stageDir = new DirectoryInfo(Path.Combine(stagingDirPath, "Stage"));
            stageDir.Create();
            extractionDir = new DirectoryInfo(Path.Combine(stagingDirPath, "Extracted"));
            extractionDir.Create();
            tempDir = new DirectoryInfo(Path.Combine(stagingDirPath, "Temp"));
            tempDir.Create();
            ClearTempDir();
        }

        void ClearTempDir()
        {
            foreach (var fileInfo in tempDir.GetFiles())
            {
                fileInfo.Delete();
            }
        }

        public bool AnyPackageStaged
        {
            get
            {
                return Directory.GetFiles(stageDir.FullName).Any();
            }
        }

        public IStagedPackage GetStagedPackage()
        {
            var stagedFile = new FileInfo(Directory.GetFiles(stageDir.FullName).Single());
            return new ZippedStagedPackage(stagedFile.FullName);
        }

        public void ClearStagingArea()
        {
            var allStagedFiles = Directory.GetFiles(stageDir.FullName).Select(s => new FileInfo(s)).ToArray();
            foreach (var file in allStagedFiles)
            {
                file.Delete();
            }
        }

        public IStagedPackage CreatePackageFromZipFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var targetPath = Path.Combine(stageDir.FullName, fileInfo.Name);

            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            File.Move(filePath, targetPath);
            return new ZippedStagedPackage(targetPath);
        }

        public FileInfo CreateTempFile()
        {
            var tempPath = Path.Combine(tempDir.FullName, Guid.NewGuid().ToString());
            return new FileInfo(tempPath);
        }

        public void ClearExtractionDir()
        {
            if (Directory.Exists(extractionDir.FullName))
            {
                Directory.Delete(extractionDir.FullName, true);
            }
        }

        public void ExtractIntoExtractionDir(IStagedPackage package)
        {
            ClearExtractionDir();
            package.ExtractIntoDirectory(extractionDir.FullName);
        }

        public void MoveExtractionDir(string newPath)
        {
            Directory.Move(extractionDir.FullName, newPath);
        }
    }
}