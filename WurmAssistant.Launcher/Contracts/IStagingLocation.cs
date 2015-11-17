using System.IO;
using AldursLab.WurmAssistant.Launcher.Modules;

namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    public interface IStagingLocation
    {
        bool AnyPackageStaged { get; }
        IStagedPackage CreatePackageFromZipFile(string filePath);
        IStagedPackage GetStagedPackage();
        void ClearStagingArea();
        void ClearExtractionDir();
        void ExtractIntoExtractionDir(IStagedPackage package);
        void MoveExtractionDir(string newPath);
        FileInfo CreateTempFile();
    }
}